using LoteriaMexicana.Domain;
using LoteriaMexicana.Hubs;          // CasillaDto
using Microsoft.AspNetCore.SignalR.Client;

namespace LoteriaMexicana.Services;

public class ClienteSignalR : IAsyncDisposable
{
    private HubConnection? _conexion;

    // FIX Bug#1: guardar la URL para conectar más tarde (desde FormJuegoRed),
    // no desde FormConexion antes de que los eventos estén suscritos.
    private readonly string _url;

    public ClienteSignalR(string url)
    {
        if (!url.StartsWith("http://") && !url.StartsWith("https://"))
            url = "http://" + url;
        if (!url.EndsWith("/loteriahub"))
            url = url.TrimEnd('/') + "/loteriahub";
        _url = url;
    }

    // ── Eventos ───────────────────────────────────────────────────────────────
    public event Action<bool, string>?              RolAsignado;
    public event Action<List<CasillaDto>>?          TablaAsignada;
    public event Action<string>?                    JuegoIniciado;
    public event Action<string, List<CasillaDto>>?  JuegoYaIniciado;
    public event Action<int, string, string>?       CartaCantada;
    public event Action?                            BarajaReiniciada;
    public event Action?                            BarajaRebrajada;
    public event Action<List<int>>?                 MarcasActualizadas;
    public event Action<string>?                    HayGanador;
    public event Action<List<int>>?                 Trampa;
    public event Action?                            FalsaAlarma;
    public event Action<List<JugadorDto>>?          JugadoresActualizados;
    public event Action<string>?                    Desconectado;
    public event Action<string, string>?            MensajeRecibido;

    public bool Conectado => _conexion?.State == HubConnectionState.Connected;

    // =========================================================================
    // CONECTAR  — llamado desde FormJuegoRed.UnirseAsync(), DESPUÉS de suscribir
    // =========================================================================

    public async Task ConectarAsync()
    {
        _conexion = new HubConnectionBuilder()
            .WithUrl(_url)
            .WithAutomaticReconnect()
            .Build();

        // ── Registrar todos los eventos del servidor ───────────────────────────

        _conexion.On<bool>("RolesActualizados", esHost =>
            RolAsignado?.Invoke(esHost, ""));

        _conexion.On<List<CasillaDto>>("TablaAsignada", casillas =>
            TablaAsignada?.Invoke(casillas));

        _conexion.On<string>("JuegoIniciado", formato =>
            JuegoIniciado?.Invoke(formato));

        _conexion.On<string, List<CasillaDto>>("JuegoYaIniciado", (formato, casillas) =>
            JuegoYaIniciado?.Invoke(formato, casillas));

        _conexion.On<int, string, string>("CartaCantada", (numero, nombre, frase) =>
            CartaCantada?.Invoke(numero, nombre, frase));

        _conexion.On("BarajaReiniciada", () =>
            BarajaReiniciada?.Invoke());

        _conexion.On("BarajaRebrajada", () =>
            BarajaRebrajada?.Invoke());

        _conexion.On<List<int>>("MarcasActualizadas", marcas =>
            MarcasActualizadas?.Invoke(marcas));

        _conexion.On<string>("HayGanador", nombre =>
            HayGanador?.Invoke(nombre));

        _conexion.On<List<int>>("Trampa", numeros =>
            Trampa?.Invoke(numeros));

        _conexion.On("FalsaAlarma", () =>
            FalsaAlarma?.Invoke());

        _conexion.On<List<JugadorDto>>("JugadoresActualizados", lista =>
            JugadoresActualizados?.Invoke(lista));

        _conexion.On<string, string>("MensajeRecibido", (nombre, texto) =>
            MensajeRecibido?.Invoke(nombre, texto));

        _conexion.Closed += ex =>
        {
            Desconectado?.Invoke(ex?.Message ?? "Conexión cerrada.");
            return Task.CompletedTask;
        };

        await _conexion.StartAsync();
    }

    // =========================================================================
    // MÉTODOS QUE EL FORM INVOCA → SERVIDOR
    // =========================================================================

    public Task UnirseAlJuego(string nombre)  => Invoke("UnirseAlJuego", nombre);
    public Task IniciarJuego(string formato)  => Invoke("IniciarJuego", formato);
    public Task CantarCarta()                 => InvokeVoid("CantarCarta");
    public Task ToggleCarta(int numero)       => Invoke("ToggleCarta", numero);
    public Task ReclamarLoteria()             => InvokeVoid("ReclamarLoteria");
    public Task EnviarMensaje(string mensaje) => Invoke("EnviarMensaje", mensaje);
    public Task PedirNuevaTabla()             => InvokeVoid("PedirNuevaTabla");
    public Task ReiniciarBaraja()             => InvokeVoid("ReiniciarBaraja");

    // FIX Bug#4: separar método sin args del método con args para evitar que
    // el array object[] sea pasado como un solo argumento al hub.
    private Task InvokeVoid(string metodo)
    {
        if (_conexion == null || _conexion.State != HubConnectionState.Connected)
            return Task.CompletedTask;
        return _conexion.InvokeAsync(metodo);
    }

    private Task Invoke(string metodo, object arg)
    {
        if (_conexion == null || _conexion.State != HubConnectionState.Connected)
            return Task.CompletedTask;
        return _conexion.InvokeAsync(metodo, arg);
    }

    public async ValueTask DisposeAsync()
    {
        if (_conexion != null)
            await _conexion.DisposeAsync();
    }
}
