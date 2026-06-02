namespace LoteriaMexicana.Domain;

public class Tabla
{
    public const int Filas = 5;
    public const int Columnas = 5;
    public const int TotalCasillas = Filas * Columnas;
    public Carta[,] Casillas { get; }
    private Tabla(Carta[,] casillas) { Casillas = casillas; }
    public static Tabla Vacia() => new(new Carta[Filas, Columnas]);
    public static Tabla GenerarAleatoria(IEnumerable<Carta> todas)
    {
        var sel = todas.OrderBy(_ => Random.Shared.Next()).Take(TotalCasillas).ToArray();
        var c = new Carta[Filas, Columnas];
        for (int i = 0; i < TotalCasillas; i++) c[i / Columnas, i % Columnas] = sel[i];
        return new Tabla(c);
    }
    public static int IndiceDe(int f, int col) => f * Columnas + col;
}
