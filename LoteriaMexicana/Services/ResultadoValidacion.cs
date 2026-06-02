namespace LoteriaMexicana.Services;

public sealed record ResultadoValidacion(
    bool EsVictoria,
    bool HayTrampa,
    IReadOnlyList<int> CartasTramposas)
{
    public static ResultadoValidacion Victoria()     => new(true,  false, Array.Empty<int>());
    public static ResultadoValidacion Trampa(IEnumerable<int> c) => new(false, true,  c.ToList().AsReadOnly());
    public static ResultadoValidacion FallaLimpia() => new(false, false, Array.Empty<int>());
}
