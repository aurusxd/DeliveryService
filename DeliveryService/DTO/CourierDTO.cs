namespace DeliveryService.DTO
{
    /// <summary>
    /// Класс-DTO для отображения Курьера в OrderListView
    /// </summary>
    public class CourierDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}