using LoteriaMexicana.Domain;
using LoteriaMexicana.Domain.Enums;
using LoteriaMexicana.Services;
using Microsoft.AspNetCore.SignalR;

namespace LoteriaMexicana.Hubs;

public class LoteriaHub : Hub
{
    // =========================================================================
    // ESTADO COMPARTIDO
    // =========================================================================
    private static readonly object _lock = new();
    private static readonly Dictionary<string, JugadorInfo> _jugadores = new();
    private static readonly JuegoService _juego = new();
    private static bool _juegoIniciado = false;
    private static string _formatoActual = "TablaLlena";

    // ── Configuración de partida persistente (la elige el host) ──────────────
    private static bool _dobles = false;
    private static JuegoService.TamañoTabla _tamañoTabla = JuegoService.TamañoTabla.Cuatro;

    private static readonly Dictionary<string, string> _idANombre = new();
    private static readonly Dictionary<string, HashSet<int>> _marcas = new();

    // =========================================================================
    // CONEXIÓN / DESCONEXIÓN
    // =========================================================================
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        string? nombre = null;
        lock (_lock)
        {
            if (_idANombre.TryGetValue(Context.ConnectionId, out nombre))
            {
                _idANombre.Remove(Context.ConnectionId);
                _jugadores.Remove(nombre);
                _marcas.Remove(nombre);

                if (!_jugadores.Any(j => j.Value.EsHost) && _jugadores.Count > 0)
                    _jugadores.Values.First().EsHost = true;
            }
        }

        if (nombre != null) await NotificarJugadores();
        await base.OnDisconnectedAsync(exception);
    }

    // =========================================================================
    // UNIRSE AL JUEGO
    // =========================================================================
    public async Task UnirseAlJuego(string nombre)
    {
        nombre = nombre.Trim();
        if (string.IsNullOrEmpty(nombre)) return;

        bool esHost = false;
        bool juegoYaIniciado = false;
        List<CasillaDto>? casillasExistentes = null;

        lock (_lock)
        {
            if (_jugadores.TryGetValue(nombre, out var infoExistente))
            {
                var viejoId = _idANombre.FirstOrDefault(x => x.Value == nombre).Key;
                if (viejoId != null) _idANombre.Remove(viejoId);
                _idANombre[Context.ConnectionId] = nombre;
                esHost = infoExistente.EsHost;
            }
            else
            {
                esHost = _jugadores.Count == 0;
                _jugadores[nombre] = new JugadorInfo { Nombre = nombre, EsHost = esHost, Victorias = 0 };
                _idANombre[Context.ConnectionId] = nombre;
                _marcas[nombre] = new HashSet<int>();

                if (!_juego.Jugadores.Any(j => j.Nombre == nombre))
                    _juego.AgregarJugador(nombre);
            }

            juegoYaIniciado = _juegoIniciado;

            if (juegoYaIniciado)
            {
                var jugador = _juego.Jugadores.FirstOrDefault(j => j.Nombre == nombre);
                if (jugador != null)
                    casillasExistentes = TablaACasillas(jugador.Tabla);
            }
        }

        await Clients.Caller.SendAsync("RolesActualizados", esHost);

        if (juegoYaIniciado && casillasExistentes != null)
        {
            await Clients.Caller.SendAsync("JuegoYaIniciado", _formatoActual, casillasExistentes);
            HashSet<int> misMarcas;
            lock (_lock) { misMarcas = _marcas.TryGetValue(nombre, out var m) ? m : new(); }
            await Clients.Caller.SendAsync("MarcasActualizadas", misMarcas.ToList());
        }
        else if (!juegoYaIniciado)
        {
            List<CasillaDto> casillas;
            lock (_lock)
            {
                // ── Aplica el tamaño elegido por el host al generar la tabla ──
                _juego.ConfigurarPartida(_dobles, _tamañoTabla);

                var jugador = _juego.Jugadores.FirstOrDefault(j => j.Nombre == nombre);
                if (jugador != null)
                {
                    _juego.GenerarTablaParaJugador(jugador);
                    casillas = TablaACasillas(jugador.Tabla);
                }
                else casillas = new();
            }
            await Clients.Caller.SendAsync("TablaAsignada", casillas);
        }

        await NotificarJugadores();
    }

    // =========================================================================
    // CONFIGURAR PARTIDA (solo host)
    // =========================================================================
    public async Task ConfigurarPartida(string formato, bool dobles, int tamañoTabla)
    {
        string? nombre;
        lock (_lock) { _idANombre.TryGetValue(Context.ConnectionId, out nombre); }
        if (nombre == null) return;

        bool esHost;
        lock (_lock) { esHost = _jugadores.TryGetValue(nombre, out var inf) && inf.EsHost; }
        if (!esHost) return;

        lock (_lock)
        {
            _dobles = dobles;
            _tamañoTabla = tamañoTabla == 5
                ? JuegoService.TamañoTabla.Cinco
                : JuegoService.TamañoTabla.Cuatro;
            _formatoActual = formato;
            _juego.ConfigurarPartida(_dobles, _tamañoTabla);
        }

        // ── Notificar a todos el tamaño de tabla elegido ──────────────────────
        await Clients.All.SendAsync("ConfiguracionActualizada", tamañoTabla, dobles);
    }

    // =========================================================================
    // INICIAR JUEGO (solo host)
    // =========================================================================
    public async Task IniciarJuego(string formato)
    {
        string? nombre;
        lock (_lock) { _idANombre.TryGetValue(Context.ConnectionId, out nombre); }
        if (nombre == null) return;

        bool esHost;
        lock (_lock) { esHost = _jugadores.TryGetValue(nombre, out var inf) && inf.EsHost; }
        if (!esHost) return;

        lock (_lock)
        {
            _formatoActual = formato;
            _juegoIniciado = true;
            // ── Reaplica config completa antes de generar tablas ──────────────
            _juego.ConfigurarPartida(_dobles, _tamañoTabla);
            _juego.ConfigurarFormato(ParseFormato(formato));
            _juego.IniciarPartida();
            foreach (var m in _marcas.Values) m.Clear();
        }

        foreach (var kvp in _jugadores)
        {
            var jugador = _juego.Jugadores.FirstOrDefault(j => j.Nombre == kvp.Key);
            if (jugador == null) continue;
            lock (_lock) { _juego.GenerarTablaParaJugador(jugador); }
            var casillas = TablaACasillas(jugador.Tabla);
            var connId = _idANombre.FirstOrDefault(x => x.Value == kvp.Key).Key;
            if (connId != null)
                await Clients.Client(connId).SendAsync("TablaAsignada", casillas);
        }

        await Clients.All.SendAsync("JuegoIniciado", formato);
    }

    // =========================================================================
    // CANTAR CARTA (solo host)
    // =========================================================================
    public async Task CantarCarta()
    {
        string? nombre;
        lock (_lock) { _idANombre.TryGetValue(Context.ConnectionId, out nombre); }
        if (nombre == null) return;

        bool esHost;
        lock (_lock) { esHost = _jugadores.TryGetValue(nombre, out var inf) && inf.EsHost; }
        if (!esHost) return;

        Carta? carta;
        lock (_lock) { carta = _juego.CantarCarta(); }

        if (carta == null)
        {
            lock (_lock) { _juego.IniciarPartidaSinTablas(); }
            await Clients.All.SendAsync("BarajaRebrajada");
            lock (_lock) { carta = _juego.CantarCarta(); }
        }

        if (carta != null)
            await Clients.All.SendAsync("CartaCantada", carta.Numero, carta.Nombre, carta.Frase);
    }

    // =========================================================================
    // TOGGLE MARCA
    // =========================================================================
    public async Task ToggleCarta(int numero)
    {
        string? nombre;
        lock (_lock) { _idANombre.TryGetValue(Context.ConnectionId, out nombre); }
        if (nombre == null) return;

        List<int> marcasActuales;
        lock (_lock)
        {
            if (!_marcas.TryGetValue(nombre, out var set)) return;
            if (!set.Remove(numero)) set.Add(numero);
            marcasActuales = set.ToList();

            var jugador = _juego.Jugadores.FirstOrDefault(j => j.Nombre == nombre);
            if (jugador != null)
            {
                jugador.LimpiarMarcas();
                foreach (var n in set) jugador.MarcarCarta(n);
            }
        }

        await Clients.Caller.SendAsync("MarcasActualizadas", marcasActuales);
    }

    // =========================================================================
    // RECLAMAR LOTERÍA
    // =========================================================================
    public async Task ReclamarLoteria()
    {
        string? nombre;
        lock (_lock) { _idANombre.TryGetValue(Context.ConnectionId, out nombre); }
        if (nombre == null) return;

        ResultadoValidacion resultado;
        lock (_lock)
        {
            var jugador = _juego.Jugadores.FirstOrDefault(j => j.Nombre == nombre);
            if (jugador == null) return;
            resultado = _juego.ValidarVictoria(jugador);
        }

        if (resultado.HayTrampa)
        {
            await Clients.Caller.SendAsync("Trampa", resultado.CartasTramposas.ToList());
        }
        else if (resultado.EsVictoria)
        {
            lock (_lock)
            {
                _juegoIniciado = false;
                var jugador = _juego.Jugadores.FirstOrDefault(j => j.Nombre == nombre);
                if (jugador != null) _juego.TerminarPartida(jugador);
            }
            await Clients.All.SendAsync("HayGanador", nombre);
            await NotificarJugadores();
        }
        else
        {
            await Clients.Caller.SendAsync("FalsaAlarma");
        }
    }

    // =========================================================================
    // PEDIR NUEVA TABLA
    // =========================================================================
    public async Task PedirNuevaTabla()
    {
        string? nombre;
        lock (_lock) { _idANombre.TryGetValue(Context.ConnectionId, out nombre); }
        if (nombre == null) return;

        List<CasillaDto> casillas;
        lock (_lock)
        {
            // ── Respeta el tamaño configurado por el host ─────────────────────
            _juego.ConfigurarPartida(_dobles, _tamañoTabla);

            var jugador = _juego.Jugadores.FirstOrDefault(j => j.Nombre == nombre);
            if (jugador == null) return;
            _juego.GenerarTablaParaJugador(jugador);
            casillas = TablaACasillas(jugador.Tabla);
            if (_marcas.TryGetValue(nombre, out var m)) m.Clear();
        }

        await Clients.Caller.SendAsync("TablaAsignada", casillas);
        await Clients.Caller.SendAsync("MarcasActualizadas", new List<int>());
    }

    // =========================================================================
    // REINICIAR BARAJA (solo host)
    // =========================================================================
    public async Task ReiniciarBaraja()
    {
        string? nombre;
        lock (_lock) { _idANombre.TryGetValue(Context.ConnectionId, out nombre); }
        if (nombre == null) return;

        bool esHost;
        lock (_lock) { esHost = _jugadores.TryGetValue(nombre, out var inf) && inf.EsHost; }
        if (!esHost) return;

        lock (_lock)
        {
            _juegoIniciado = false;
            _juego.IniciarPartidaSinTablas();
            foreach (var m in _marcas.Values) m.Clear();
        }

        await Clients.All.SendAsync("BarajaReiniciada");
    }

    // =========================================================================
    // CHAT
    // =========================================================================
    public async Task EnviarMensaje(string mensaje)
    {
        string? nombre;
        lock (_lock) { _idANombre.TryGetValue(Context.ConnectionId, out nombre); }
        if (nombre == null || string.IsNullOrWhiteSpace(mensaje)) return;
        await Clients.All.SendAsync("MensajeRecibido", nombre, mensaje.Trim());
    }

    // =========================================================================
    // HELPERS
    // =========================================================================
    private async Task NotificarJugadores()
    {
        List<JugadorDto> lista;
        lock (_lock)
        {
            lista = _jugadores.Values.Select(j => new JugadorDto(
                j.Nombre, j.EsHost, j.Victorias, j.EsHost)).ToList();
        }
        await Clients.All.SendAsync("JugadoresActualizados", lista);
    }

    private static List<CasillaDto> TablaACasillas(Tabla tabla)
    {
        var lista = new List<CasillaDto>();
        for (int f = 0; f < tabla.Filas; f++)
            for (int c = 0; c < tabla.Columnas; c++)
            {
                var carta = tabla.Casillas[f, c];
                if (carta != null)
                    lista.Add(new CasillaDto(carta.Numero, carta.Nombre, f, c));
            }
        return lista;
    }

    private static FormatoGanador ParseFormato(string fmt) => fmt switch
    {
        "FilaCompleta" => FormatoGanador.FilaCompleta,
        "ColumnaCompleta" => FormatoGanador.ColumnaCompleta,
        "DiagonalCompleta" => FormatoGanador.DiagonalCompleta,
        "EsquinasCompletas" => FormatoGanador.EsquinasCompletas,
        "CruzCentral" => FormatoGanador.CruzCentral,
        "FormaTee" => FormatoGanador.FormaTee,
        "MarcoCompleto" => FormatoGanador.MarcoCompleto,
        "MarcoInterior" => FormatoGanador.MarcoInterior,
        "DosDiagonales" => FormatoGanador.DosDiagonales,
        "PrimeraCarta" => FormatoGanador.PrimeraCarta,
        "TresFilas" => FormatoGanador.TresFilas,
        "TresColumnas" => FormatoGanador.TresColumnas,
        "TodasLasFormas" => FormatoGanador.TodasLasFormas,
        _ => FormatoGanador.TablaLlena
    };
}