using Microsoft.EntityFrameworkCore;

namespace SiCoMet.Web.Data;

public record CumplimientoArea(string Area, int Total, int Vigentes, int EnTaller, int Vencidos);

public record TanquesProducto(string Producto, int Total, int Vigentes, int PorVencer, int Vencidos, int SinCertificado);

public record ResumenTanques(
    int Total,
    int Vigentes,
    int PorVencer,
    int Vencidos,
    int SinCertificado,
    List<TanquesProducto> PorProducto);

public record ResumenDashboard(
    int Total,
    int Vigentes,
    int PorVencer,
    int EnTaller,
    int Vencidos,
    int NoAptos,
    List<CumplimientoArea> PorArea,
    ResumenTanques Tanques,
    DateTime Generado);

/// <summary>
/// Calcula el resumen del panel de inicio. Reutiliza <see cref="CalibracionEstadoHelper"/>
/// para que las cifras de instrumentos coincidan con las del panel /calibraciones.
/// </summary>
public class DashboardService
{
    private readonly IServiceScopeFactory _scopeFactory;

    public DashboardService(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    private record Fila(Instrumento Instrumento, Calibracion? Ultima, EstadoCalibracion Estado, int? DiasParaVencer);

    private enum EstadoCert { Vigente, PorVencer, Vencido, Sin }

    public async Task<ResumenDashboard> ObtenerResumenAsync()
    {
        // Scope propio: evita compartir DbContext con el layout u otros componentes.
        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var hoy = DateOnly.FromDateTime(DateTime.Today);

        // ---------- Instrumentos ----------
        var instrumentos = await db.Instrumentos
            .AsNoTracking()
            .AsSplitQuery()
            .Where(i => i.Activo)
            .Include(i => i.Area)
            .Include(i => i.Calibraciones)
            .ToListAsync();

        var filas = instrumentos
            .Select(i =>
            {
                var ultima = CalibracionEstadoHelper.Ultima(i.Calibraciones);
                return new Fila(
                    i,
                    ultima,
                    CalibracionEstadoHelper.Calcular(ultima, hoy),
                    CalibracionEstadoHelper.DiasParaVencer(ultima, hoy));
            })
            .ToList();

        bool EsPorVencer(Fila f) => f.Estado == EstadoCalibracion.Calibrado && f.DiasParaVencer is not null;
        bool EsVigente(Fila f) => f.Estado == EstadoCalibracion.Calibrado && f.DiasParaVencer is null;

        var porArea = filas
            .GroupBy(f => f.Instrumento.Area.Nombre)
            .OrderBy(g => g.Key)
            .Select(g => new CumplimientoArea(
                g.Key,
                g.Count(),
                g.Count(f => f.Estado == EstadoCalibracion.Calibrado),
                g.Count(f => f.Estado == EstadoCalibracion.EnTaller),
                g.Count(f => f.Estado == EstadoCalibracion.Vencida)))
            .ToList();

        // ---------- Tanques (certificado de aforo) ----------
        var tanques = await db.Tanques
            .AsNoTracking()
            .Select(t => new
            {
                Producto = t.Producto != null ? t.Producto.Nombre : null,
                t.NumeroCertificadoAforo,
                t.VigenciaCertificado
            })
            .ToListAsync();

        var limite = hoy.AddDays(CalibracionEstadoHelper.DiasAvisoVencimiento);

        var tanquesEstado = tanques
            .Select(t =>
            {
                EstadoCert e;
                if (t.VigenciaCertificado is null) e = EstadoCert.Sin; // sin certificado o sin vigencia
                else if (t.VigenciaCertificado.Value < hoy) e = EstadoCert.Vencido;
                else if (t.VigenciaCertificado.Value <= limite) e = EstadoCert.PorVencer;
                else e = EstadoCert.Vigente;
                return (Producto: t.Producto ?? "Sin producto", Estado: e);
            })
            .ToList();

        var resumenTanques = new ResumenTanques(
            Total: tanquesEstado.Count,
            Vigentes: tanquesEstado.Count(t => t.Estado == EstadoCert.Vigente),
            PorVencer: tanquesEstado.Count(t => t.Estado == EstadoCert.PorVencer),
            Vencidos: tanquesEstado.Count(t => t.Estado == EstadoCert.Vencido),
            SinCertificado: tanquesEstado.Count(t => t.Estado == EstadoCert.Sin),
            PorProducto: tanquesEstado
                .GroupBy(t => t.Producto)
                .OrderBy(g => g.Key)
                .Select(g => new TanquesProducto(
                    g.Key,
                    g.Count(),
                    g.Count(t => t.Estado == EstadoCert.Vigente),
                    g.Count(t => t.Estado == EstadoCert.PorVencer),
                    g.Count(t => t.Estado == EstadoCert.Vencido),
                    g.Count(t => t.Estado == EstadoCert.Sin)))
                .ToList());

        return new ResumenDashboard(
            Total: filas.Count,
            Vigentes: filas.Count(EsVigente),
            PorVencer: filas.Count(EsPorVencer),
            EnTaller: filas.Count(f => f.Estado == EstadoCalibracion.EnTaller),
            Vencidos: filas.Count(f => f.Estado == EstadoCalibracion.Vencida),
            NoAptos: filas.Count(f => f.Estado == EstadoCalibracion.Calibrado
                                      && f.Ultima?.EstadoTecnico == EstadoTecnico.NoApto),
            PorArea: porArea,
            Tanques: resumenTanques,
            Generado: DateTime.Now);
    }
}
