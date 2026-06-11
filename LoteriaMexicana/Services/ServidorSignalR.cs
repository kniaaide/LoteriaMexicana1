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
                FileName = "netsh",
                Arguments = $"advfirewall firewall add rule name=\"LoteriaMexicana\" " +
                            $"dir=in action=allow protocol=TCP localport={puerto}",
                CreateNoWindow = true,
                UseShellExecute = false,
            };
            System.Diagnostics.Process.Start(psi)?.WaitForExit(3000);
        }
        catch { }
    }

    public async Task DetenerAsync()
    {
        if (_app != null)
            await _app.StopAsync();
    }

    public async ValueTask DisposeAsync()
    {
        if (_disposed) return;
        _disposed = true;
        if (_app != null)
        {
            try { await _app.StopAsync(); }
            catch { }
            await _app.DisposeAsync();
            _app = null;
        }
        GC.SuppressFinalize(this); // FIX: agregado para cumplir IAsyncDisposable
    }
}