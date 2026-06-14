using DeliveryService.Models;
using DeliveryService.Repositories;

namespace DeliveryService.Services
{
    /// <summary>
    /// Сервис, работающий с Курьерами
    /// </summary>
    public class CourierService
    {
        private readonly OrderRepository _orderRepository;
        private readonly CourierRepository _courierRepository;


        public CourierService(OrderRepository orderRepo, CourierRepository courirerRepo)
        {
            _orderRepository = orderRepo;
            _courierRepository = courirerRepo;
        }


        /// <summary>
        /// Получение всех курьеров
        /// </summary>
        /// <returns>Список курьеров</returns>
        public async Task<List<Courier>> GetAllAsync()
        {
            return await _courierRepository.GetAllAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Получение всех активных курьеров
        /// </summary>
        /// <returns>Список активных курьеров</returns>
        public async Task<List<Courier>> GetActiveCouriersAsync()
        {
            return await _courierRepository.GetActive().ConfigureAwait(false);
        }

        /// <summary>
        /// Получение всех свободных от заказов курьеров
        /// </summary>
        /// <returns>Список свободных от заказов курьеров</returns>
        public async Task<List<Courier>> GetFreeCouriersAsync()
        {
            return await _courierRepository.GetFreeCouriers().ConfigureAwait(false);
        }

        /// <summary>
        /// Добавление курьера в базу данных
        /// </summary>
        /// <param name="courier">Курьер</param>
        /// <returns>Прошла ли операция</returns>
        public async Task<bool> AddCourierAsync(Courier courier)
        {
            if (courier == null)
                return false;

            courier.IsActive = true;
            courier.Created_At = DateTime.UtcNow;

            courier.Current_Lat = 0.0;
            courier.Current_Lon = 0.0;

            await _courierRepository.AddAsync(courier).ConfigureAwait(false);
            return true;
        }

        /// <summary>
        /// Назначение курьера на заказ
        /// </summary>
        /// <param name="courierId">ID курьера</param>
        /// <param name="orderId">ID заказа</param>
        /// <returns>Прошла ли операция назначения</returns>
        public async Task<bool> AssignCourierToOrderAsync(int courierId, int orderId)
        {
            Courier? courier = await _courierRepository.GetById(courierId).ConfigureAwait(false);
            if (courier == null)
                return false;

            Order? order = await _orderRepository.GetById(orderId).ConfigureAwait(false);
            if (order == null)
                return false;

            if (order.CourierId != null || order.CourierId == courierId)
                return false;

            order.CourierId = courierId;
            order.Status = "В пути"; // Заменить на нужный
            courier.Current_Lat = order.Lat_From;
            courier.Current_Lon = order.Lon_From;
            await _orderRepository.UpdateAsync(order).ConfigureAwait(false);

            return true;
        }

        /// <summary>
        /// Назначение курьера на заказ
        /// </summary>
        /// <param name="courierId">ID курьера</param>
        /// <param name="orderId">ID заказа</param>
        /// <returns>Прошла ли операция назначения</returns>
        public async Task<bool> AssignFreeCourierToOrderAsync(Order order)
        {
            if (order == null) return false;
            var list = await _courierRepository.GetFreeCouriers().ConfigureAwait(false);
            if (list.Count == 0)
            {
                order.CourierId = null;
                order.Status = "Новый";
                await _orderRepository.UpdateAsync(order).ConfigureAwait(false);
                return false;
            }
            Courier? courier = null;
            if (list.Count != 0) courier = list[new Random().Next(list.Count)];
            if (list == null) return false;
            if (courier == null) return false;
            if (order.CourierId != null) return false;


            order.CourierId = courier.Id;
            order.Status = "В пути";
            courier.Current_Lat = order.Lat_From;
            courier.Current_Lon = order.Lon_From;
            await _orderRepository.UpdateAsync(order).ConfigureAwait(false);

            return true;
        }

        /// <summary>
        /// Изменение статуса онлайн/офлайн курьера
        /// </summary>
        /// <param name="courierId">ID курьера</param>
        /// <returns>Прошла ли операция</returns>
        public async Task<bool> ToggleCourierOnlineAsync(int courierId)
        {
            if (await _courierRepository.GetById(courierId).ConfigureAwait(false) == null)
                return false;

            await _courierRepository.ToggleOnline(courierId).ConfigureAwait(false);
            return true;
        }
        /// <summary>
        /// Удаление курьера
        /// </summary>
        /// <param name="courier">Объект курьера</param>
        /// <returns></returns>
        public async Task<bool> RemoveCourierAsync(int courierId)
        {

            var courier = await _courierRepository.GetById(courierId).ConfigureAwait(false);

            if (courier == null) return false;
            _courierRepository?.DeleteAsync(courier);
            return true;
        }
        /// <summary>
        /// Получение курьера по айди
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Courier?> GetById(int id)
        {
            return await (_courierRepository?.GetById(id)).ConfigureAwait(false);
        }

        /// <summary>
        /// Обновление курьера
        /// </summary>
        /// <param name="courier"></param>
        /// <returns></returns>
        public async Task Update(Courier courier)
        {
            await _courierRepository.UpdateAsync(courier).ConfigureAwait(false);
        }

        /// <summary>
        /// Функция, сохраняющая координаты курьеры
        /// </summary>
        /// <param name="Lat"></param>
        /// <param name="Lat"></param>
        /// <param name="courier"></param>
        /// <returns></returns>
        public async Task SaveCourierCoords(double Lat, double Lon, Courier courier)
        {
            courier.Current_Lat = Lat;
            courier.Current_Lon = Lon;
            await Update(courier).ConfigureAwait(false);
        }
    }
}