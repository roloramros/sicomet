using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace SiCoMet.Web.Data;

/// <summary>
/// Modelo de formulario del Tanque. Todos los campos opcionales son strings para que
/// un campo vacío nunca produzca error de enlace (en .NET 8 un número/fecha vacío en un
/// formulario estático da "The value '' is not valid"). Se convierten aquí, a mano.
/// </summary>
public class TanqueInput
{
    [Required(ErrorMessage = "El número del tanque es obligatorio.")]
    [MaxLength(50)]
    public string Numero { get; set; } = "";

    public string? ProductoId { get; set; }

    // Metros
    public string? DiametroNominal { get; set; }
    public string? AlturaReferencia { get; set; }
    public string? AlturaOperacional { get; set; }
    public string? AlturaMaximoLlenado { get; set; }
    public string? AlturaFondaje { get; set; }
    public string? AlturaPlatinaMedicion { get; set; }
    public string? AlturaCoronaConoFondo { get; set; }

    public string? TipoTecho { get; set; }
    public string? TipoFondo { get; set; }

    // Kilogramos
    public string? MasaTechoFlotante { get; set; }

    public string? CantidadRolos { get; set; }

    public string? FechaUltimaCalibracion { get; set; }

    [MaxLength(100)]
    public string? NumeroCertificadoAforo { get; set; }

    [MaxLength(200)]
    public string? EntidadAforo { get; set; }

    public string? VigenciaCertificado { get; set; }

    public static TanqueInput Desde(Tanque t) => new()
    {
        Numero = t.Numero,
        ProductoId = t.ProductoId?.ToString(CultureInfo.InvariantCulture),
        DiametroNominal = Fmt(t.DiametroNominal),
        AlturaReferencia = Fmt(t.AlturaReferencia),
        AlturaOperacional = Fmt(t.AlturaOperacional),
        AlturaMaximoLlenado = Fmt(t.AlturaMaximoLlenado),
        AlturaFondaje = Fmt(t.AlturaFondaje),
        AlturaPlatinaMedicion = Fmt(t.AlturaPlatinaMedicion),
        AlturaCoronaConoFondo = Fmt(t.AlturaCoronaConoFondo),
        TipoTecho = t.TipoTecho?.ToString(),
        TipoFondo = t.TipoFondo?.ToString(),
        MasaTechoFlotante = Fmt(t.MasaTechoFlotante),
        CantidadRolos = t.CantidadRolos?.ToString(CultureInfo.InvariantCulture),
        FechaUltimaCalibracion = t.FechaUltimaCalibracion?.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
        NumeroCertificadoAforo = t.NumeroCertificadoAforo,
        EntidadAforo = t.EntidadAforo,
        VigenciaCertificado = t.VigenciaCertificado?.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)
    };

    /// <summary>
    /// Copia los datos al tanque. Los campos vacíos quedan en null (nunca impiden guardar);
    /// solo falla si un campo trae un valor escrito que no es válido.
    /// </summary>
    public bool TryAplicar(Tanque t, out string? error)
    {
        var malos = new List<string>();

        decimal? Dec(string? s, string nombre, decimal max = 9999)
        {
            if (string.IsNullOrWhiteSpace(s)) return null;
            var limpio = s.Trim().Replace(',', '.');
            if (decimal.TryParse(limpio, NumberStyles.Number, CultureInfo.InvariantCulture, out var v) && v >= 0 && v <= max)
                return v;
            malos.Add(nombre);
            return null;
        }

        int? Ent(string? s, string nombre, int max)
        {
            if (string.IsNullOrWhiteSpace(s)) return null;
            if (int.TryParse(s.Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out var v) && v >= 0 && v <= max)
                return v;
            malos.Add(nombre);
            return null;
        }

        DateOnly? Fecha(string? s, string nombre)
        {
            if (string.IsNullOrWhiteSpace(s)) return null;
            if (DateOnly.TryParseExact(s.Trim(), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var v))
                return v;
            malos.Add(nombre);
            return null;
        }

        var diametro = Dec(DiametroNominal, "Diámetro nominal");
        var aRef = Dec(AlturaReferencia, "Altura de referencia");
        var aOper = Dec(AlturaOperacional, "Altura operacional");
        var aMax = Dec(AlturaMaximoLlenado, "Altura de máximo llenado");
        var aFondaje = Dec(AlturaFondaje, "Altura de fondaje");
        var aPlatina = Dec(AlturaPlatinaMedicion, "Altura de la platina de medición");
        var aCorona = Dec(AlturaCoronaConoFondo, "Altura de la corona del cono del fondo");
        var masa = Dec(MasaTechoFlotante, "Masa del techo flotante", 99999999);
        var rolos = Ent(CantidadRolos, "Cantidad de rolos", 100);
        var fCal = Fecha(FechaUltimaCalibracion, "Fecha de última calibración");
        var fVig = Fecha(VigenciaCertificado, "Vigencia del certificado");

        if (malos.Count > 0)
        {
            error = "Revisa estos campos (deben ser números positivos o quedar vacíos): " + string.Join(", ", malos) + ".";
            return false;
        }

        int? productoId = int.TryParse(ProductoId, NumberStyles.Integer, CultureInfo.InvariantCulture, out var pid) ? pid : null;
        global::SiCoMet.Web.Data.TipoTecho? techo =
            Enum.TryParse<global::SiCoMet.Web.Data.TipoTecho>(TipoTecho, out var tt) && Enum.IsDefined(tt) ? tt : null;
        global::SiCoMet.Web.Data.TipoFondo? fondo =
            Enum.TryParse<global::SiCoMet.Web.Data.TipoFondo>(TipoFondo, out var tf) && Enum.IsDefined(tf) ? tf : null;

        t.Numero = Numero.Trim();
        t.ProductoId = productoId;
        t.DiametroNominal = diametro;
        t.AlturaReferencia = aRef;
        t.AlturaOperacional = aOper;
        t.AlturaMaximoLlenado = aMax;
        t.AlturaFondaje = aFondaje;
        t.AlturaPlatinaMedicion = aPlatina;
        t.AlturaCoronaConoFondo = aCorona;
        t.TipoTecho = techo;
        t.TipoFondo = fondo;
        t.MasaTechoFlotante = techo == global::SiCoMet.Web.Data.TipoTecho.Flotante ? masa : null;
        t.CantidadRolos = rolos;
        t.FechaUltimaCalibracion = fCal;
        t.NumeroCertificadoAforo = string.IsNullOrWhiteSpace(NumeroCertificadoAforo) ? null : NumeroCertificadoAforo.Trim();
        t.EntidadAforo = string.IsNullOrWhiteSpace(EntidadAforo) ? null : EntidadAforo.Trim();
        t.VigenciaCertificado = fVig;

        error = null;
        return true;
    }

    private static string? Fmt(decimal? v) => v?.ToString("0.####", CultureInfo.InvariantCulture);
}
