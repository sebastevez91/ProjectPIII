using System.ComponentModel.DataAnnotations;

namespace AutoPartesRazor.Models.Enum
{
    public enum LevelUrgency
    {
        [Display(Name = "Baja")]
        Baja = 1,

        [Display(Name = "Media")]
        Media = 2,

       [Display(Name = "Alta")]
        Alta = 3,

       [Display(Name = "Crítica")]
        Critica = 4
    }
}
