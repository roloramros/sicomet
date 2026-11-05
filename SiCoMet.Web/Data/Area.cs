using System.ComponentModel.DataAnnotations;

namespace SiCoMet.Web.Data;

public class Area
{
    public int Id { get; set; }

    [Required]
    [MaxLength(150)]
    [Display(Name = "Nombre")]
    public string Nombre { get; set; } = "";

    [MaxLength(500)]
    [Display(Name = "Descripción")]
    public string? Descripcion { get; set; }

    public ICollection<Instrumento> Instrumentos { get; set; } = new List<Instrumento>();
}
