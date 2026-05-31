 using DeliveryService.Models;
using DeliveryService.Repositories;

namespace DeliveryService.Services
{
    /// <summary>
    /// Сервис, работающий с Корзиной
    /// </summary>
    public class BasketService
    {
        private readonly BasketRepository _basketRepository;
        private readonly OrderRepository _orderRepository;
        private readonly FoodRepository _foodRepository;


        public BasketService(BasketRepository basketRepository, OrderRepository orderRepository, FoodRepository foodRepository)
        {
            _basketRepository = basketRepository;
            _orderRepository = orderRepository;
            _foodRepository = foodRepository;
        }


        /// <summary>
        /// Получение объекта корзины по id
        /// </summary>
        /// <param name="basketId">ID объекта корзины</param>
        /// <returns>Объект корзины</returns>
        public async Task<Basket?> GetById(int basketId) => await _basketRepository.GetById(basketId);
        /// <summary>
        /// Полуение списка корзины и полной суммы по пользователю
        /// </summary>
        /// <param name="userId">ID пользователя</param>
        /// <returns>Список корзины и полная сумма</returns>
        public async Task<(List<Basket> userBasket, decimal totalPrice)> GetUserBasketAsync(int userId)
        {
            var basket = await _basketRepository.GetUserBasketAsync(userId);
            decimal totalPrice = basket.Sum(b => b.Price);
            
            return (basket, totalPrice);
        }
        /// <summary>
        /// Полуение списка корзины, исключая те объекты, которые уже привязаны к заказам и полной суммы по пользователю
        /// </summary>
        /// <param name="userId">ID пользователя</param>
        /// <returns>Список объектов корзины, c не привязанными к заказам и полная сумма</returns>
        public async Task<(List<Basket> userBasket, decimal totalPrice)> GetUserActiveBasketAsync(int userId)
        {
            var basket = await _basketRepository.GetUserActiveBasketAsync(userId);
            decimal totalPrice = basket.Sum(b => b.Price);

            return (basket, totalPrice);
        }
        /// <summary>
        /// Создание нового объекта корзины
        /// </summary>
        /// <param name="userId">ID пользователя</param>
        /// <param name="foodId">ID еды</param>
        /// <param name="quantity">Количество</param>
        /// <returns>Прошла ли операция</returns>
        public async Task<bool> AddNewBasketItemAsync(int userId, int foodId, int quantity)
        {
            var food = await _foodRepository.GetById(foodId);
            if (food == null)
                return false;

            decimal price = food.Price * quantity;
            Basket item = new Basket
            {
                UserId = userId,
                FoodId = foodId,
                Quantity = quantity,
                Price = price
            };

            await _basketRepository.AddAsync(item);

            return true;
        }
        /// <summary>
        /// Создание нового объекта корзины или обновление уже существующего
        /// </summary>
        /// <param name="userId">ID пользователя</param>
        /// <param name="foodId">ID еды</param>
        /// <param name="quantity">Количество</param>
        /// <returns>Прошла ли операция</returns>
        public async Task<bool> AddOrUpdateBasketItemAsync(int userId, int foodId, int quantity)
        {
            var food = await _foodRepository.GetById(foodId);
            if (food == null)
                return false;

            var item = await _basketRepository.GetActiveByUserAndFoodId(userId, foodId);
            if (item != null)
            {
                item.Quantity += quantity;
                item.Price = food.Price * item.Quantity;
                await _basketRepository.UpdateAsync(item);
            }
            else
            {
                decimal price = food.Price * quantity;
                Basket newItem = new Basket 
                { 
                    UserId = userId,
                    FoodId = foodId,
                    Quantity = quantity,
                    Price = price
                };

                await _basketRepository.AddAsync(newItem);
            }

            return true;
        }
        /// <summary>
        /// Удаление объекта из корзины
        /// </summary>
        /// <param name="basketId">ID объекта корзины</param>
        /// <returns>Прошла ли операция</returns>
        public async Task<bool> RemoveItemAsync(int basketId)
        {
            var item = await _basketRepository.GetById(basketId);
            if (item == null)
                return false;

            await _basketRepository.DeleteAsync(item);
            return true;
        }
        /// <summary>
        /// Очистка всей всех объектов корзины по пользователю
        /// </summary>
        /// <param name="userId">ID пользователя</param>
        public async Task ClearUserBasketAsync(int userId)
        {
            await _basketRepository.ClearUserBasketAsync(userId);
        }
    }
}