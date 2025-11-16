using System.ComponentModel.DataAnnotations;

namespace AutoPartesRazor.Models.Enum
{
    public enum StatusClaim
    {
        [Display(Name = "Nuevo")]
        Nuevo = 1,

        [Display(Name = "En Proceso")]
        EnProceso = 2,

        [Display(Name = "Respondido")]
        Respondido = 3,

        [Display(Name = "Resuelto")]
        Resuelto = 4,

        [Display(Name = "Cerrado")]
        Cerrado = 5
    }
}
