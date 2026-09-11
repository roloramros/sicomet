using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace SiCoMet.Web.Data;

public interface IAuditoriaService
{
    Task RegistrarAsync(string entidad, TipoAccionAuditoria accion, string mensaje);
}

// Servicio simple para dejar constancia de acciones (crear/editar/eliminar) en cualquier
// modulo del sistema. Se llama explicitamente desde cada pantalla en el punto donde
// la accion se confirma, para evitar registrar cambios internos de Identity (ej. intentos
// de login fallidos) que no son acciones reales del usuario sobre los datos.
public class AuditoriaService(ApplicationDbContext db, IHttpContextAccessor httpContextAccessor) : IAuditoriaService
{
    public async Task RegistrarAsync(string entidad, TipoAccionAuditoria accion, string mensaje)
    {
        var user = httpContextAccessor.HttpContext?.User;

        var usuarioId = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var nombre = user?.FindFirst("NombreCompleto")?.Value;
        if (string.IsNullOrWhiteSpace(nombre))
        {
            nombre = user?.Identity?.Name ?? "Sistema";
        }

        db.RegistrosAuditoria.Add(new RegistroAuditoria
        {
            UsuarioId = usuarioId,
            NombreUsuario = nombre,
            Entidad = entidad,
            Accion = accion,
            Mensaje = mensaje
        });

        await db.SaveChangesAsync();
    }
}
