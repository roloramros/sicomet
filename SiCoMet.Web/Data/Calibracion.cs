using System.ComponentModel.DataAnnotations;

namespace SiCoMet.Web.Data;

public enum EstadoTecnico
{
    DentroDeLimites,
    NoApto
}

/// <summary>
/// Ciclo completo de una calibración de un instrumento: se abre al enviarlo al
/// taller de calibración (<see cref="FechaEnvioTaller"/>) y se completa al registrar
/// el resultado (fecha de calibración, próxima fecha y estado técnico).
/// Mientras el resultado no se registra, esos campos son nulos. El estado actual del
/// instrumento no se guarda: se deriva de su calibración más reciente
/// (ver <see cref="CalibracionEstadoHelper"/>).
/// </summary>
public class Calibracion
{
    public int Id { get; set; }

    public int InstrumentoId { get; set; }
    public Instrumento Instrumento { get; set; } = null!;

    /// <summary>Fecha en que el instrumento se envió al taller de calibración.</summary>
    public DateOnly FechaEnvioTaller { get; set; }

    /// <summary>Fecha en que se realizó la calibración. Nula hasta registrar el resultado.</summary>
    public DateOnly? FechaCalibracion { get; set; }

    /// <summary>Fecha de vencimiento de esta calibración. Nula hasta registrar el resultado.</summary>
    public DateOnly? FechaProximaCalibracion { get; set; }

    [MaxLength(100)]
    public string? CertificadoNumero { get; set; }

    /// <summary>Nombre original del PDF del certificado, tal como lo subió el usuario.</summary>
    [MaxLength(255)]
    public string? CertificadoArchivoNombre { get; set; }

    /// <summary>
    /// Nombre interno del archivo en la carpeta de certificados (aleatorio, sin ruta).
    /// Nulo si no se adjuntó el PDF.
    /// </summary>
    [MaxLength(100)]
    public string? CertificadoArchivoRuta { get; set; }

    /// <summary>Tamaño del PDF en bytes.</summary>
    public long? CertificadoArchivoTamano { get; set; }

    /// <summary>Fecha y hora (UTC) en que se adjuntó el PDF.</summary>
    public DateTime? CertificadoArchivoFecha { get; set; }

    /// <summary>Resultado de la calibración. Nulo hasta registrar el resultado.</summary>
    public EstadoTecnico? EstadoTecnico { get; set; }

    /// <summary>
    /// Motivo de no conformidad. Solo debe tener valor cuando
    /// <see cref="EstadoTecnico"/> es <see cref="Data.EstadoTecnico.NoApto"/>;
    /// esta regla se valida a nivel de servicio/formulario, no en la base de datos.
    /// </summary>
    [MaxLength(500)]
    public string? MotivoNoApto { get; set; }

    public string? CalibradorId { get; set; }
    public ApplicationUser? Calibrador { get; set; }

    [MaxLength(500)]
    public string? Observaciones { get; set; }
}
