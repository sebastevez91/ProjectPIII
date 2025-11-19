<<<<<<< HEAD
﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutoPartesRazor.Models;
=======
﻿namespace AutoPartesRazor.Models;
>>>>>>> c21700ccb191ba5352ac20e138b4c311c4f8d090

public class ProductSupplier
{
    public int ProductId { get; set; }
    public Product? Product { get; set; }

    public int SupplierId { get; set; }
    public Supplier? Supplier { get; set; }
<<<<<<< HEAD

    [Column(TypeName = "decimal(18, 2)")]
    [Display(Name = "Precio proveédor")]
    public decimal? SupplyPrice { get; set; }
}
=======
}
>>>>>>> c21700ccb191ba5352ac20e138b4c311c4f8d090
