using DeliveryService.ViewModels;
using DeliveryService.Views;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace DeliveryService.Services
{
    /// <summary>
    /// Сервис, открывающий окна
    /// </summary>
    public class WindowsService
    {
        private readonly IServiceProvider _services;
        /// <summary>
        /// Словарь открытых, не модальных, окон
        /// </summary>
        private readonly Dictionary<Type, Window> _openedWindows;


        public WindowsService(IServiceProvider services)
        {
            _services = services;
            _openedWindows = new Dictionary<Type, Window>();
        }


        /// <summary>
        /// Открытие окна с возможностью настройки перед отображением
        /// </summary>
        /// <typeparam name="TView">Класс открываемого окна</typeparam>
        /// <param name="configure">Команда настройки окна</param>
        private void OpenWindow<TView>(Action<TView>? configure = null) where TView : Window
        {
            var type = typeof(TView);

            if (_openedWindows.TryGetValue(type, out var window) && window.IsVisible)
            {
                window.Activate();
                return;
            }

            var win = _services.GetRequiredService<TView>();
            configure?.Invoke(win);
            win.Closed += (s, e) => _openedWindows.Remove(type);
            win.Show();
            _openedWindows[type] = win;
        }
        /// <summary>
        /// Открытие окна как модальное с возможностью настройки перед отображением
        /// </summary>
        /// <typeparam name="TView">Класс открываемого окна</typeparam>
        /// <param name="configure">Команда настройки окна</param>
        /// <returns>Результат работы окна - DialogResult</returns>
        private bool? OpenModalWindow<TView>(Action<TView>? configure = null) where TView : Window
        {
            var win = _services.GetRequiredService<TView>();
            configure?.Invoke(win);
            return win.ShowDialog();
        }

        /// <summary>
        /// Открытие RegistrationView
        /// </summary>
        public void OpenRegistration() => OpenWindow<RegistrationView>();
        /// <summary>
        /// Открытие MenuView
        /// </summary>
        public void OpenMenu() => OpenWindow<MenuView>();
        /// <summary>
        /// Открытие MenuView с передачей id пользователя
        /// </summary>
        public void OpenMenu(int userId)
        {
            OpenWindow<MenuView>(win =>
            {
                if (win.DataContext is MenuViewModel vm)
                    vm.SetCurrentUserId(userId);
            });
        }
        /// <summary>
        /// Открытие NewOrderView
        /// </summary>
        /// <returns>Результат работы окна - DialogResult</returns>
        public bool? OpenNewOrder() => OpenModalWindow<NewOrderView>();
        /// <summary>
        /// Открытие NewOrderView с передачей id пользователя
        /// </summary>
        /// <param name="userId">ID пользователя</param>
        /// <returns>Результат работы окна - DialogResult</returns>
        public bool? OpenNewOrder(int userId)
        {
            return OpenModalWindow<NewOrderView>(win =>
            {
                if (win.DataContext is NewOrderViewModel vm)
                    vm.SetCurrentUserId(userId);
            });
        }
        /// <summary>
        /// Открытие MainWindow
        /// </summary>
        /// <returns>Результат работы окна - DialogResult</returns>
        public bool? OpenMainWindow() => OpenModalWindow<MainWindow>();
        /// <summary>
        /// Открытие MainWindow с передачей id пользователя
        /// </summary>
        /// <param name="userId">ID пользователя</param>
        /// <returns>Результат работы окна - DialogResult</returns>
        public bool? OpenMainWindow(int userId)
        {
            return OpenModalWindow<MainWindow>(win =>
            { 
                if (win.DataContext is MainWindowModel vm)
                    vm.SetCurrentUserId(userId);
            });
        }
        /// <summary>
        /// Открытие RegistrationCourier
        /// </summary>
        /// <returns>Результат работы окна - DialogResult</returns>
        public bool? OpenRegistrationCourier() => OpenModalWindow<RegistrationCourier>();

        /// <summary>
        /// Закрытие всех немодальных окон
        /// </summary>
        public void CloseWindows()
        {
            var openedWindows = _openedWindows.Values.ToList();
            foreach (var window in openedWindows)
            {
                if (window.IsVisible)
                    window.Close();
            }

            _openedWindows.Clear();
        }
    }
}