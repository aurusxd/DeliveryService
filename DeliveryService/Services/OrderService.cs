using DeliveryService.Models;
using DeliveryService.Repositories;

namespace DeliveryService.Services
{
    /// <summary>
    /// Сервис, работающий с заказами
    /// </summary>
    public class OrderService
    {
        private readonly OrderRepository _orderRepository;
        private readonly ClientRepository _clientRepository;

        public OrderService(OrderRepository orderRepo, ClientRepository clientRepo)
        {
            _orderRepository = orderRepo;
            _clientRepository = clientRepo;
        }
        /// <summary>
        /// Получение всех заказов
        /// </summary>
        /// <returns>Возвращает список всех заказов</returns>
        public async Task<List<Order>> GetAllAsync() => await _orderRepository.GetAllAsync();

        /// <summary>
        /// Получение всех активных заказов
        /// </summary>
        /// <returns>Список активных заказов</returns>
        public async Task<List<Order>> GetActiveOrdersAsync() => await _orderRepository.GetActive();

        public async Task<Order?> GetByIdAsync(int id) => await _orderRepository.GetById(id);

        public async Task<bool> CreateOrderAsync(Client client, Order order)
        {
            if (client == null) return false;
            if (string.IsNullOrEmpty(order.Address_From) || string.IsNullOrEmpty(order.Address_To)) return false;

            if (client.Id == 0)
                await _clientRepository.AddAsync(client);
            order.ClientId = client.Id;

            order.Status = "new";
            order.Created_At = DateTime.UtcNow;

            await _orderRepository.AddAsync(order);
            return true;
        }

        /// <summary>
        /// Меняет статус заказа
        /// </summary>
        /// <param name="orderId">Айди заказа</param>
        /// <param name="newStatus">Новый статус</param>
        /// <param name="feedback">Отзыв, при закрытии заказа</param>
        /// <returns></returns>
        public async Task<bool> ChangeStatusAsync(int orderId, string newStatus, string? feedback = null)
        {
            var order = await _orderRepository.GetById(orderId);
            if (order == null) return false;

            order.Status = newStatus;
            await _orderRepository.UpdateAsync(order);

            await _orderRepository.AddStatusHistoryAsync(new OrderStatusHistory
            {
                OrderId = orderId,
                Status = newStatus,
                Changed_At = DateTime.UtcNow,
                FeedBack = feedback
            });
            return true;
        }
        /// <summary>
        /// Отменяет заказ
        /// </summary>
        /// <param name="orderId">Айди заказа</param>
        /// <param name="feedback">Отзыв, при желании</param>
        /// <returns>true-если заказ отменен,false-если заказ не найден</returns>
        public async Task<bool?> CancelOrderAsync(int orderId, string? feedback = null)
        {
            var order = await _orderRepository.GetById(orderId);
            if (order == null) return false;


            order.Status = "Cancelled";
            await _orderRepository.UpdateAsync(order);
            await _orderRepository.AddStatusHistoryAsync(new OrderStatusHistory
            {
                OrderId = orderId,
                Status = "Cancelled",
                Changed_At = DateTime.UtcNow,
                FeedBack = feedback
            });
            return true;
        }

        /// <summary>
        /// Получеие заказа по айди курьера
        /// </summary>
        /// <param name="courierId">айди курьера</param>
        /// <returns></returns>
        public async Task<Order?> FindOrderByCourierIdAsync(int courierId) => await _orderRepository.GetByCourierId(courierId);    

    }
}
