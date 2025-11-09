namespace AutoPartesRazor.Models
{
    public class OrderItem
    {
        public int id { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }

        // navegación
        public Product? Product { get; set; }
        public Order? Order { get; set; }
    }
}