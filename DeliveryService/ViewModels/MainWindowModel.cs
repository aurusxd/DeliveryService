using DeliveryService.Commands;
using DeliveryService.Services;
using System.Windows.Input;
using System.Windows.Navigation;

namespace DeliveryService.ViewModels
{
    /// <summary>
    /// Логика взаимодействия пользователя с MainWindow
    /// </summary>
    public class MainWindowModel : BaseViewModel
    {
        private readonly WindowsService _windowsService;

        /// <summary>
        /// Команда открытия DispatcherView
        /// </summary>
        public ICommand OpenDispatcherCommand { get; }
        /// <summary>
        /// Команда открытия OrderListView
        /// </summary>
        public ICommand OpenOrderListCommand { get; }
        /// <summary>
        /// Команда открытия ListCouriersView
        /// </summary>
        public ICommand OpenListCouriersCommand { get; }
        /// <summary>
        /// Команда открытия NewOrderView
        /// </summary>
        public ICommand OpenNewOrderCommand { get; }
        /// <summary>
        /// Команда открытия RegistrationCouriers
        /// </summary>
        public ICommand OpenRegistrationCourierCommand { get; }
        /// <summary>
        /// Команда закрытия всех открытых немодальных окон
        /// </summary>
        public ICommand CloseWindowsCommand { get; }

        private object _currentView;
        private readonly DispatcherViewModel _dispatcherVm;
        private readonly OrderListViewModel _ordersVm;
        private readonly ListCouriersViewModel _couriersVm;

        public object CurrentView
        {
            get => _currentView;
            set {
                switch (value)
                {
                    case OrderListViewModel o: o.LoadOrdersCommand.Execute(null); break;
                    case DispatcherViewModel d: d.LoadDataCommand.Execute(null); break;
                    case ListCouriersViewModel d: d.LoadCouriersCommand.Execute(null); break;
                }
                SetProperty(ref _currentView, value);
                }
        }


        public MainWindowModel(
            OrderListViewModel ordersVm,
            DispatcherViewModel dispatcherVm,
            ListCouriersViewModel couriersVm,
            WindowsService windowsService)
        {

            _dispatcherVm = dispatcherVm;
            _ordersVm = ordersVm;
            _couriersVm = couriersVm;

            CurrentView = ordersVm;
            _windowsService = windowsService;

            OpenDispatcherCommand = new RelayCommand(() =>
                CurrentView = _dispatcherVm


                );
            OpenOrderListCommand = new RelayCommand(()=>
                CurrentView = _ordersVm
                
                );
            OpenListCouriersCommand = new RelayCommand(()=>
                CurrentView = _couriersVm
                
                
                );

            // Эти потом можно изменить с проверками DialogResult
            OpenNewOrderCommand = new RelayCommand(() => {
                _windowsService.OpenNewOrder();
            });
            OpenRegistrationCourierCommand = new RelayCommand(() => { 
                _windowsService.OpenRegistrationCourier();
            });


            CloseWindowsCommand = new RelayCommand(_windowsService.CloseWindows);
        }
    }
}