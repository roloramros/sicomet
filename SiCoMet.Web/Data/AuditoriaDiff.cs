using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Reflection;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace SiCoMet.Web.Data;

/// <summary>Un campo que cambio en una edicion: nombre legible, valor anterior y valor nuevo.</summary>
public record CambioAuditoria(string Campo, string? Antes, string? Despues);

/// <summary>
/// Calcula que campos de una entidad cambiaron comparando, en el ChangeTracker de EF Core,
/// el valor original (el de la base de datos) con el actual. Debe llamarse DESPUES de aplicar
/// los cambios a la entidad y ANTES de SaveChangesAsync: al guardar, EF descarta los valores
/// originales y la comparacion saldria vacia.
///
/// El nombre legible de cada campo sale del atributo [Display(Name = "...")] de la entidad;
/// si no lo tiene, se usa el nombre de la propiedad.
/// </summary>
public static class AuditoriaDiff
{
    private static readonly JsonSerializerOptions OpcionesJson = new()
    {
        // Deja los acentos legibles en la base de datos. El texto solo se vuelve a leer
        // con este mismo codigo y se muestra con el escape normal de Blazor.
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };

    /// <param name="entrada">Entrada de la entidad en el DbContext (DbContext.Entry(entidad)).</param>
    /// <param name="formatear">
    /// Opcional. Recibe (nombre de propiedad, valor) y devuelve el texto a mostrar, o null para
    /// usar el formato por defecto. Sirve, por ejemplo, para mostrar el nombre de un producto
    /// en vez de su Id.
    /// </param>
    /// <param name="excluir">Nombres de propiedades que no deben registrarse (datos sensibles).</param>
    public static List<CambioAuditoria> Calcular(
        EntityEntry entrada,
        Func<string, object?, string?>? formatear = null,
        params string[] excluir)
    {
        entrada.Context.ChangeTracker.DetectChanges();

        var tipo = entrada.Metadata.ClrType;
        var cambios = new List<CambioAuditoria>();

        foreach (var propiedad in entrada.Properties)
        {
            var nombre = propiedad.Metadata.Name;

            if (propiedad.Metadata.IsPrimaryKey()) continue;
            if (excluir.Contains(nombre)) continue;
            if (SonIguales(propiedad.OriginalValue, propiedad.CurrentValue)) continue;

            cambios.Add(new CambioAuditoria(
                Etiqueta(tipo, nombre),
                Formatear(nombre, propiedad.OriginalValue, formatear),
                Formatear(nombre, propiedad.CurrentValue, formatear)));
        }

        return cambios;
    }

    /// <summary>Texto corto para el mensaje del registro: "1 campo modificado" / "3 campos modificados".</summary>
    public static string Resumen(int cantidad) =>
        cantidad == 1 ? "1 campo modificado" : $"{cantidad} campos modificados";

    // Un texto vacio, solo con espacios o null cuentan como lo mismo: un formulario puede
    // devolver "" donde la base de datos tiene null, y eso no es un cambio real.
    private static bool SonIguales(object? original, object? actual)
    {
        if (Equals(original, actual)) return true;

        if ((original is null or string) && (actual is null or string))
        {
            return string.Equals(((string?)original)?.Trim() ?? "", ((string?)actual)?.Trim() ?? "");
        }

        return false;
    }

    public static string? Serializar(IReadOnlyList<CambioAuditoria>? cambios) =>
        cambios is { Count: > 0 } ? JsonSerializer.Serialize(cambios, OpcionesJson) : null;

    public static List<CambioAuditoria> Deserializar(string? detalle)
    {
        if (string.IsNullOrWhiteSpace(detalle)) return new List<CambioAuditoria>();

        try
        {
            return JsonSerializer.Deserialize<List<CambioAuditoria>>(detalle, OpcionesJson)
                   ?? new List<CambioAuditoria>();
        }
        catch (JsonException)
        {
            return new List<CambioAuditoria>();
        }
    }

    private static string Etiqueta(Type tipo, string propiedad)
    {
        var nombre = tipo.GetProperty(propiedad)?.GetCustomAttribute<DisplayAttribute>()?.GetName();
        return string.IsNullOrWhiteSpace(nombre) ? propiedad : nombre;
    }

    private static string? Formatear(string propiedad, object? valor, Func<string, object?, string?>? formatear)
    {
        var propio = formatear?.Invoke(propiedad, valor);
        return propio ?? FormatearPorDefecto(valor);
    }

    private static string? FormatearPorDefecto(object? valor)
    {
        switch (valor)
        {
            case null:
                return null;
            case string texto:
                return string.IsNullOrWhiteSpace(texto) ? null : texto.Trim();
            case decimal d:
                return d.ToString("0.######", CultureInfo.InvariantCulture).Replace('.', ',');
            case double db:
                return db.ToString("0.######", CultureInfo.InvariantCulture).Replace('.', ',');
            case DateOnly fecha:
                return fecha.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
            case DateTime fechaHora:
                return fechaHora.ToString("dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture);
            case bool b:
                return b ? "Sí" : "No";
            case Enum e:
                return EnumLegible(e);
            default:
                return Convert.ToString(valor, CultureInfo.InvariantCulture);
        }
    }

    // CoronaArriba -> "Corona arriba"
    private static string EnumLegible(Enum valor)
    {
        var texto = Regex.Replace(valor.ToString(), "(?<=[a-z])(?=[A-Z])", " ");
        return char.ToUpperInvariant(texto[0]) + texto[1..].ToLowerInvariant();
    }
}
