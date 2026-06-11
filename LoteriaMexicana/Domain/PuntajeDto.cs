namespace LoteriaMexicana.Domain;

/// <summary>
/// Representa el puntaje acumulado de un jugador en la sesión.
/// </summary>
public sealed class PuntajeDto
{
    public string Nombre { get; init; } = string.Empty;
    public int Victorias { get; init; }
    public int Partidas { get; init; }

    public double PorcentajeVictorias =>
        Partidas == 0 ? 0 : Math.Round((double)Victorias / Partidas * 100, 1);

    public override string ToString() =>
        $"{Nombre}  —  {Victorias}/{Partidas} victorias ({PorcentajeVictorias}%)";
}