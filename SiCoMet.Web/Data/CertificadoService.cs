using Microsoft.AspNetCore.Components.Forms;

namespace SiCoMet.Web.Data;

public class CertificadoInvalidoException : Exception
{
    public CertificadoInvalidoException(string mensaje) : base(mensaje) { }
}

public record CertificadoGuardado(string Ruta, string Nombre, long Tamano);

/// <summary>
/// Guarda y localiza los certificados de calibración (PDF) en una carpeta del servidor,
/// fuera de la parte pública del sitio. En la base de datos solo se guarda la referencia.
/// </summary>
public class CertificadoService
{
    public const long TamanoMaximo = 10 * 1024 * 1024; // 10 MB
    private const string RutaPorDefecto = "/var/lib/sicomet/certificados";

    private readonly string _raiz;

    public CertificadoService(IConfiguration configuration)
    {
        var configurada = configuration["Certificados:Ruta"];
        _raiz = Path.GetFullPath(string.IsNullOrWhiteSpace(configurada) ? RutaPorDefecto : configurada);
    }

    /// <summary>Valida lo que se puede saber sin leer el archivo (extensión y tamaño).</summary>
    public static string? ValidarSeleccion(IBrowserFile archivo)
    {
        if (!Path.GetExtension(archivo.Name).Equals(".pdf", StringComparison.OrdinalIgnoreCase))
            return "El certificado debe ser un archivo PDF.";
        if (archivo.Size == 0)
            return "El archivo está vacío.";
        if (archivo.Size > TamanoMaximo)
            return $"El archivo supera el tamaño máximo de {TamanoMaximo / (1024 * 1024)} MB.";
        return null;
    }

    public async Task<CertificadoGuardado> GuardarAsync(IBrowserFile archivo)
    {
        var error = ValidarSeleccion(archivo);
        if (error is not null) throw new CertificadoInvalidoException(error);

        var nombreInterno = Guid.NewGuid().ToString("N") + ".pdf";
        var destino = Path.Combine(_raiz, nombreInterno);
        var temporal = destino + ".tmp";

        try
        {
            Directory.CreateDirectory(_raiz);

            await using (var entrada = archivo.OpenReadStream(TamanoMaximo))
            await using (var salida = new FileStream(temporal, FileMode.CreateNew, FileAccess.Write, FileShare.None))
            {
                await entrada.CopyToAsync(salida);
            }

            // Comprobar el contenido real: un PDF siempre empieza con "%PDF-".
            var cabecera = new byte[5];
            await using (var lectura = new FileStream(temporal, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var leidos = await lectura.ReadAsync(cabecera);
                if (leidos < 5 || System.Text.Encoding.ASCII.GetString(cabecera) != "%PDF-")
                    throw new CertificadoInvalidoException("El archivo no es un PDF válido.");
            }

            File.Move(temporal, destino);
        }
        catch (CertificadoInvalidoException)
        {
            BorrarSiExiste(temporal);
            throw;
        }
        catch (IOException ex) when (ex is not FileNotFoundException)
        {
            BorrarSiExiste(temporal);
            throw new CertificadoInvalidoException("No se pudo guardar el archivo en el servidor. Avise al administrador.");
        }
        catch (UnauthorizedAccessException)
        {
            BorrarSiExiste(temporal);
            throw new CertificadoInvalidoException("El servidor no tiene permiso para guardar certificados. Avise al administrador.");
        }

        var nombre = Path.GetFileName(archivo.Name);
        if (nombre.Length > 255) nombre = nombre[..251] + ".pdf";

        return new CertificadoGuardado(nombreInterno, nombre, archivo.Size);
    }

    /// <summary>Ruta completa de un certificado; null si el nombre interno no es válido.</summary>
    public string? RutaCompleta(string ruta)
    {
        // El nombre interno nunca lleva carpetas: evita salirse de la carpeta de certificados.
        if (string.IsNullOrWhiteSpace(ruta) || Path.GetFileName(ruta) != ruta) return null;
        return Path.Combine(_raiz, ruta);
    }

    public void Eliminar(string ruta)
    {
        var completa = RutaCompleta(ruta);
        if (completa is not null) BorrarSiExiste(completa);
    }

    private static void BorrarSiExiste(string ruta)
    {
        try { if (File.Exists(ruta)) File.Delete(ruta); }
        catch (IOException) { /* limpieza best-effort */ }
        catch (UnauthorizedAccessException) { }
    }
}
