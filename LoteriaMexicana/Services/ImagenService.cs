namespace LoteriaMexicana.Services;

public sealed class ImagenService : IDisposable
{
    private readonly string _carpeta;
    private readonly Dictionary<int, Image> _cache = new();
    private bool _disposed;

    public ImagenService(string carpeta) { _carpeta = carpeta; }

    public Image? ObtenerImagenCarta(int numero)
    {
        if (_cache.TryGetValue(numero, out var img)) return img;
        var ruta = Path.Combine(_carpeta, $"{numero}.jpg");
        if (!File.Exists(ruta)) return null;
        img = Image.FromFile(ruta);
        _cache[numero] = img;
        return img;
    }

    public void Dispose()
    {
        if (_disposed) return;
        foreach (var img in _cache.Values) img.Dispose();
        _cache.Clear(); _disposed = true;
    }
}
