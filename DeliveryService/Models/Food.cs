

namespace DeliveryService.Models
{
    public class Food
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public int Weight { get; set; }
        public int CategoriesId { get; set; }
        public decimal Price { get; set; }
        public Categories? Categories { get; set; }

        public ICollection<Basket> Baskets { get; set; } = new List<Basket>();
    }
}
