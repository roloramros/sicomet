using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace SiCoMet.Web.Data;

[Index(nameof(Numero), IsUnique = true)]
public class Tanque
{
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string Numero { get; set; } = string.Empty;

    public int? ProductoId { get; set; }
    public Producto? Producto { get; set; }

    // Alturas y diámetro en metros
    public decimal? AlturaReferencia { get; set; }
    public decimal? AlturaOperacional { get; set; }
    public decimal? AlturaMaximoLlenado { get; set; }
    public decimal? AlturaFondaje { get; set; }
    public decimal? AlturaPlatinaMedicion { get; set; }
    public decimal? AlturaCoronaConoFondo { get; set; }
    public decimal? DiametroNominal { get; set; }

    public TipoTecho? TipoTecho { get; set; }
    public int? CantidadRolos { get; set; }

    // Solo aplica si TipoTecho == Flotante
    public decimal? MasaTechoFlotante { get; set; }

    public DateOnly? FechaUltimaCalibracion { get; set; }

    [MaxLength(100)]
    public string? NumeroCertificadoAforo { get; set; }

    [MaxLength(200)]
    public string? EntidadAforo { get; set; }

    public DateOnly? VigenciaCertificado { get; set; }

    // Estado derivado (no se guarda)
    public string EstadoCertificado =>
        string.IsNullOrWhiteSpace(NumeroCertificadoAforo) && VigenciaCertificado == null
            ? "Sin certificado"
            : VigenciaCertificado == null
                ? "Sin vigencia"
                : VigenciaCertificado.Value >= DateOnly.FromDateTime(DateTime.Today) ? "Vigente" : "Vencido";
}
