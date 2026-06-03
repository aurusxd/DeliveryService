using DeliveryService.Data;
using DeliveryService.Models;
using Microsoft.EntityFrameworkCore;

namespace DeliveryService.Repositories
{
    /// <summary>
    /// Репозиторий для доступа к категориям еды в базе данных
    /// </summary>
    public class FoodCategoryRepository
    {
        private readonly AppDbContext _context;


        public FoodCategoryRepository(AppDbContext context)
        {
            _context = context;
        }


        /// <summary>
        /// Получение категории еды по id
        /// </summary>
        /// <param name="categoryId">ID категории</param>
        /// <returns>Категория еды с указаным id</returns>
        public async Task<Categories?> GetById(int categoryId) => await _context.Categories.FindAsync(categoryId).ConfigureAwait(false);
        /// <summary>
        /// Получение всех категорий еды
        /// </summary>
        /// <returns>Список категорий еды</returns>
        public async Task<List<Categories>> GetAllAsync() => await _context.Categories.ToListAsync().ConfigureAwait(false);
        /// <summary>
        /// Добавление категории еды
        /// </summary>
        /// <param name="categories">Категория</param>
        public async Task AddAsync(Categories categories)
        {
            await _context.Categories.AddAsync(categories).ConfigureAwait(false);
            await _context.SaveChangesAsync().ConfigureAwait(false);
        }
        /// <summary>
        /// Обновление категории еды в базе данных
        /// </summary>
        /// <param name="categories">Категория</param>
        /// <returns></returns>
        public async Task UpdateAsync(Categories categories)
        {
            _context.Update(categories);
            await _context.SaveChangesAsync().ConfigureAwait(false);
        }
        /// <summary>
        /// Удаление категории еды
        /// </summary>
        /// <param name="category">Категория</param>
        public async Task DeleteAsync(Categories category)
        {
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync().ConfigureAwait(false);
        }
    }
}