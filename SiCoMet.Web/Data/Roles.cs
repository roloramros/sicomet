namespace SiCoMet.Web.Data;

public static class Roles
{
    public const string Administrador = "Administrador";
    public const string Directores = "Directores";
    public const string Tecnicos = "Técnicos";
    public const string Usuario = "Usuario";
    public const string Calibrador = "Calibrador";

    public static readonly string[] Todos =
    {
        Administrador,
        Directores,
        Tecnicos,
        Usuario,
        Calibrador
    };
}
