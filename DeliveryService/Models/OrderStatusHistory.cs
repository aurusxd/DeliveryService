namespace DeliveryService.Models
{
    public class OrderStatusHistory
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime Changed_At { get; set; } = DateTime.UtcNow;
        public string? FeedBack { get; set; }

        public Order Order { get; set; } = null!;
    }
}
