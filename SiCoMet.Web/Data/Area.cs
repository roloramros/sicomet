using System.ComponentModel.DataAnnotations;

namespace SiCoMet.Web.Data;

public class Area
{
    public int Id { get; set; }

    [Required]
    [MaxLength(150)]
    public string Nombre { get; set; } = "";

    [MaxLength(500)]
    public string? Descripcion { get; set; }

    public ICollection<Instrumento> Instrumentos { get; set; } = new List<Instrumento>();
}
