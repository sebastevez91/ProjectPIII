namespace AutoPartesRazor.Models
{
    public class OrderItem
    {
        public int id { get; set; }
        public int OrderNumber { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }

        // Delete
        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; }
    }
}