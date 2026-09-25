using System.ComponentModel.DataAnnotations;

namespace SiCoMet.Web.Data;

public enum EstadoTecnico
{
    DentroDeLimites,
    NoApto
}

/// <summary>
/// Registro histórico de una calibración realizada sobre un instrumento.
/// Cada fila representa un evento de calibración puntual; el estado actual
/// de un instrumento se obtiene consultando su calibración más reciente.
/// </summary>
public class Calibracion
{
    public int Id { get; set; }

    public int InstrumentoId { get; set; }
    public Instrumento Instrumento { get; set; } = null!;

    public DateOnly FechaCalibracion { get; set; }
    public DateOnly FechaProximaCalibracion { get; set; }

    [MaxLength(100)]
    public string? CertificadoNumero { get; set; }

    [MaxLength(300)]
    public string? CertificadoArchivoUrl { get; set; }

    public EstadoTecnico EstadoTecnico { get; set; }

    /// <summary>
    /// Motivo de no conformidad. Solo debe tener valor cuando
    /// <see cref="EstadoTecnico"/> es <see cref="EstadoTecnico.NoApto"/>;
    /// esta regla se valida a nivel de servicio/formulario, no en la base de datos.
    /// </summary>
    [MaxLength(500)]
    public string? MotivoNoApto { get; set; }

    public string? CalibradorId { get; set; }
    public ApplicationUser? Calibrador { get; set; }

    [MaxLength(500)]
    public string? Observaciones { get; set; }
}
