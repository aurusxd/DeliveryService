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
        /// Открытие окна
        /// </summary>
        /// <typeparam name="TView">Класс открываемого окна</typeparam>
        private void OpenWindow<TView>() where TView : Window
        {
            var type = typeof(TView);

            if (_openedWindows.TryGetValue(type, out var window) && window.IsVisible)
            {
                window.Activate();
                return;
            }

            var win = _services.GetRequiredService<TView>();
            win.Closed += (s, e) => _openedWindows.Remove(type);
            win.Show();
            _openedWindows[type] = win;
        }
        /// <summary>
        /// Открытие окна как модальное
        /// </summary>
        /// <typeparam name="TView">Класс открываемого окна</typeparam>
        /// <returns>Результат работы окна - DialogResult</returns>
        private bool? OpenModalWindow<TView>() where TView : Window
        {
            var win = _services.GetRequiredService<TView>();
            return win.ShowDialog();
        }

        /// <summary>
        /// Открытие DispatcherView
        /// </summary>
        public void OpenDispatcher() => OpenWindow<DispatcherView>();
        /// <summary>
        /// Открытие OrderListView
        /// </summary>
        public void OpenOrderList() => OpenWindow<OrdersListView>();
        /// <summary>
        /// Открытие ListCouriersView
        /// </summary>
        public void OpenListCouriers() => OpenWindow<ListCouriersView>();
        /// <summary>
        /// Открытие MenuView
        /// </summary>
        public void OpenMenu() => OpenWindow<MenuView>();
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
            var win = _services.GetRequiredService<NewOrderView>();
            if (win.DataContext is NewOrderViewModel vm)
                vm.SetCurrentUserId(userId);

            return win.ShowDialog();
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