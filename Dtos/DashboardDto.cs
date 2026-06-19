namespace CareSync.Dtos
{
    public class DashboardDto
    {
        public int TotalCustomers { get; set; }

        public int TotalMedicines { get; set; }

        public int TotalOrders { get; set; }

        public decimal TotalRevenue { get; set; }

        public int PendingOrders { get; set; }
    }
}
