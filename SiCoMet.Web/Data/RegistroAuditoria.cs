using System.ComponentModel.DataAnnotations;

namespace SiCoMet.Web.Data;

public enum TipoAccionAuditoria
{
    Crear,
    Editar,
    Eliminar
}

public class RegistroAuditoria
{
    public int Id { get; set; }

    public string? UsuarioId { get; set; }

    [MaxLength(150)]
    public string NombreUsuario { get; set; } = "";

    public DateTime FechaHora { get; set; } = DateTime.UtcNow;

    [MaxLength(100)]
    public string Entidad { get; set; } = "";

    public TipoAccionAuditoria Accion { get; set; }

    [MaxLength(500)]
    public string Mensaje { get; set; } = "";
}
