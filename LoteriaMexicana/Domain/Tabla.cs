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
       
        var seleccion = todas
            .OrderBy(_ => Random.Shared.Next())
            .Take(TotalCasillas - 1)   
            .ToList();

        
        var duplicada = seleccion[Random.Shared.Next(seleccion.Count)];

       
        var lista = new List<Carta>(seleccion) { duplicada };

      
        lista = lista.OrderBy(_ => Random.Shared.Next()).ToList();

        var casillas = new Carta[Filas, Columnas];
        for (int i = 0; i < TotalCasillas; i++)
            casillas[i / Columnas, i % Columnas] = lista[i];

        return new Tabla(casillas);
    }

    public static int IndiceDe(int f, int col) => f * Columnas + col;
}