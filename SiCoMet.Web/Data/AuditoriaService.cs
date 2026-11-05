using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Http;

namespace SiCoMet.Web.Data;

public interface IAuditoriaService
{
    Task RegistrarAsync(string entidad, TipoAccionAuditoria accion, string mensaje, IReadOnlyList<CambioAuditoria>? cambios = null);
}

// Servicio simple para dejar constancia de acciones (crear/editar/eliminar) en cualquier
// modulo del sistema. Se llama explicitamente desde cada pantalla en el punto donde
// la accion se confirma, para evitar registrar cambios internos de Identity (ej. intentos
// de login fallidos) que no son acciones reales del usuario sobre los datos.
//
// El usuario se obtiene de dos fuentes:
//  - HttpContext: disponible en las paginas de formulario (renderizado estatico), donde
//    cada accion es una peticion HTTP normal.
//  - AuthenticationStateProvider: es la unica fuente fiable dentro de un circuito de
//    Blazor Server (@rendermode InteractiveServer), donde HttpContext llega vacio y la
//    auditoria quedaba registrada como "Sistema" sin usuario.
public class AuditoriaService(
    ApplicationDbContext db,
    IHttpContextAccessor httpContextAccessor,
    AuthenticationStateProvider authenticationStateProvider) : IAuditoriaService
{
    public async Task RegistrarAsync(string entidad, TipoAccionAuditoria accion, string mensaje, IReadOnlyList<CambioAuditoria>? cambios = null)
    {
        var user = await ObtenerUsuarioAsync();

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
            Mensaje = mensaje,
            Detalle = AuditoriaDiff.Serializar(cambios)
        });

        await db.SaveChangesAsync();
    }

    private async Task<ClaimsPrincipal?> ObtenerUsuarioAsync()
    {
        var usuarioHttp = httpContextAccessor.HttpContext?.User;
        if (usuarioHttp?.Identity?.IsAuthenticated == true)
        {
            return usuarioHttp;
        }

        try
        {
            var estado = await authenticationStateProvider.GetAuthenticationStateAsync();
            if (estado.User.Identity?.IsAuthenticated == true)
            {
                return estado.User;
            }
        }
        catch (InvalidOperationException)
        {
            // El estado de autenticacion aun no esta inicializado en este contexto.
        }

        return usuarioHttp;
    }
}
