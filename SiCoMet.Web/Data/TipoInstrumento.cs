using System.ComponentModel.DataAnnotations;

namespace SiCoMet.Web.Data;

public class TipoInstrumento
{
    public int Id { get; set; }

    [Required]
    [MaxLength(150)]
    public string Nombre { get; set; } = "";

    [MaxLength(50)]
    public string? UnidadMedida { get; set; }

    public ICollection<Instrumento> Instrumentos { get; set; } = new List<Instrumento>();
}
