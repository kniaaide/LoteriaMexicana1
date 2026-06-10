namespace LoteriaMexicana.Domain;

public class Tabla
{
    public int Filas { get; }
    public int Columnas { get; }
    public int TotalCasillas => Filas * Columnas;

    public Carta[,] Casillas { get; }

    private Tabla(Carta[,] casillas)
    { 
        Casillas = casillas;
        Filas = casillas.GetLength(0);
        Columnas = casillas.GetLength(1);
    }

    public static Tabla Vacia(int tamaño = 5) => new(new Carta[tamaño, tamaño]);

    public static Tabla GenerarAleatoria(IEnumerable<Carta> todas, int tamaño = 5, bool permitirDobles = true)
    {
        int totalCasillas = tamaño * tamaño;
        var cartasDisponibles = todas.ToList();

        if (cartasDisponibles.Count < totalCasillas && !permitirDobles)
            throw new InvalidOperationException($"No hay suficientes cartas para una tabla de {tamaño}x{tamaño} sin dobles.");

        List<Carta> seleccion;

        if (permitirDobles)
        {
            // Con dobles: seleccionar aleatorio, y puede haber duplicadas
            seleccion = cartasDisponibles
                .OrderBy(_ => Random.Shared.Next())
                .Take(totalCasillas - 1)
                .ToList();

            // Agregar una carta duplicada
            var duplicada = seleccion[Random.Shared.Next(seleccion.Count)];
            seleccion.Add(duplicada);
        }
        else
        {
            // Sin dobles: todas las cartas son únicas
            seleccion = cartasDisponibles
                .OrderBy(_ => Random.Shared.Next())
                .Take(totalCasillas)
                .ToList();
        }

        // Barajar la selección final
        seleccion = seleccion.OrderBy(_ => Random.Shared.Next()).ToList();

        var casillas = new Carta[tamaño, tamaño];
        for (int i = 0; i < totalCasillas; i++)
            casillas[i / tamaño, i % tamaño] = seleccion[i];

        return new Tabla(casillas);
    }

    public static int IndiceDe(int f, int col) => f * 5 + col; // Para compatibilidad

    public int ObtenerIndice(int fila, int columna) => fila * Columnas + columna;
}
