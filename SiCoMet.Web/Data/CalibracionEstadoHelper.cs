namespace SiCoMet.Web.Data;

/// <summary>
/// Estado de calibración de un instrumento. Es un estado derivado (no se guarda en base de datos).
/// </summary>
public enum EstadoCalibracion
{
    /// <summary>Su calibración venció, o nunca se le registró ninguna.</summary>
    Vencida,

    /// <summary>Fue enviado al taller y aún no se registra el resultado.</summary>
    EnTaller,

    /// <summary>Calibración vigente (FechaProximaCalibracion >= hoy).</summary>
    Calibrado
}

public static class CalibracionEstadoHelper
{
    /// <summary>Días de antelación para marcar un instrumento calibrado como "por vencer".</summary>
    public const int DiasAvisoVencimiento = 30;

    /// <summary>Calibración más reciente: la de mayor fecha de envío (desempata por Id).</summary>
    public static Calibracion? Ultima(IEnumerable<Calibracion> calibraciones) =>
        calibraciones
            .OrderByDescending(c => c.FechaEnvioTaller)
            .ThenByDescending(c => c.Id)
            .FirstOrDefault();

    public static EstadoCalibracion Calcular(Calibracion? ultima, DateOnly hoy)
    {
        if (ultima is null)
            return EstadoCalibracion.Vencida;

        if (ultima.FechaCalibracion is null)
            return EstadoCalibracion.EnTaller;

        if (ultima.FechaProximaCalibracion is null || ultima.FechaProximaCalibracion.Value < hoy)
            return EstadoCalibracion.Vencida;

        return EstadoCalibracion.Calibrado;
    }

    /// <summary>
    /// Días restantes hasta el vencimiento si el instrumento está calibrado y vence dentro
    /// del período de aviso; null en cualquier otro caso.
    /// </summary>
    public static int? DiasParaVencer(Calibracion? ultima, DateOnly hoy)
    {
        if (Calcular(ultima, hoy) != EstadoCalibracion.Calibrado)
            return null;

        var dias = ultima!.FechaProximaCalibracion!.Value.DayNumber - hoy.DayNumber;
        return dias <= DiasAvisoVencimiento ? dias : null;
    }
}
