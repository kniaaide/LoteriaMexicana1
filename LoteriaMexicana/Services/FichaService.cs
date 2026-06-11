namespace LoteriaMexicana.Services;

public sealed class FichaService : IDisposable
{
    private readonly string _carpeta;
    private readonly Dictionary<string, Image> _cache = new();
    private bool _disposed;

    public static readonly string[] NombresFichas = { "moeda1", "moneda2", "moneda10", "50c" };

    public FichaService(string carpeta) { _carpeta = carpeta; }

    public void Precargar()
    {
        foreach (var nombre in NombresFichas)
            ObtenerFicha(nombre);
    }

    public Image? ObtenerFicha(string nombre)
    {
        if (_cache.TryGetValue(nombre, out var img)) return img;

        // Primero intenta desde disco
        var ruta = Path.Combine(_carpeta, $"{nombre}.png");
        if (File.Exists(ruta))
        {
            img = Image.FromFile(ruta);
            _cache[nombre] = img;
            return img;
        }

        // Si no existe en disco, busca en recursos embebidos
        var asm = System.Reflection.Assembly.GetExecutingAssembly();
        var nombreRecurso = asm.GetManifestResourceNames()
            .FirstOrDefault(n => n.EndsWith($"{nombre}.png", StringComparison.OrdinalIgnoreCase));

        if (nombreRecurso != null)
        {
            using var stream = asm.GetManifestResourceStream(nombreRecurso);
            if (stream != null)
            {
                img = Image.FromStream(stream);
                _cache[nombre] = img;
                return img;
            }
        }

        return null;
    }

    public IReadOnlyDictionary<string, Image> TodasLasFichas() => _cache;

    public void Dispose()
    {
        if (_disposed) return;
        foreach (var img in _cache.Values) img.Dispose();
        _cache.Clear();
        _disposed = true;
    }
}