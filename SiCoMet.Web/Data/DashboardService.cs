using Microsoft.EntityFrameworkCore;

namespace SiCoMet.Web.Data;

public record AlertaInstrumento(string Serie, string Posicion, string Tipo, string Area, DateOnly? Fecha, int? Dias);

public record CumplimientoArea(string Area, int Total, int Vigentes, int EnTaller, int Vencidos);

public record ResumenDashboard(
    int Total,
    int Vigentes,
    int PorVencer,
    int EnTaller,
    int Vencidos,
    int NoAptos,
    List<AlertaInstrumento> ProximosAVencer,
    List<AlertaInstrumento> VencidosMasAtrasados,
    List<CumplimientoArea> PorArea,
    DateTime Generado);

/// <summary>
/// Calcula el resumen del panel de inicio. Reutiliza <see cref="CalibracionEstadoHelper"/>
/// para que las cifras coincidan con las del panel /calibraciones.
/// </summary>
public class DashboardService
{
    private const int MaxAlertas = 8;

    private readonly IServiceScopeFactory _scopeFactory;

    public DashboardService(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    private record Fila(Instrumento Instrumento, Calibracion? Ultima, EstadoCalibracion Estado, int? DiasParaVencer);

    public async Task<ResumenDashboard> ObtenerResumenAsync()
    {
        // Scope propio: evita compartir DbContext con el layout u otros componentes.
        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var hoy = DateOnly.FromDateTime(DateTime.Today);

        var instrumentos = await db.Instrumentos
            .AsNoTracking()
            .AsSplitQuery()
            .Where(i => i.Activo)
            .Include(i => i.Area)
            .Include(i => i.TipoInstrumento)
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

        var proximos = filas
            .Where(EsPorVencer)
            .OrderBy(f => f.DiasParaVencer)
            .ThenBy(f => f.Instrumento.Serie)
            .Take(MaxAlertas)
            .Select(f => new AlertaInstrumento(
                f.Instrumento.Serie, f.Instrumento.Posicion ?? "", f.Instrumento.TipoInstrumento.Nombre,
                f.Instrumento.Area.Nombre, f.Ultima!.FechaProximaCalibracion, f.DiasParaVencer))
            .ToList();

        // Vencidos: primero los de mayor atraso; los que nunca se calibraron, al final.
        var vencidos = filas
            .Where(f => f.Estado == EstadoCalibracion.Vencida)
            .OrderBy(f => f.Ultima?.FechaProximaCalibracion is null ? 1 : 0)
            .ThenBy(f => f.Ultima?.FechaProximaCalibracion)
            .ThenBy(f => f.Instrumento.Serie)
            .Take(MaxAlertas)
            .Select(f => new AlertaInstrumento(
                f.Instrumento.Serie, f.Instrumento.Posicion ?? "", f.Instrumento.TipoInstrumento.Nombre,
                f.Instrumento.Area.Nombre, f.Ultima?.FechaProximaCalibracion,
                f.Ultima?.FechaProximaCalibracion is { } p ? hoy.DayNumber - p.DayNumber : null))
            .ToList();

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

        return new ResumenDashboard(
            Total: filas.Count,
            Vigentes: filas.Count(EsVigente),
            PorVencer: filas.Count(EsPorVencer),
            EnTaller: filas.Count(f => f.Estado == EstadoCalibracion.EnTaller),
            Vencidos: filas.Count(f => f.Estado == EstadoCalibracion.Vencida),
            NoAptos: filas.Count(f => f.Estado == EstadoCalibracion.Calibrado
                                      && f.Ultima?.EstadoTecnico == EstadoTecnico.NoApto),
            ProximosAVencer: proximos,
            VencidosMasAtrasados: vencidos,
            PorArea: porArea,
            Generado: DateTime.Now);
    }
}
