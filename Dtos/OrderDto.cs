namespace CareSync.Dtos
{
    public class OrderDto
    {
        public int Id { get; set; }

        public string? OrderNo { get; set; }

        public decimal? OrderTotal { get; set; }

        public string? OrderStatus { get; set; }
    }
}
