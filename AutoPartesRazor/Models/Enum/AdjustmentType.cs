using System.ComponentModel.DataAnnotations;

namespace AutoPartesRazor.Models.Enum;

public enum AdjustmentType
{
    [Display(Name = "Diferencia en Recepción")]
    ReceptionDiscrepancy,

    [Display(Name = "Inventario Físico")]
    PhysicalInventory,

    [Display(Name = "Producto Dañado")]
    DamagedProduct,

    [Display(Name = "Producto Perdido")]
    LostProduct,

    [Display(Name = "Error de Sistema")]
    SystemError,

    [Display(Name = "Otro")]
    Other
}
