namespace LoteriaMexicana.Services;

public class ValidacionCartas
{
    /// <summary>
    /// Valida si una carta colocada en la tabla es válida (ha sido cantada).
    /// </summary>
    public static bool EsValida(int numeroCarta, HashSet<int> cartasCantadas)
    {
        return cartasCantadas.Contains(numeroCarta);
    }

    /// <summary>
    /// Obtiene todas las cartas inválidas que el jugador ha colocado.
    /// </summary>
    public static List<int> ObtenerCartasInvalidas(HashSet<int> cartasColocadas, HashSet<int> cartasCantadas)
    {
        return cartasColocadas
            .Where(numero => !cartasCantadas.Contains(numero))
            .ToList();
    }

    /// <summary>
    /// Obtiene todas las cartas válidas que el jugador ha colocado.
    /// </summary>
    public static List<int> ObtenerCartasValidas(HashSet<int> cartasColocadas, HashSet<int> cartasCantadas)
    {
        return cartasColocadas
            .Where(numero => cartasCantadas.Contains(numero))
            .ToList();
    }
}
