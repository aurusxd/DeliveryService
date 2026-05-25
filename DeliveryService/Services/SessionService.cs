using DeliveryService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace DeliveryService.Services
{
    /// <summary>
    /// Класc-сервис для хранения текущего пользователя
    /// </summary>
    public class SessionService
    {
        /// <summary>
        /// Текущий пользователь
        /// </summary>
        private Client? _currentClient;

        /// <summary>
        /// Текущий пользователь
        /// </summary>
        public Client? CurrentClient
        {
            get => _currentClient;
            set
            {
                if (_currentClient?.Id != value?.Id)
                {
                    _currentClient = value;
                    CurrentUserChanged?.Invoke();
                }
            }
        }

        /// <summary>
        /// Ивент при изменении текущего пользователя (пока что нигде не используется)
        /// </summary>
        public event Action? CurrentUserChanged; 
    }
}