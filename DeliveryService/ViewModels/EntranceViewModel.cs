using DeliveryService.Commands;
using DeliveryService.Models;
using DeliveryService.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace DeliveryService.ViewModels
{
    public class EntranceViewModel : BaseViewModel
    {
        private readonly SessionService _sessionService;

        /// <summary>
        /// Имя
        /// </summary>
        private string _name;
        /// <summary>
        /// Пароль 
        /// </summary>
        private string _password;
        /// <summary>
        /// Юзер, который входит в систему
        /// </summary>
        private Client? _client;
        /// <summary>
        /// Роль юзера
        /// </summary>
        private string _role;

        /// <summary>
        /// Роль юзера
        /// </summary>
        public string Role
        {
            get => _role;
            set=> SetProperty(ref _role, value);
        }

        /// <summary>
        /// Юзер, который входит в систему
        /// </summary>
        public Client? Client
        {
            get => _client;
            set => SetProperty(ref _client, value);
        }

        /// <summary>
        /// Имя
        /// </summary>
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }
        /// <summary>
        /// Пароль 
        /// </summary>
        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        /// <summary>
        /// Клиент сервис
        /// </summary>
        private readonly ClientService _clientService;
        private readonly WindowsService _windowService;
        public ICommand LoginCommand { get; }
        public ICommand RegistrationCommand { get; }

        public EntranceViewModel(ClientService clientService, WindowsService windowService, SessionService sessionService)
        {

            _clientService = clientService;
            _windowService = windowService;
            _sessionService = sessionService;

            LoginCommand = new RelayCommandAsync(
                execute: () => TryRunTaskAsync(CheckAndAuthClient, "Ошибка аунтефикации"),
                canExecute: () => !IsBusy
            );
            RegistrationCommand = new RelayCommandAsync(
                execute: () => TryRunTaskAsync(OpenReg, "Ошибка регистрации"),
                canExecute: () => !IsBusy
            );
        }

        private async Task OpenReg() => _windowService.OpenRegistration();
        private async Task CheckAndAuthClient()
        {

            Client = await _clientService.GetClientByName(Name);
            _sessionService.CurrentClient = Client;

            if (Client == null)
            {
                ErrorMessage = "Юзер не найден";
                return;
            }
            Role = Client.Role;
            switch (Role)
            {
                case "admin":
                    _windowService.OpenMainWindow();
                    break;
                case "user":
                    _windowService.OpenMenu();
                    break;
            }
            CloseWindow(true);


        }
        /// <summary>
        /// Закрытие окна
        /// </summary>
        /// <param name="result">Результат работы окна</param>
        private void CloseWindow(bool result)
        {
            var window = Application.Current.Windows
                .OfType<Window>()
                .FirstOrDefault(w => w.DataContext == this);

            if (window != null) window.Close();

        }
    }
}
