using DeliveryService.Data;
using DeliveryService.Models;
using Microsoft.EntityFrameworkCore;

namespace DeliveryService.Repositories
{
    /// <summary>
    /// Репозиторий для доступа к еде в базе данных
    /// </summary>
    public class FoodRepository
    {
        private readonly AppDbContext _context;


        public FoodRepository(AppDbContext context)
        {
            _context = context;
        }


        /// <summary>
        /// Получение еды по id
        /// </summary>
        /// <param name="foodId">ID еды</param>
        /// <returns>Еда с указаным id</returns>
        public async Task<Food?> GetById(int foodId) => await _context.Foods.FindAsync(foodId).ConfigureAwait(false);
        /// <summary>
        /// Получение всей еды
        /// </summary>
        /// <returns>Список еды</returns>
        public async Task<List<Food>> GetAllAsync()
        {
            return await _context.Foods
                .Include(f => f.Categories)
                .ToListAsync().ConfigureAwait(false);
        }
        /// <summary>
        /// Получение еды по категории
        /// </summary>
        /// <param name="categoryId">ID категории</param>
        /// <returns>Список еды с указанной категорией</returns>
        public async Task<List<Food>> GetAllFromCategoryAsync(int categoryId)
        {
            return await _context.Foods
                .Include(f => f.Categories)
                .Where(f => f.CategoriesId == categoryId)
                .ToListAsync().ConfigureAwait(false);
        }
        /// <summary>
        /// Добавление еды
        /// </summary>
        /// <param name="food">Объект еды</param>
        public async Task AddAsync(Food food)
        {
            await _context.Foods.AddAsync(food).ConfigureAwait(false);
            await _context.SaveChangesAsync().ConfigureAwait(false);
        }
        /// <summary>
        /// Обновление еды в базе данных
        /// </summary>
        /// <param name="food">Объект еды</param>
        public async Task UpdateAsync(Food food)
        {
            _context.Foods.Update(food);
            await _context.SaveChangesAsync().ConfigureAwait(false);
        }
        /// <summary>
        /// Удаление еды
        /// </summary>
        /// <param name="food">Объект еды</param>
        public async Task DeleteAsync(Food food)
        {
            _context.Foods.Remove(food);
            await _context.SaveChangesAsync().ConfigureAwait(false);
        }
    }
}