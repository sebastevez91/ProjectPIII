namespace AutoPartesRazor.Models
{
    public class Order
    {
        public int id { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;
        public string ShippingAddress { get; set; } = string.Empty;
        public string PaymentMethod { get; set; } = string.Empty;
        public decimal Total { get; set; }
        public string Status { get; set; } = "Pending"; // Usaremos este campo para el ESTADO del despacho/tracking
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Soft Delete
        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; }

        // navegación
        public List<OrderItem> Items { get; set; } = new();
    }
}
