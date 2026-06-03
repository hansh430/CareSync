namespace CareSync.Dtos
{
    public class UpdateMedicineDto
    {
        public string? Name { get; set; }

        public string? Manufacturer { get; set; }

        public decimal UnitPrice { get; set; }

        public decimal Discount { get; set; }

        public int Quantity { get; set; }

        public DateTime ExpDate { get; set; }
    }
}
