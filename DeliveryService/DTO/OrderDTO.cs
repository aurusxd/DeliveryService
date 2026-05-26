namespace DeliveryService.DTO
{
    /// <summary>
    /// Класс-DTO для отображения Заказа в OrderListView
    /// </summary>
    public class OrderDTO
    {
        public int Id { get; set; }
        public string ClientName { get; set; } = string.Empty;
        public string Route { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string OrderTime { get; set; } = string.Empty;
        public int? CourierId { get; set; }
    }
}