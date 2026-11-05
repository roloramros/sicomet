using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace SiCoMet.Web.Data;

[Index(nameof(Numero), IsUnique = true)]
public class Tanque
{
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    [Display(Name = "Número")]
    public string Numero { get; set; } = string.Empty;

    [Display(Name = "Producto")]
    public int? ProductoId { get; set; }
    public Producto? Producto { get; set; }

    // Alturas y diámetro en metros
    [Display(Name = "Altura de referencia (m)")]
    public decimal? AlturaReferencia { get; set; }
    [Display(Name = "Altura operacional (m)")]
    public decimal? AlturaOperacional { get; set; }
    [Display(Name = "Altura de máximo llenado (m)")]
    public decimal? AlturaMaximoLlenado { get; set; }
    [Display(Name = "Altura de fondaje (m)")]
    public decimal? AlturaFondaje { get; set; }
    [Display(Name = "Altura de la platina de medición (m)")]
    public decimal? AlturaPlatinaMedicion { get; set; }
    [Display(Name = "Altura de la corona del cono del fondo (m)")]
    public decimal? AlturaCoronaConoFondo { get; set; }
    [Display(Name = "Altura total de la pared (m)")]
    public decimal? AlturaTotalPared { get; set; }
    [Display(Name = "Diámetro nominal (m)")]
    public decimal? DiametroNominal { get; set; }

    [Display(Name = "Tipo de techo")]
    public TipoTecho? TipoTecho { get; set; }
    [Display(Name = "Tipo de fondo")]
    public TipoFondo? TipoFondo { get; set; }

    [Display(Name = "Cantidad de rolos")]
    public int? CantidadRolos { get; set; }

    // En kilogramos. Solo aplica si TipoTecho == Flotante
    [Display(Name = "Masa del techo flotante (kg)")]
    public decimal? MasaTechoFlotante { get; set; }

    [Display(Name = "Factor de volumen")]
    public decimal? FactorVolumen { get; set; }

    [Display(Name = "Fecha de última calibración")]
    public DateOnly? FechaUltimaCalibracion { get; set; }

    [MaxLength(100)]
    [Display(Name = "Número del certificado de aforo")]
    public string? NumeroCertificadoAforo { get; set; }

    [MaxLength(200)]
    [Display(Name = "Entidad que realizó el aforo")]
    public string? EntidadAforo { get; set; }

    [Display(Name = "Vigencia del certificado")]
    public DateOnly? VigenciaCertificado { get; set; }

    // Estado derivado (no se guarda)
    public string EstadoCertificado =>
        string.IsNullOrWhiteSpace(NumeroCertificadoAforo) && VigenciaCertificado == null
            ? "Sin certificado"
            : VigenciaCertificado == null
                ? "Sin vigencia"
                : VigenciaCertificado.Value >= DateOnly.FromDateTime(DateTime.Today) ? "Vigente" : "Vencido";
}
