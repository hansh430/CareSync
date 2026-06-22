namespace CareSync.Dtos
{
    public class OrderDetailsDto
    {
        public int Id { get; set; }

        public string? OrderNo { get; set; }

        public string? UserName { get; set; }

        public decimal? OrderTotal { get; set; }

        public string? OrderStatus { get; set; }

        public List<OrderItemDto> Items { get; set; } = new();
    }
}
