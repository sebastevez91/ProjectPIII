using System.ComponentModel.DataAnnotations;

namespace AutoPartesRazor.Models.Enum;

public enum ClaimStatus
{
    [Display(Name = "Pendiente")]
    Pending,

    [Display(Name = "En Proceso")]
    InProgress,

    [Display(Name = "Resuelto - Aceptado")]
    ResolvedAccepted,

    [Display(Name = "Resuelto - Rechazado")]
    ResolvedRejected,

    [Display(Name = "Cancelado")]
    Cancelled
}