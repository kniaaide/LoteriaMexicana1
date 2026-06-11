namespace LoteriaMexicana.Services;

public sealed class ImagenService : IDisposable
{
    // ── Instancia estática global ─────────────────────────────────────────────
    private static ImagenService? _instancia;

    public static ImagenService Instancia =>
        _instancia ?? throw new InvalidOperationException(
            "ImagenService no ha sido inicializado. Llama a Inicializar() antes de usar.");

    public static void Inicializar(string carpeta) =>
        _instancia = new ImagenService(carpeta);

    // ── Instancia ─────────────────────────────────────────────────────────────
    private readonly string _carpeta;
    private readonly Dictionary<int, Image> _cache = new();
    private bool _disposed;

    private ImagenService(string carpeta) { _carpeta = carpeta; }

    // ── API pública ───────────────────────────────────────────────────────────
    public Image? ObtenerImagenCarta(int numero)
    {
        if (_cache.TryGetValue(numero, out var img)) return img;

        // 1. Busca en disco
        var ruta = Path.Combine(_carpeta, $"{numero}.jpg");
        if (File.Exists(ruta))
        {
            img = Image.FromFile(ruta);
            _cache[numero] = img;
            return img;
        }

        // 2. Busca en recursos embebidos
        var asm = System.Reflection.Assembly.GetExecutingAssembly();
        var nombreRecurso = asm.GetManifestResourceNames()
            .FirstOrDefault(n =>
                n.EndsWith($"{numero}.jpg", StringComparison.OrdinalIgnoreCase) ||
                n.EndsWith($"carta{numero}.jpg", StringComparison.OrdinalIgnoreCase));

        if (nombreRecurso != null)
        {
            using var stream = asm.GetManifestResourceStream(nombreRecurso);
            if (stream != null)
            {
                img = Image.FromStream(stream);
                _cache[numero] = img;
                return img;
            }
        }

        return null;
    }

    // ── Dispose ───────────────────────────────────────────────────────────────
    public void Dispose()
    {
        if (_disposed) return;
        foreach (var img in _cache.Values) img.Dispose();
        _cache.Clear();
        _disposed = true;
    }
}