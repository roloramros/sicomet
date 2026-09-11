using System.ComponentModel.DataAnnotations;

namespace SiCoMet.Web.Data;

public class RegistroAcceso
{
    public int Id { get; set; }

    // Se guarda el email como texto plano ademas del UsuarioId, para conservar el
    // registro legible aunque el usuario sea eliminado en el futuro.
    public string? UsuarioId { get; set; }

    [MaxLength(256)]
    public string Email { get; set; } = "";

    public DateTime FechaHora { get; set; } = DateTime.UtcNow;

    public bool Exitoso { get; set; }

    [MaxLength(45)]
    public string? DireccionIP { get; set; }

    [MaxLength(200)]
    public string? Detalle { get; set; }
}
