using LoteriaMexicana.Forms;
using LoteriaMexicana.Services;

namespace LoteriaMexicana;

public static class Program
{
    [STAThread]
    public static void Main()
    {
        var carpetaCartas = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory, "Resources", "Cartas");

        ImagenService.Inicializar(carpetaCartas);

        ApplicationConfiguration.Initialize();
        Application.Run(new FormConexion());

        // Libera imágenes al cerrar la aplicación
        ImagenService.Instancia.Dispose();
    }
}