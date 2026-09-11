using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SiCoMet.Web.Data;

namespace Microsoft.AspNetCore.Routing;

internal static class IdentityComponentsEndpointRouteBuilderExtensions
{
    // Endpoint mínimo requerido por la página de Login: cerrar sesión.
    public static IEndpointConventionBuilder MapAdditionalIdentityEndpoints(this IEndpointRouteBuilder endpoints)
    {
        ArgumentNullException.ThrowIfNull(endpoints);

        var accountGroup = endpoints.MapGroup("/Account");

        accountGroup.MapPost("/Logout", async (
            ClaimsPrincipal user,
            SignInManager<ApplicationUser> signInManager,
            ApplicationDbContext dbContext,
            [FromForm] string returnUrl) =>
        {
            var email = user.Identity?.Name;
            var usuarioId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            await signInManager.SignOutAsync();

            if (!string.IsNullOrEmpty(email))
            {
                dbContext.RegistrosAcceso.Add(new RegistroAcceso
                {
                    UsuarioId = usuarioId,
                    Email = email,
                    Exitoso = true,
                    Detalle = "Cierre de sesión"
                });
                await dbContext.SaveChangesAsync();
            }

            return TypedResults.LocalRedirect($"~/{returnUrl}");
        });

        return accountGroup;
    }
}
