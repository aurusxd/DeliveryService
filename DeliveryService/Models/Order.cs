
namespace DeliveryService.Models
{

    public class Order
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public int? CourierId { get; set; }
        public int? BasketId { get; set; }
        public string Address_From { get; set; } = string.Empty;
        public double Lat_From { get; set; }
        public double Lon_From { get; set; }
        public string Address_To { get; set; } = string.Empty;
        public double Lat_To { get; set; }
        public double Lon_To { get; set; }
        public string? Status { get; set; }
        public decimal Price { get; set; }
        public DateTime Created_At { get; set; } = DateTime.UtcNow;
        public DateTime? Delivered_At { get; set; }

        public Client Client { get; set; } = null!;
        public Courier? Courier { get; set; }
        public Basket? Basket { get; set; }
        public ICollection<RoutePoint> RoutePoints { get; set; } = new List<RoutePoint>();
        public ICollection<OrderStatusHistory> StatusHistory { get; set; } = new List<OrderStatusHistory>();
    }
}
