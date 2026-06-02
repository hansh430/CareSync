namespace CareSync.Dtos
{
    public class OrderItemDto
    {
        public int MedicineId { get; set; }

        public string? MedicineName { get; set; }

        public string? ImageUrl { get; set; }

        public decimal? UnitPrice { get; set; }

        public decimal? Discount { get; set; }

        public int? Quantity { get; set; }

        public decimal? TotalPrice { get; set; }
    }
}
