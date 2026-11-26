namespace SiCoMet.Web.Data;

/// <summary>Estado de una línea del plan de calibración.</summary>
public enum EstadoPlan
{
    /// <summary>La calibración que sigue a la fecha planificada ya se realizó (aunque haya sido con retraso).</summary>
    Cumplido,

    /// <summary>Aún no se calibra y el mes planificado no ha terminado.</summary>
    Pendiente,

    /// <summary>Aún no se calibra y el mes planificado ya terminó.</summary>
    Atrasado
}

/// <summary>
/// Una línea del plan: un instrumento que le toca calibrarse en el mes de <see cref="FechaPlan"/>.
/// </summary>
/// <param name="FechaPlan">Fecha de vencimiento (próxima calibración) de la calibración anterior.</param>
/// <param name="FechaEjecucion">Fecha en que se calibró de verdad; nula si todavía no.</param>
/// <param name="EnTaller">Ya fue enviado al taller y falta registrar el resultado.</param>
public record ItemPlan(
    Instrumento Instrumento,
    DateOnly FechaPlan,
    EstadoPlan Estado,
    DateOnly? FechaEjecucion,
    bool EnTaller);

/// <summary>
/// Calcula el plan de calibración a partir de las calibraciones ya registradas, sin guardar
/// nada aparte: cada calibración con "próxima fecha" genera una línea de plan en el mes de
/// esa fecha, y esa línea se da por cumplida cuando existe una calibración posterior con
/// resultado registrado. Así el plan nunca queda desactualizado respecto a lo realmente calibrado.
/// </summary>
public static class PlanCalibracionHelper
{
    public static readonly string[] Meses =
    {
        "Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio",
        "Julio", "Agosto", "Septiembre", "Octubre", "Noviembre", "Diciembre"
    };

    /// <summary>
    /// Todas las líneas del plan de los instrumentos dados (de cualquier año). Los instrumentos
    /// deben traer cargadas sus calibraciones. Un instrumento sin ninguna calibración con
    /// próxima fecha no genera líneas: no hay de dónde calcular su mes.
    /// </summary>
    public static List<ItemPlan> Calcular(IEnumerable<Instrumento> instrumentos, DateOnly hoy)
    {
        var items = new List<ItemPlan>();

        foreach (var instrumento in instrumentos)
        {
            var ordenadas = instrumento.Calibraciones
                .OrderBy(c => c.FechaEnvioTaller)
                .ThenBy(c => c.Id)
                .ToList();

            for (var i = 0; i < ordenadas.Count; i++)
            {
                var vencimiento = ordenadas[i].FechaProximaCalibracion;
                if (vencimiento is null) continue;

                var siguiente = i + 1 < ordenadas.Count ? ordenadas[i + 1] : null;

                if (siguiente?.FechaCalibracion is not null)
                {
                    items.Add(new ItemPlan(instrumento, vencimiento.Value, EstadoPlan.Cumplido, siguiente.FechaCalibracion, false));
                    continue;
                }

                var finDeMes = new DateOnly(vencimiento.Value.Year, vencimiento.Value.Month, 1).AddMonths(1).AddDays(-1);
                var estado = finDeMes < hoy ? EstadoPlan.Atrasado : EstadoPlan.Pendiente;
                items.Add(new ItemPlan(instrumento, vencimiento.Value, estado, null, siguiente is not null));
            }
        }

        return items;
    }
}
