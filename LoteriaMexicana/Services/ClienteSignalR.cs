using LoteriaMexicana.Domain;
using Microsoft.AspNetCore.SignalR.Client;

namespace LoteriaMexicana.Services;

public class ClienteSignalR : IAsyncDisposable
{
    private HubConnection? _conexion;
    private readonly string _url;
    private string _nombre = "";

    public ClienteSignalR(string url)
    {
        if (!url.StartsWith("http://") && !url.StartsWith("https://"))
            url = "http://" + url;
        if (!url.EndsWith("/loteriahub"))
            url = url.TrimEnd('/') + "/loteriahub";
        _url = url;
    }

    // ── Eventos ───────────────────────────────────────────────────────────────
    public event Action<bool, string>? RolAsignado;
    public event Action<List<CasillaDto>>? TablaAsignada;
    public event Action<string>? JuegoIniciado;
    public event Action<string, List<CasillaDto>>? JuegoYaIniciado;
    public event Action<int, string, string>? CartaCantada;
    public event Action? BarajaReiniciada;
    public event Action? BarajaRebrajada;
    public event Action<List<int>>? MarcasActualizadas;
    public event Action<string>? HayGanador;
    public event Action<List<int>>? Trampa;
    public event Action? FalsaAlarma;
    public event Action<List<JugadorDto>>? JugadoresActualizados;
    public event Action<string>? Desconectado;
    public event Action<string, string>? MensajeRecibido;
    public event Action<string>? FormatoActual;
    public event Action? PreguntarJugarDeNuevo;
    public event Action<int, int, int>? ProgresoVotacion;
    public event Action? NuevaRondaIniciando;
    public event Action<List<PuntajeDto>>? PartidaFinalizada;
    public event Action<int, bool>? ConfiguracionActualizada;

    public bool Conectado => _conexion?.State == HubConnectionState.Connected;

    // ── Conectar ──────────────────────────────────────────────────────────────
    public async Task ConectarAsync()
    {
        _conexion = new HubConnectionBuilder()
            .WithUrl(_url)
            .WithAutomaticReconnect()
            .Build();

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

        _conexion.On<string>("FormatoActual", fmt =>
            FormatoActual?.Invoke(fmt));

        _conexion.On("PreguntarJugarDeNuevo", () =>
            PreguntarJugarDeNuevo?.Invoke());

        _conexion.On<int, int, int>("ProgresoVotacion", (si, no, total) =>
            ProgresoVotacion?.Invoke(si, no, total));

        _conexion.On("NuevaRondaIniciando", () =>
            NuevaRondaIniciando?.Invoke());

        _conexion.On<List<PuntajeDto>>("PartidaFinalizada", puntajes =>
            PartidaFinalizada?.Invoke(puntajes));

        _conexion.On<int, bool>("ConfiguracionActualizada", (tamaño, dobles) =>
            ConfiguracionActualizada?.Invoke(tamaño, dobles));

        _conexion.Closed += ex =>
        {
            Desconectado?.Invoke(ex?.Message ?? "Conexión cerrada.");
            return Task.CompletedTask;
        };

        _conexion.Reconnected += async _ =>
        {
            if (!string.IsNullOrEmpty(_nombre))
                await _conexion.InvokeAsync("UnirseAlJuego", _nombre);
        };

        await _conexion.StartAsync();
    }

    // ── Métodos públicos ──────────────────────────────────────────────────────
    public Task UnirseAlJuego(string nombre)
    {
        _nombre = nombre;
        return Invoke("UnirseAlJuego", nombre);
    }

    public Task IniciarJuego(string formato) => Invoke("IniciarJuego", formato);
    public Task ReconectarComoHost(string nombre) => Invoke("ReconectarComoHost", nombre);
    public Task ToggleCarta(int numero) => Invoke("ToggleCarta", numero);
    public Task EnviarMensaje(string mensaje) => Invoke("EnviarMensaje", mensaje);
    public Task ReiniciarYJugar(string formato) => Invoke("ReiniciarYJugar", formato);

    public Task CantarCarta() => InvokeVoid("CantarCarta");
    public Task ReclamarLoteria() => InvokeVoid("ReclamarLoteria");
    public Task PedirNuevaTabla() => InvokeVoid("PedirNuevaTabla");
    public Task ReiniciarBaraja() => InvokeVoid("ReiniciarBaraja");

    public Task ConfigurarPartida(string formato, bool dobles, int tamañoTabla)
        => Invoke3("ConfigurarPartida", formato, dobles, tamañoTabla);

    public Task EnviarTablaPersonalizada(List<int> numerosElegidos)
        => Invoke("EnviarTablaPersonalizada", numerosElegidos);

    // ── Helpers privados ──────────────────────────────────────────────────────
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

    private Task Invoke3(string metodo, object arg1, object arg2, object arg3)
    {
        if (_conexion == null || _conexion.State != HubConnectionState.Connected)
            return Task.CompletedTask;
        return _conexion.InvokeAsync(metodo, arg1, arg2, arg3);
    }

    // ── Dispose ───────────────────────────────────────────────────────────────
    public async ValueTask DisposeAsync()
    {
        if (_conexion != null)
            await _conexion.DisposeAsync();
        GC.SuppressFinalize(this);
    }
}