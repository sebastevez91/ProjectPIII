namespace AutoPartesRazor.Models
{
    public class Brand
    {
        public int id {  get; set; }
        public string name { get; set; }

        // Navegación
        public List<Product>? products { get; set; }
    }
}
