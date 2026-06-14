using DeliveryService.Commands;
using DeliveryService.Models;
using DeliveryService.Services;
using Microsoft.Extensions.Logging; 
using System.Windows.Input;

namespace DeliveryService.ViewModels
{
    public class EntranceViewModel : BaseViewModel
    {
        private readonly SessionService _sessionService;
        private readonly ILogger<EntranceViewModel> _logger; 

        public event Action? CloseRequested;
        private string _name;
        private string _password;
        private Client? _client;
        private string _role;

        public string Role { get => _role; set => SetProperty(ref _role, value); }
        public Client? Client { get => _client; set => SetProperty(ref _client, value); }
        public string Name { get => _name; set => SetProperty(ref _name, value); }
        public string Password { get => _password; set => SetProperty(ref _password, value); }

        private readonly ClientService _clientService;
        private readonly WindowsService _windowService;
        public ICommand LoginCommand { get; }

        public EntranceViewModel(ClientService clientService, WindowsService windowService, SessionService sessionService, ILogger<EntranceViewModel> logger)
        {
            _clientService = clientService;
            _windowService = windowService;
            _sessionService = sessionService;
            _logger = logger; 

            LoginCommand = new RelayCommandAsync(
                execute: () => TryRunTaskAsync(CheckAndAuthClient, "Ошибка аутентификации"),
                canExecute: () => !IsBusy
            );
        }

        private async Task CheckAndAuthClient()
        {
            _logger.LogInformation("Попытка входа пользователя: {Name}", Name); 
            Client = await _clientService.GetClientByName(Name).ConfigureAwait(false);
            _sessionService.CurrentClient = Client;

            if (Client == null)
            {
                _logger.LogWarning("Пользователь {Name} не найден", Name); 
                ErrorMessage = "Такого пользователя не существует";
                return;
            }
            Role = Client.Role;
            switch (Role)
            {
                case "admin":
                    _windowService.OpenMainWindow();
                    CloseRequested?.Invoke();
                    break;
                case "user":
                    _windowService.OpenMenu();
                    CloseRequested?.Invoke();
                    break;
            }
            _logger.LogInformation("Вход выполнен успешно: {Name}", Name); 
        }
    }
}