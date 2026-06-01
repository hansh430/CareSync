namespace CareSync.Dtos
{
    public class AddMedicineDto
    {
        public string Name { get; set; } = string.Empty;

        public string Manufacturer { get; set; } = string.Empty;

        public decimal UnitPrice { get; set; }

        public decimal Discount { get; set; }

        public int Quantity { get; set; }

        public DateTime ExpDate { get; set; }

        public IFormFile? ImageFile { get; set; }
    }
}
