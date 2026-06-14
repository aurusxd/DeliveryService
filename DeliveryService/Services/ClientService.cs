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
        /// Добавление нового клиента в БД
        /// </summary>
        /// <param name="client">Объект клиента</param>
        /// <returns></returns>
        public async Task<bool> AddClientAsync(Client client)
        {
            if (client == null) return false;
            await _clientRepository.AddAsync(client).ConfigureAwait(false);
            return true;
        }

        /// <summary>
        /// Получение клиента по id
        /// </summary>
        /// <param name="userId">ID клиента</param>
        /// <returns>Клиент. Если был не найден то null</returns>
        public async Task<Client?> GetClientById(int userId)
        {
            Client? client = await _clientRepository.GetById(userId).ConfigureAwait(false);

            if (client == null)
                return null;
            return client;
        }
        /// <summary>
        /// Получение клиента по Name
        /// </summary>
        /// <param name="name">Логин клиента</param>
        /// <returns>Клиент. Если был не найден то null</returns>
        public async Task<Client?> GetClientByName(string name)
        {
            Client? client = await _clientRepository.GetByName(name).ConfigureAwait(false);

            if (client == null)
                return null;
            return client;
        }
    }
}