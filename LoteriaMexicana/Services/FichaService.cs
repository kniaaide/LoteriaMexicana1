namespace LoteriaMexicana.Services;

/// <summary>
/// Carga y provee las imágenes de las fichas disponibles.
/// Cada ficha es una tapita que el jugador arrastra sobre su tabla.
/// </summary>
public sealed class FichaService : IDisposable
{
    private readonly string _carpeta;
    private readonly Dictionary<string, Image> _cache = new();
    private bool _disposed;

    // Nombres de archivo de las fichas disponibles (sin extensión)
    public static readonly string[] NombresFichas = { "moeda1", "moneda2", "moneda10","50c" };

    public FichaService(string carpeta) { _carpeta = carpeta; }

    /// <summary>Carga todas las fichas al iniciar.</summary>
    public void Precargar()
    {
        foreach (var nombre in NombresFichas)
            ObtenerFicha(nombre);
    }

    public Image? ObtenerFicha(string nombre)
    {
        if (_cache.TryGetValue(nombre, out var img)) return img;
        var ruta = Path.Combine(_carpeta, $"{nombre}.png");
        if (!File.Exists(ruta)) return null;
        img = Image.FromFile(ruta);
        _cache[nombre] = img;
        return img;
    }

    public IReadOnlyDictionary<string, Image> TodasLasFichas() => _cache;

    public void Dispose()
    {
        if (_disposed) return;
        foreach (var img in _cache.Values) img.Dispose();
        _cache.Clear(); _disposed = true;
    }
}
