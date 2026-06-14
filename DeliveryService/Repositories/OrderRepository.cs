using DeliveryService.Data;
using DeliveryService.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging; 

namespace DeliveryService.Repositories
{
    public class OrderRepository
    {
        private readonly AppDbContext _context;
        private readonly ILogger<OrderRepository> _logger; 

        public OrderRepository(AppDbContext context, ILogger<OrderRepository> logger)
        {
            _context = context;
            _logger = logger; 
        }

        public async Task<Order?> GetById(int orderId) => await _context.Orders.FindAsync(orderId).ConfigureAwait(false);

        public async Task<Order?> GetByCourierId(int courierId) => await _context.Orders.FirstOrDefaultAsync(x => x.CourierId == courierId).ConfigureAwait(false);

        public async Task<List<Order>> GetAllAsync()
        {
            return await _context.Orders
                .Include(o => o.Client)
                .Include(o => o.Courier)
                .Include(o => o.RoutePoints)
                .Include(o => o.StatusHistory)
                .ToListAsync().ConfigureAwait(false);
        }

        public async Task<List<Order>> GetActive()
        {
            return await _context.Orders
                .Where(o => o.Status != "Done")
                .Include(o => o.Client)
                .Include(o => o.Courier)
                .Include(o => o.RoutePoints)
                .Include(o => o.StatusHistory)
                .ToListAsync().ConfigureAwait(false);
        }

        public async Task AddAsync(Order order)
        {
            try {
                await _context.Orders.AddAsync(order).ConfigureAwait(false);
                await _context.SaveChangesAsync().ConfigureAwait(false);
            } catch (Exception ex) {
                _logger.LogError(ex, "Ошибка БД при добавлении заказа"); 
                throw;
            }
        }

        public async Task UpdateAsync(Order order)
        {
            _context.Orders.Update(order);
            await _context.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task DeleteAsync(Order order)
        {
            _context.Orders.Remove(order);
            await _context.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task AddStatusHistoryAsync(OrderStatusHistory history)
        {
            await _context.OrderStatusHistories.AddAsync(history).ConfigureAwait(false);
            await _context.SaveChangesAsync().ConfigureAwait(false);
        }
    }
}