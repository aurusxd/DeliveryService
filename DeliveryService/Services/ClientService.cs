using DeliveryService.Models;
using DeliveryService.Repositories;

namespace DeliveryService.Services
{
    /// <summary>
    /// Репозиторий для доступа к Клиентам в базе данных
    /// </summary>
    public class ClientService
    {
        private readonly ClientRepository _clientRepository;


        public ClientService(ClientRepository clientRepository)
        {
            _clientRepository = clientRepository;
        }


        /// <summary>
        /// Получение клиента по id
        /// </summary>
        /// <param name="userId">ID клиента</param>
        /// <returns>Клиент. Если был не найден то null</returns>
        public async Task<Client?> GetClientById(int userId)
        {
            Client? client = await _clientRepository.GetById(userId);

            if (client == null)
                return null;
            return client;
        }
    }
}