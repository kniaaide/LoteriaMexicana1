using LoteriaMexicana.Hubs;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Sockets;
using System.Net;

namespace LoteriaMexicana.Services;

public class ServidorSignalR : IAsyncDisposable
{
    private WebApplication? _app;
    private bool _disposed;
    public const int Puerto = 5050;

    public static string ObtenerIpLocal()
    {
        try
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            var ip = host.AddressList
                .FirstOrDefault(a => a.AddressFamily == AddressFamily.InterNetwork);
            return ip?.ToString() ?? "127.0.0.1";
        }
        catch { return "127.0.0.1"; }
    }

    public async Task IniciarAsync()
    {
        // Abrir el puerto en el firewall de Windows automáticamente
        AbrirFirewall(Puerto);

        var builder = WebApplication.CreateBuilder();
        builder.Services.AddSignalR();
        builder.Services.AddCors(o => o.AddDefaultPolicy(p =>
            p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));
        builder.WebHost.UseUrls($"http://0.0.0.0:{Puerto}");
        _app = builder.Build();
        _app.UseCors();
        _app.MapHub<LoteriaHub>("/loteriahub");
        await _app.StartAsync();
    }

    private static void AbrirFirewall(int puerto)
    {
        try
        {
            var psi = new System.Diagnostics.ProcessStartInfo
            {
                FileName  = "netsh",
                Arguments = $"advfirewall firewall add rule name=\"LoteriaMexicana\" " +
                            $"dir=in action=allow protocol=TCP localport={puerto}",
                CreateNoWindow  = true,
                UseShellExecute = false,
            };
            System.Diagnostics.Process.Start(psi)?.WaitForExit(3000);
        }
        catch { /* Si falla, continuar — el usuario puede abrir el puerto manualmente */ }
    }

    public async Task DetenerAsync()
    {
        if (_app != null)
            await _app.StopAsync();
    }

    // FIX Bug#5 y Bug#6: implementar IAsyncDisposable en lugar de IDisposable
    // para evitar .Wait() bloqueante en el hilo UI, y asegurar que StopAsync
    // siempre se llame antes de Dispose para liberar el puerto correctamente.
    public async ValueTask DisposeAsync()
    {
        if (_disposed) return;
        _disposed = true;
        if (_app != null)
        {
            try   { await _app.StopAsync(); }
            catch { /* ignorar errores al detener */ }
            await _app.DisposeAsync();
            _app = null;
        }
    }
}
