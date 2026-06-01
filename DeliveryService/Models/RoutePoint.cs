namespace DeliveryService.Models
{
    public class RoutePoint
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public double Lat { get; set; }
        public double Lon { get; set; }
        public DateTime Recorded_At { get; set; } = DateTime.UtcNow;

        public Order Order { get; set; } = null!;
    }
}
