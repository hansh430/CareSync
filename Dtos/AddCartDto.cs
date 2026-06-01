namespace CareSync.Dtos
{
    public class AddCartDto
    {
        public int UserId { get; set; }
        public int MedicineId { get; set; }
        public int Quantity { get; set; }
    }
}
