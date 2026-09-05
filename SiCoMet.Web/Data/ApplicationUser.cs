using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace SiCoMet.Web.Data;

// Add profile data for application users by adding properties to the ApplicationUser class
public class ApplicationUser : IdentityUser
{
    [Required]
    [MaxLength(150)]
    public string NombreCompleto { get; set; } = string.Empty;
}

