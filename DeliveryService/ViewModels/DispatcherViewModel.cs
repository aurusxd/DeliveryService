using DeliveryService.Commands;
using DeliveryService.Models;
using DeliveryService.Services;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;
using System.Windows.Threading;

namespace DeliveryService.ViewModels
{
    /// <summary>
    /// Логика взаимодействия пользователя и базы данных с DispatcherView
    /// </summary>
    public class DispatcherViewModel : BaseViewModel
    {
        /// <summary>
        /// Интервал таймера
        /// </summary>
        private const int TIMER_INTERVAL = 30; 

        /// <summary>
        /// Таймер, который перезагружает данные
        /// </summary>
        private DispatcherTimer _refreshTimer;
        /// <summary>
        /// Активен ли таймер
        /// </summary>
        private bool _isTimerActive = true;

        private readonly OrderService _orderService;
        private readonly CourierService _courierService;

        // ВАЖНО: Поменять названия статусов в комментариях
        /// <summary>
        /// Счётчик заказов со статусом "New"
        /// </summary>
        private int _newOrderCount;
        /// <summary>
        /// Счётчик заказов со статусом "InDelivery"
        /// </summary>
        private int _inTransitOrderCount;
        /// <summary>
        /// Счётчик заказов со статусом "Done"
        /// </summary>
        private int _completedOrderCount;
        /// <summary>
        /// Выбранный заказ
        /// </summary>
        private Order _selectedOrder;
        /// <summary>
        /// Выбранный курьер
        /// </summary>
        private Courier _selectedCourier;
        /// <summary>
        /// Выбранный курьер
        /// </summary>
        public Courier SelectedCourier
        {
            get => _selectedCourier;
            set => SetProperty(ref _selectedCourier, value);
        }
        /// <summary>
        /// Выбранный Заказ
        /// </summary>
        public Order SelectedOrder
        {
            get => _selectedOrder;
            set => SetProperty(ref _selectedOrder, value);
        }

        /// <summary>
        /// Список активных заказов
        /// </summary>
        public ObservableCollection<Order> ActiveOrders { get; }
        /// <summary>
        /// Список курьеров, которые онлайн
        /// </summary>
        public ObservableCollection<Courier> OnlineCouriers { get; }

        /// <summary>
        /// Список свободных курьеров, которые онлайн
        /// </summary>
        public ObservableCollection<Courier> FreeCouriers { get; }

        // ВАЖНО: Поменять названия статусов в комментариях
        /// <summary>
        /// Счётчик заказов со статусом "New"
        /// </summary>
        public int NewOrderCount
        {
            get => _newOrderCount;
            set => SetProperty(ref _newOrderCount, value);
        }
        /// <summary>
        /// Счётчик заказов со статусом "InDelivery"
        /// </summary>
        public int InTransitOrderCount
        {
            get => _inTransitOrderCount;
            set => SetProperty(ref _inTransitOrderCount, value);
        }
        /// <summary>
        /// Счётчик заказов со статусом "Done"
        /// </summary>
        public int CompletedOrderCount
        {
            get => _completedOrderCount;
            set => SetProperty(ref _completedOrderCount, value);
        }

        /// <summary>
        /// Команда загрузки данных
        /// </summary>
        public ICommand LoadDataCommand { get; }

        /// <summary>
        /// Команда назначения курьера на заказ
        /// </summary>
        public ICommand AssignCourierCommand { get; }
        /// <summary>
        /// Команда для выбора заказа
        /// </summary>
        public ICommand SelectOrderCommand { get; }

        /// <summary>
        /// Команда для выбора курьера
        /// </summary>
        public ICommand SelectCourierCommand { get; }

        /// <summary>
        /// Событие, которое вызывается при выборе заказа 
        /// </summary>
        public event Action<Order>? OrderSelected;
        /// <summary>
        /// Событие, которое вызывается при выборе курьера 
        /// </summary>
        public event Action<double,double,double,double>? CourierSelected;

        public DispatcherViewModel(OrderService orderService, CourierService courierService)
        {
            _orderService = orderService;
            _courierService = courierService;

            ActiveOrders = new ObservableCollection<Order>();
            OnlineCouriers = new ObservableCollection<Courier>();
            FreeCouriers = new ObservableCollection<Courier>();

            LoadDataCommand = new RelayCommandAsync(
                execute: () => TryRunTaskAsync(LoadDataAsync, "Ошибка загрузки"),
                canExecute: () => !IsBusy
            );

            //LoadDataCommand.Execute(null);



            AssignCourierCommand = new RelayCommandAsync(async order =>
            {
                if (SelectedCourier == null || order == null) return;
                Order ord = (Order)order;
                bool success = await _courierService.AssignCourierToOrderAsync(SelectedCourier.Id, ord.Id);
                if (success) await LoadDataAsync();
            });

            SelectOrderCommand = new RelayCommandAsync(async order =>
            {
                if (order == null) return;
                SelectedOrder = (Order)order;
                OrderSelected?.Invoke((Order)order);
            });


            SelectCourierCommand = new RelayCommandAsync(async parameter =>
            {
                if (parameter is not Courier courier)
                    return;
                SelectedCourier = courier;

                SelectedOrder = await _orderService
                    .FindOrderByCourierIdAsync(courier.Id);

                if (SelectedOrder == null) return;
                if (SelectedCourier == null) return;

                double startLat = courier.Current_Lat;
                double startLon = courier.Current_Lon;

                bool courierAtPickup =
                Math.Abs(courier.Current_Lat - SelectedOrder.Lat_From) < 0.00001 &&
                Math.Abs(courier.Current_Lon - SelectedOrder.Lon_From) < 0.00001;

                double endLat = courierAtPickup ? SelectedOrder.Lat_To : SelectedOrder.Lat_From;
                double endLon = courierAtPickup ? SelectedOrder.Lon_To : SelectedOrder.Lon_From;

                CourierSelected?.Invoke(startLat, startLon, endLat, endLon);
            });

            InitializeTimer();
        }

        /// <summary>
        /// Загрузка данных о заказах
        /// </summary>
        private async Task LoadOrdersAsync()
        {
            var allOrders = await _orderService.GetAllAsync();

            if (allOrders != null && allOrders.Any())
            {
                var activeOrders = allOrders.Where(o => o.Status != "Done").OrderByDescending(o => o.Created_At).ToList();

                ActiveOrders.Clear();
                foreach (var order in activeOrders) 
                    ActiveOrders.Add(order);

                System.Diagnostics.Debug.WriteLine($"Заказов загружено: {ActiveOrders.Count}");

                // Поменять статусы на нужные проекту

                NewOrderCount = activeOrders.Count(o => o.Status == "new");
                InTransitOrderCount = activeOrders.Count(o => o.Status == "InDelivery");
                CompletedOrderCount = allOrders.Count(o => o.Status == "Done");
            }
            else
            {
                ActiveOrders.Clear();

                NewOrderCount = 0;
                InTransitOrderCount = 0;
                CompletedOrderCount = 0;

                return;
            }
        }
        /// <summary>
        /// Загрузка свободных курьеров
        /// </summary>
        /// <returns></returns>
        private async Task LoadFreeCouriersAsync()
        {
            var freeCouriers = await _courierService.GetFreeCouriersAsync();
            if (freeCouriers == null)
                return;

            var onlineCouriers = freeCouriers.Where(c => c.IsActive).ToList();

            FreeCouriers.Clear();
            foreach (var courier in onlineCouriers)
                FreeCouriers.Add(courier);
;

        }

        /// <summary>
        /// Назначение курьера на заказ
        /// </summary>
        /// <param name="courierId">Айди курьера</param>
        /// <param name="orderId">Айди заказа</param>
        /// <returns></returns>
        public async Task AssignCourier(int courierId, int orderId) => await _courierService.AssignCourierToOrderAsync(courierId, orderId);

        /// <summary>
        /// Загрузка данных об курьерах
        /// </summary>
        private async Task LoadCouriersAsync()
        {
            var allCouriers = await _courierService.GetAllAsync();
            if (allCouriers == null) 
                return;

            var onlineCouriers = allCouriers.Where(c => c.IsActive).ToList();

            OnlineCouriers.Clear();
            foreach (var courier in onlineCouriers)
                OnlineCouriers.Add(courier);
        }

        /// <summary>
        /// Загрузка данных
        /// </summary>
        private async Task LoadDataAsync()
        {
            await LoadOrdersAsync();
            await LoadCouriersAsync();
            await LoadFreeCouriersAsync();
        }

        /// <summary>
        /// Логика таймера
        /// </summary>
        private void OnTimerTick(object? sender, EventArgs e)
        {
            bool windowExists = Application.Current.Windows
                .Cast<Window>()
                .Any(w => w.DataContext == this);

            if (!windowExists)
            {
                if (_isTimerActive)
                {
                    _refreshTimer.Stop();
                    _refreshTimer.Tick -= OnTimerTick;
                    _isTimerActive = false;
                }
                return;
            }

            Debug.WriteLine("Done");
            LoadDataCommand.Execute(null);
        }

        /// <summary>
        /// Инициализация таймера
        /// </summary>
        private void InitializeTimer()
        {
            _refreshTimer = new DispatcherTimer();
            _refreshTimer.Interval = TimeSpan.FromSeconds(TIMER_INTERVAL);
            _refreshTimer.Tick += OnTimerTick;
            _refreshTimer.Start();
        }
    }
}