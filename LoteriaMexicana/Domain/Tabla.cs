namespace LoteriaMexicana.Domain;

public class Tabla
{
    public int Filas { get; }
    public int Columnas { get; }
    public int TotalCasillas => Filas * Columnas;
    public Carta[,] Casillas { get; }

    public Tabla(int filas, int columnas)
    {
        Filas = filas;
        Columnas = columnas;
        Casillas = new Carta[filas, columnas];
    }

    public static Tabla Vacia(int filas, int columnas) => new(filas, columnas);

    /// <summary>Genera una tabla aleatoria con o sin dobles.</summary>
    public static Tabla GenerarAleatoria(
        IEnumerable<Carta> todas,
        int filas,
        int columnas,
        bool permitirDobles = false)
    {
        int total = filas * columnas;
        var pool = todas.OrderBy(_ => Random.Shared.Next()).ToList();

        List<Carta> lista;

        if (permitirDobles)
        {
            var seleccion = pool.Take(total - 1).ToList();
            var duplicada = seleccion[Random.Shared.Next(seleccion.Count)];
            lista = new List<Carta>(seleccion) { duplicada }
                .OrderBy(_ => Random.Shared.Next())
                .ToList();
        }
        else
        {
            lista = pool.Take(total).ToList();
        }

        var tabla = new Tabla(filas, columnas);
        for (int i = 0; i < total; i++)
            tabla.Casillas[i / columnas, i % columnas] = lista[i];

        return tabla;
    }

    /// <summary>
    /// Crea una tabla con exactamente las cartas que el jugador eligió,
    /// colocadas en orden aleatorio dentro de la cuadrícula.
    /// </summary>
    public static Tabla CrearDesdeSeleccion(
        IEnumerable<Carta> cartasElegidas,
        int filas,
        int columnas)
    {
        var lista = cartasElegidas.OrderBy(_ => Random.Shared.Next()).ToList();
        int total = filas * columnas;

        if (lista.Count != total)
            throw new ArgumentException(
                $"Se requieren exactamente {total} cartas, se recibieron {lista.Count}.");

        var tabla = new Tabla(filas, columnas);
        for (int i = 0; i < total; i++)
            tabla.Casillas[i / columnas, i % columnas] = lista[i];

        return tabla;
    }

    public static int IndiceDe(int f, int col, int columnas) => f * columnas + col;
}