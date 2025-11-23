using System.ComponentModel.DataAnnotations;

namespace AutoPartesRazor.Models.Enum;

public enum StockMovementType
{
    [Display(Name = "Entrada por Compra")]
    PurchaseEntry,

    [Display(Name = "Salida por Venta")]
    SaleExit,

    [Display(Name = "Ajuste Positivo")]
    AdjustmentIncrease,

    [Display(Name = "Ajuste Negativo")]
    AdjustmentDecrease,

    [Display(Name = "Devolución")]
    Return
}