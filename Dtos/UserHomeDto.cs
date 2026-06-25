namespace CareSync.Dtos
{
    public class UserHomeDto
    {
        public int TotalMedicines { get; set; }

        public int CartItems { get; set; }

        public int TotalOrders { get; set; }

        public decimal WalletBalance { get; set; }

        public List<FeaturedMedicineDto> FeaturedMedicines { get; set; } = new();
    }
    public class FeaturedMedicineDto
    {
        public int Id { get; set; }

        public string? Name { get; set; }

        public decimal? UnitPrice { get; set; }

        public string? ImageUrl { get; set; }
    }
}
