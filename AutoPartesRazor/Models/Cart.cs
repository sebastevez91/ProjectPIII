<<<<<<< HEAD
﻿using System.ComponentModel.DataAnnotations;

namespace AutoPartesRazor.Models;

public class Cart
{

    public int Id { get; set; }

    // Foreign Key - Product
    public int ProductId { get; set; }

    [Display(Name = "Cantidad")]
    public int Quantity { get; set; }

    [Display(Name = "Fecha de creación")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    [Display(Name = "Fecha de actualización")]
    public DateTime? UpdatedAt { get; set; }

    // Navigation Properties
    public Product? Product { get; set; }
}
=======
﻿namespace AutoPartesRazor.Models
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
>>>>>>> c21700ccb191ba5352ac20e138b4c311c4f8d090
