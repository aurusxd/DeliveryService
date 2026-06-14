using DeliveryService.Models;
using DeliveryService.Repositories;
using Microsoft.Extensions.Logging; 

namespace DeliveryService.Services
{
    public class OrderService
    {
        private readonly OrderRepository _orderRepository;
        private readonly ClientRepository _clientRepository;
        private readonly ILogger<OrderService> _logger; 

        public OrderService(OrderRepository orderRepo, ClientRepository clientRepo, ILogger<OrderService> logger)
        {
            _orderRepository = orderRepo;
            _clientRepository = clientRepo;
            _logger = logger;
        }

        public async Task<List<Order>> GetAllAsync() => await _orderRepository.GetAllAsync().ConfigureAwait(false);

        public async Task<List<Order>> GetActiveOrdersAsync() => await _orderRepository.GetActive().ConfigureAwait(false);

        public async Task<Order?> GetByIdAsync(int id) => await _orderRepository.GetById(id).ConfigureAwait(false);

        public async Task<bool> CreateOrderAsync(Client client, Order order)
        {
            _logger.LogInformation("Попытка создания заказа"); 
            if (client == null) return false;
            if (string.IsNullOrEmpty(order.Address_From) || string.IsNullOrEmpty(order.Address_To)) return false;

            if (client.Id == 0)
                await _clientRepository.AddAsync(client).ConfigureAwait(false);
            order.ClientId = client.Id;

            order.Status = "Новый";
            order.Created_At = DateTime.UtcNow;

            await _orderRepository.AddAsync(order).ConfigureAwait(false);
            _logger.LogInformation("Заказ {OrderId} успешно создан", order.Id); 
            return true;
        }

        public async Task<bool> ChangeStatusAsync(int orderId, string newStatus, string? feedback = null)
        {
            var order = await _orderRepository.GetById(orderId).ConfigureAwait(false);
            if (order == null) return false;

            order.Status = newStatus;
            await _orderRepository.UpdateAsync(order).ConfigureAwait(false);

            await _orderRepository.AddStatusHistoryAsync(new OrderStatusHistory
            {
                OrderId = orderId,
                Status = newStatus,
                Changed_At = DateTime.UtcNow,
                FeedBack = feedback
            }).ConfigureAwait(false);
            
            _logger.LogInformation("Статус заказа {OrderId} изменен на {Status}", orderId, newStatus);
            return true;
        }

        public async Task<bool?> CancelOrderAsync(int orderId, string? feedback = null)
        {
            var order = await _orderRepository.GetById(orderId).ConfigureAwait(false);
            if (order == null) return false;

            order.Status = "Отменён";
            await _orderRepository.UpdateAsync(order).ConfigureAwait(false);
            await _orderRepository.AddStatusHistoryAsync(new OrderStatusHistory
            {
                OrderId = orderId,
                Status = "Отменён",
                Changed_At = DateTime.UtcNow,
                FeedBack = feedback
            }).ConfigureAwait(false);
            
            _logger.LogWarning("Заказ {OrderId} отменен", orderId); 
            return true;
        }

        public async Task<Order?> FindOrderByCourierIdAsync(int courierId) => await _orderRepository.GetByCourierId(courierId).ConfigureAwait(false);

        public async Task<bool> RemoveOrderAsync(int orderId)
        {
            Order? order = await _orderRepository.GetById(orderId).ConfigureAwait(false);
            if (order == null) return false;
            await _orderRepository.DeleteAsync(order).ConfigureAwait(false);
            return true;
        }

        public async Task Update(Order order) => await _orderRepository.UpdateAsync(order).ConfigureAwait(false);

        public async Task<bool> DeleteAsync(Order order)
        {
            if (order == null) return false;
            await _orderRepository.DeleteAsync(order).ConfigureAwait(false);
            return true;
        }

        public async Task AddToHistory(Order order, string status, string? feedback = null)
        {
            if (order == null) return;
            await _orderRepository.AddStatusHistoryAsync(new OrderStatusHistory
            {
                Id = order.Id,
                Order = order,
                Changed_At = DateTime.UtcNow,
                FeedBack = feedback,
                Status = status,
                OrderId = order.Id
            }).ConfigureAwait(false);
        }
    }
}