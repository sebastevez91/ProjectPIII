namespace AutoPartesRazor.Models
{
    public class Cart
    {
        public int id {  get; set; }

        public int productId { get; set; }

        public int quantity { get; set; }

        // Navegación
        public Product? producto { get; set; }
    }
}
