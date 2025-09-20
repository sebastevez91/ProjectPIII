namespace AutoPartesRazor.Models
{
    public class Product
    {
        public int id {  get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public int stock { get; set; }
        public decimal price { get; set; }

        // Clave Foreign Key
        public int idCategory { get; set; }

        // Navegación 
        public Category? Category { get; set; }

        // Clave Foreign Key
        public int idBrand { get; set; }

        // Navegación 
        public Brand? Brand { get; set; }
    }
}
