using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

namespace SiCoMet.Web.Data;

// Agrega el NombreCompleto como claim (dato incluido en la cookie de sesión),
// para que componentes como el layout puedan mostrarlo sin volver a consultar la base de datos.
public class CustomUserClaimsPrincipalFactory : UserClaimsPrincipalFactory<ApplicationUser, IdentityRole>
{
    public CustomUserClaimsPrincipalFactory(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        Microsoft.Extensions.Options.IOptions<IdentityOptions> optionsAccessor)
        : base(userManager, roleManager, optionsAccessor)
    {
    }

    protected override async Task<ClaimsIdentity> GenerateClaimsAsync(ApplicationUser user)
    {
        var identity = await base.GenerateClaimsAsync(user);
        identity.AddClaim(new Claim("NombreCompleto", user.NombreCompleto ?? ""));
        return identity;
    }
}
