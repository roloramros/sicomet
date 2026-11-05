using System.ComponentModel.DataAnnotations;

namespace SiCoMet.Web.Data;

public class TipoInstrumento
{
    public int Id { get; set; }

    [Required]
    [MaxLength(150)]
    [Display(Name = "Nombre")]
    public string Nombre { get; set; } = "";

    [MaxLength(50)]
    [Display(Name = "Unidad de medida")]
    public string? UnidadMedida { get; set; }

    public ICollection<Instrumento> Instrumentos { get; set; } = new List<Instrumento>();
}
