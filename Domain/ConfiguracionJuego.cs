namespace LoteriaMexicana.Domain;

using LoteriaMexicana.Domain.Enums;

public class ConfiguracionJuego
{
    public int TamañoTabla { get; set; } = 5; // 4, 5, 6, etc.
    public bool PermitirCartasDobles { get; set; } = true;
    public List<FormatoGanador> FormatosActivos { get; set; } = new()
    {
        FormatoGanador.LineaHorizontal,
        FormatoGanador.LineaVertical,
        FormatoGanador.Diagonal,
        FormatoGanador.Cruz,
        FormatoGanador.Cruzita,
        FormatoGanador.TablaLlena
    };

    public ConfiguracionJuego() { }

    public ConfiguracionJuego(int tamaño, bool dobles, List<FormatoGanador> formatos)
    {
        TamañoTabla = tamaño;
        PermitirCartasDobles = dobles;
        FormatosActivos = formatos ?? new List<FormatoGanador>();
    }

    public string ObtenerResumen()
    {
        return $"Tabla {TamañoTabla}x{TamañoTabla} | Dobles: {(PermitirCartasDobles ? "Sí" : "No")} | Formas: {FormatosActivos.Count}";
    }
}
