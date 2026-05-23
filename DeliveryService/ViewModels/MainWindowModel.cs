using DeliveryService.Commands;
using DeliveryService.Services;
using System.Windows.Input;

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


        public MainWindowModel(WindowsService windowsService)
        {
            _windowsService = windowsService;

            OpenDispatcherCommand = new RelayCommand(_windowsService.OpenDispatcher);
            OpenOrderListCommand = new RelayCommand(_windowsService.OpenOrderList);
            OpenListCouriersCommand = new RelayCommand(_windowsService.OpenListCouriers);
            OpenNewOrderCommand = new RelayCommand(_windowsService.OpenMenu);

            // Эти потом можно изменить с проверками DialogResult
            OpenRegistrationCourierCommand = new RelayCommand(() => { 
                _windowsService.OpenRegistrationCourier();
            });

            CloseWindowsCommand = new RelayCommand(_windowsService.CloseWindows);
        }
    }
}