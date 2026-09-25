using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SiCoMet.Web.Data;

[Index(nameof(Serie), IsUnique = true)]
public class Instrumento
{
    public int Id { get; set; }

    public int AreaId { get; set; }
    public Area Area { get; set; } = null!;

    public int TipoInstrumentoId { get; set; }
    public TipoInstrumento TipoInstrumento { get; set; } = null!;

    [Required]
    [MaxLength(100)]
    public string Serie { get; set; } = "";

    [MaxLength(100)]
    public string? Posicion { get; set; }

    [Column(TypeName = "decimal(18,4)")]
    public decimal? RangoMedicionMin { get; set; }

    [Column(TypeName = "decimal(18,4)")]
    public decimal? RangoMedicionMax { get; set; }

    /// <summary>
    /// Periodicidad de calibración en meses, definida manualmente por instrumento.
    /// </summary>
    public int PeriodicidadMeses { get; set; }

    public bool Activo { get; set; } = true;

    public ICollection<Calibracion> Calibraciones { get; set; } = new List<Calibracion>();
}
