using LoteriaMexicana.Domain;

namespace LoteriaMexicana.Services;

public class ServicioDesempate
{
    /// <summary>
    /// Filtra los jugadores que ganaron en esta ronda.
    /// </summary>
    public static List<string> ObtenerGanadores(List<string> todosLosJugadores, 
                                                   List<string> ganoresDeEstRonda)
    {
        return todosLosJugadores
            .Where(j => ganoresDeEstRonda.Contains(j))
            .ToList();
    }

    /// <summary>
    /// Desactiva fichas para jugadores que no ganaron en desempate.
    /// </summary>
    public static Dictionary<string, bool> GenerarFichasActivas(List<string> ganadores, 
                                                                  List<string> todosJugadores)
    {
        var fichasActivas = new Dictionary<string, bool>();
        
        foreach (var jugador in todosJugadores)
        {
            fichasActivas[jugador] = ganadores.Contains(jugador);
        }
        
        return fichasActivas;
    }

    /// <summary>
    /// Verifica si hay desempate (más de un ganador).
    /// </summary>
    public static bool HayDesempate(List<string> ganadores)
    {
        return ganadores.Count > 1;
    }

    /// <summary>
    /// Obtiene el mensaje de desempate.
    /// </summary>
    public static string ObtenerMensajeDesempate(List<string> ganadores)
    {
        if (ganadores.Count == 0)
            return "No hay ganadores en esta ronda.";
        
        if (ganadores.Count == 1)
            return $"¡{ganadores[0]} ganó!";
        
        return $"¡Desempate! {string.Join(", ", ganadores)} ganaron al mismo tiempo. " +
               "Solo ustedes pueden continuar jugando.";
    }
}
