using System.ComponentModel.DataAnnotations;

namespace SiCoMet.Web.Data;

public class Producto
{
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Nombre { get; set; } = string.Empty;

    public bool Activo { get; set; } = true;

    public ICollection<Tanque> Tanques { get; set; } = new List<Tanque>();
}
