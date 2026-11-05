using System.ComponentModel.DataAnnotations;

namespace SiCoMet.Web.Data;

public class Producto
{
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    [Display(Name = "Nombre")]
    public string Nombre { get; set; } = string.Empty;

    [Display(Name = "Activo")]
    public bool Activo { get; set; } = true;

    public ICollection<Tanque> Tanques { get; set; } = new List<Tanque>();
}
