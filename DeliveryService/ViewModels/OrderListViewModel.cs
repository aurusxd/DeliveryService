using DeliveryService.Commands;
using DeliveryService.DTO;
using DeliveryService.Services;
using DeliveryService.Views;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace DeliveryService.ViewModels
{
    /// <summary>
    /// Логика взаимодействия пользователя и базы данных с OrderListView
    /// </summary>
    public class OrderListViewModel : BaseViewModel
    {
        private readonly WindowsService _windowsService;
        private readonly OrderService _orderService;

        /// <summary>
        /// Список DTO всех заказов
        /// </summary>
        private List<OrderDTO> _allOrders;
        /// <summary>
        /// Список DTO всех заказов для отображения
        /// </summary>
        private ObservableCollection<OrderDTO> _orders;
        /// <summary>
        /// Фильтр списка заказов
        /// </summary>
        private string _filter;

        /// <summary>
        /// Количество заказов
        /// </summary>
        private int _totalCount;
        /// <summary>
        /// Количество заказов со статусом "В процессе" (поменять в кавычках на название в проекте)
        /// </summary>
        private int _inProcessCount;
        /// <summary>
        /// Количество заказов со статусом "Ожидают" (поменять в кавычках на название в проекте)
        /// </summary>
        private int _pendingCount;
        /// <summary>
        /// Количество заказов со статусом "Завершено" (поменять в кавычках на название в проекте)
        /// </summary>
        private int _completedCount;

        /// <summary>
        /// Список DTO всех заказов для отображения
        /// </summary>
        public ObservableCollection<OrderDTO> Orders
        {
            get => _orders;
            set => SetProperty(ref _orders, value);
        }
        /// <summary>
        /// Фильтр списка заказов
        /// </summary>
        public string Filter
        {
            get => _filter;
            set
            {
                if (SetProperty(ref _filter, value))
                    ApplyFilter();
            }
        }
        /// <summary>
        /// Количество заказов
        /// </summary>
        public int TotalCount
        {
            get => _totalCount;
            set => SetProperty(ref _totalCount, value);
        }
        /// <summary>
        /// Количество заказов со статусом "В процессе" (поменять в кавычках на название в проекте)
        /// </summary>
        public int InProcessCount
        {
            get => _inProcessCount;
            set => SetProperty(ref _inProcessCount, value);
        }
        /// <summary>
        /// Количество заказов со статусом "Ожидают" (поменять в кавычках на название в проекте)
        /// </summary>
        public int PendingCount
        {
            get => _pendingCount;
            set => SetProperty(ref _pendingCount, value);
        }
        /// <summary>
        /// Количество заказов со статусом "Завершено" (поменять в кавычках на название в проекте)
        /// </summary>
        public int CompletedCount
        {
            get => _completedCount;
            set => SetProperty(ref _completedCount, value);
        }

        /// <summary>
        /// Команда загрузки заказов в таблицу
        /// </summary>
        public ICommand LoadOrdersCommand { get; }
        /// <summary>
        /// Команда открытия окна добавления нового заказа
        /// </summary>
        public ICommand AddOrderCommand { get; }


        public OrderListViewModel(WindowsService windowsService, OrderService orderService)
        {
            _windowsService = windowsService;
            _orderService = orderService;

            _allOrders = new List<OrderDTO>();
            Orders = new ObservableCollection<OrderDTO>();

            LoadOrdersCommand = new RelayCommandAsync(
                execute: () => TryRunTaskAsync(LoadOrdersAsync, "Ошибка загрузки"),
                canExecute: () => !IsBusy
            );

            //AddOrderCommand = new RelayCommand(OpenAddOrderWindow);
            AddOrderCommand = new RelayCommand(() => {
                if (_windowsService.OpenNewOrder() == true)
                    LoadOrdersCommand.Execute(null);
            });

            LoadOrdersCommand.Execute(null);
        }


        /// <summary>
        /// Сбор статистики по списку заказов - полное кол-во, кол-во с определёнными статусами
        /// </summary>
        /// <param name="orders">Список заказов</param>
        private void SetOrderStatistic(ObservableCollection<OrderDTO> orders)
        {
            TotalCount = orders.Count;
            // Изменить названия статусов на нужные проекту
            InProcessCount = orders.Count(o => o.Status == "InProgress");
            PendingCount = orders.Count(o => o.Status == "Pending");
            CompletedCount = orders.Count(o => o.Status == "Done");
        }
        /// <summary>
        /// Загрузка данных о заказах в список
        /// </summary>
        private async Task LoadOrdersAsync()
        {
            var orders = await _orderService.GetAllAsync();
            var items = new List<OrderDTO>();

            foreach (var order in orders)
            {
                string clientName = order.Client?.Name ?? string.Empty;

                items.Add(new OrderDTO
                {
                    Id = order.Id,
                    ClientName = clientName,
                    Route = $"{order.Address_From} → {order.Address_To}",
                    Status = order.Status ?? "—",
                    Price = order.Price,
                    OrderTime = order.Created_At.ToString()
                });
            }

            _allOrders = items;
            Orders = new ObservableCollection<OrderDTO>(_allOrders);

            SetOrderStatistic(Orders);
        }
        /// <summary>
        /// Загрузка списка заказов с учётом фильтрации по имени клиента или ардресам откуда и куда
        /// </summary>
        private void ApplyFilter()
        {
            IEnumerable<OrderDTO> filtered = _allOrders;

            if (!string.IsNullOrWhiteSpace(Filter))
            {
                string search = Filter.Trim().ToLower();
                filtered = _allOrders.Where(o =>
                    o.ClientName?.ToLower().Contains(search) == true ||
                    o.Route?.ToLower().Contains(search) == true
                );
            }

            Orders = new ObservableCollection<OrderDTO>(filtered);
            SetOrderStatistic(Orders);
        }

        /// <summary>
        /// Открытие окна создания нового заказа
        /// </summary>
        private void OpenAddOrderWindow()
        {
            var win = App.Services.GetRequiredService<NewOrderView>();
            if (win.ShowDialog() == true)
                LoadOrdersCommand.Execute(null);
        }
    }
}