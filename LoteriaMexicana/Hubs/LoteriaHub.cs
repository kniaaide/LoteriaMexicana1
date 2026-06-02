using LoteriaMexicana.Domain;
using LoteriaMexicana.Domain.Enums;
using LoteriaMexicana.Services;
using Microsoft.AspNetCore.SignalR;

namespace LoteriaMexicana.Hubs;

// DTO serializable para enviar casillas de la tabla por SignalR
public record CasillaDto(int Numero, string Nombre);

public class LoteriaHub : Hub
{
    // ── Estado global ─────────────────────────────────────────────────────────
    private static readonly object _lock = new();
    private static Baraja _baraja = new();
    private static bool _juegoIniciado = false;
    private static FormatoGanador _formatoActual = FormatoGanador.Ninguno;
    private static string? _hostConnectionId = null;

    private static readonly Dictionary<string, JugadorInfo> _jugadores = new();
    private static readonly Dictionary<string, Tabla>       _tablas    = new();
    private static readonly Dictionary<string, HashSet<int>> _marcas   = new();
    // Acumula TODOS los números cantados durante la partida (sobrevive reba rajadas)
    private static readonly HashSet<int> _cantadasEnPartida = new();

    // =========================================================================
    // DESCONEXIÓN
    // =========================================================================

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        string? nuevoHost = null;

        lock (_lock)
        {
            if (!_jugadores.ContainsKey(Context.ConnectionId))
                goto salir;

            bool eraHost = (_hostConnectionId == Context.ConnectionId);
            _jugadores.Remove(Context.ConnectionId);
            _tablas.Remove(Context.ConnectionId);
            _marcas.Remove(Context.ConnectionId);

            if (eraHost)
            {
                _hostConnectionId = _jugadores.Keys.FirstOrDefault();
                if (_hostConnectionId != null)
                {
                    _jugadores[_hostConnectionId].EsHost = true;
                    nuevoHost = _hostConnectionId;
                }
                else
                {
                    // FIX Bug#3: sala vacía → reiniciar TODO el estado estático
                    // para que una nueva sesión arranque limpia sin jugadores
                    // "fantasmas" de partidas anteriores.
                    _juegoIniciado       = false;
                    _formatoActual       = FormatoGanador.Ninguno;
                    _baraja              = new Baraja();
                    _hostConnectionId    = null;
                    _cantadasEnPartida.Clear();
                    // Las colecciones ya están vacías (se limpió el último jugador arriba),
                    // pero hacemos Clear explícito por si acaso.
                    _jugadores.Clear();
                    _tablas.Clear();
                    _marcas.Clear();
                }
            }
        }
        salir:

        if (nuevoHost != null)
            await Clients.Client(nuevoHost).SendAsync("RolesActualizados", true);

        await EnviarEstadoJugadores();
        await base.OnDisconnectedAsync(exception);
    }

    // =========================================================================
    // UNIRSE AL JUEGO
    // =========================================================================

    public async Task UnirseAlJuego(string nombreJugador)
    {
        var nombre = string.IsNullOrWhiteSpace(nombreJugador) ? "Jugador Anónimo" : nombreJugador.Trim();

        bool esHost;
        List<CasillaDto> casillas;
        bool partidaEnCurso;
        List<Carta> cantadas;
        FormatoGanador formato;

        lock (_lock)
        {
            esHost = _jugadores.Count == 0;
            if (esHost) _hostConnectionId = Context.ConnectionId;

            _jugadores[Context.ConnectionId] = new JugadorInfo
            {
                Nombre = nombre,
                EsHost = esHost,
                Listo  = false
            };
            _marcas[Context.ConnectionId] = new HashSet<int>();

            var tabla = Tabla.GenerarAleatoria(_baraja.ObtenerTodas());
            _tablas[Context.ConnectionId] = tabla;
            casillas = TablaACasillas(tabla);

            partidaEnCurso = _juegoIniciado;
            cantadas       = _baraja.CartasCantadas.ToList();
            formato        = _formatoActual;
        }

        await Clients.Caller.SendAsync("RolesActualizados", esHost);
        await Clients.Caller.SendAsync("TablaAsignada", casillas);

        if (partidaEnCurso && cantadas.Any())
        {
            await Clients.Caller.SendAsync("JuegoYaIniciado", formato.ToString(), casillas);
            foreach (var carta in cantadas)
                await Clients.Caller.SendAsync("CartaCantada", carta.Numero, carta.Nombre, carta.Frase);
        }

        await EnviarEstadoJugadores();
    }

    // =========================================================================
    // INICIAR JUEGO — solo el host
    // =========================================================================

    public async Task IniciarJuego(string formatoStr)
    {
        lock (_lock)
        {
            if (Context.ConnectionId != _hostConnectionId)
                throw new HubException("Solo el Gritón puede iniciar la partida.");
        }

        if (!Enum.TryParse<FormatoGanador>(formatoStr, out var formato))
            formato = FormatoGanador.TablaLlena;

        Dictionary<string, List<CasillaDto>> tablasParaEnviar;

        lock (_lock)
        {
            _baraja        = new Baraja();
            _baraja.Barajear();
            _juegoIniciado = true;
            _formatoActual = formato;

            tablasParaEnviar = new Dictionary<string, List<CasillaDto>>();
            _cantadasEnPartida.Clear();
            foreach (var connId in _jugadores.Keys.ToList())
            {
                _marcas[connId] = new HashSet<int>();
                // Conservar la tabla que el jugador ya eligió; solo regenerar si no tiene una
                if (!_tablas.ContainsKey(connId))
                    _tablas[connId] = Tabla.GenerarAleatoria(_baraja.ObtenerTodas());
                tablasParaEnviar[connId] = TablaACasillas(_tablas[connId]);
            }
        }

        foreach (var (connId, casillas) in tablasParaEnviar)
            await Clients.Client(connId).SendAsync("TablaAsignada", casillas);

        await Clients.All.SendAsync("JuegoIniciado", formatoStr);
    }

    // =========================================================================
    // CANTAR CARTA — solo el host
    // =========================================================================

    public async Task CantarCarta()
    {
        lock (_lock)
        {
            if (Context.ConnectionId != _hostConnectionId)
                throw new HubException("Solo el Gritón puede cantar cartas.");
            if (!_juegoIniciado)
                throw new HubException("El juego no ha iniciado.");
        }

        Carta? carta;
        bool rebarajo = false;
        lock (_lock)
        {
            if (!_baraja.TieneCartas)
            {
                // La baraja se agotó pero la partida SIGUE — rebaraja y continúa
                _baraja.Barajear();
                rebarajo = true;
            }
            carta = _baraja.SacarCarta();
        }

        if (rebarajo)
            await Clients.All.SendAsync("BarajaRebrajada"); // aviso informativo, no termina la partida

        if (carta != null)
        {
            lock (_lock) { _cantadasEnPartida.Add(carta.Numero); }
            await Clients.All.SendAsync("CartaCantada", carta.Numero, carta.Nombre, carta.Frase);
        }
    }

    // =========================================================================
    // REINICIAR BARAJA
    // =========================================================================

    public async Task ReiniciarBaraja()
    {
        lock (_lock)
        {
            if (Context.ConnectionId != _hostConnectionId)
                throw new HubException("Solo el Gritón puede reiniciar la baraja.");
        }
        await ReiniciarBarajaInterno();
    }

    private async Task ReiniciarBarajaInterno()
    {
        lock (_lock)
        {
            _baraja = new Baraja();
            _baraja.Barajear();
            _cantadasEnPartida.Clear();
            // NO se limpian _marcas: el jugador conserva sus fichas entre vueltas
        }
        await Clients.All.SendAsync("BarajaReiniciada");
    }

    // =========================================================================
    // TOGGLE CARTA
    // =========================================================================

    public async Task ToggleCarta(int numero)
    {
        List<int> marcasActuales;
        lock (_lock)
        {
            if (!_marcas.TryGetValue(Context.ConnectionId, out var set)) return;
            if (!_cantadasEnPartida.Contains(numero)) return;

            if (!set.Remove(numero)) set.Add(numero);
            marcasActuales = set.ToList();
        }

        await Clients.Caller.SendAsync("MarcasActualizadas", marcasActuales);
    }

    // =========================================================================
    // RECLAMAR LOTERÍA
    // =========================================================================

    public async Task ReclamarLoteria()
    {
        string?    nombreGanador = null;
        List<int>? trampas       = null;

        lock (_lock)
        {
            if (!_juegoIniciado) return;
            if (!_jugadores.TryGetValue(Context.ConnectionId, out var jugador)) return;
            if (!_tablas.TryGetValue(Context.ConnectionId, out var tabla))      return;
            if (!_marcas.TryGetValue(Context.ConnectionId, out var marcas))     return;

            var marcadasSet     = (IReadOnlySet<int>)marcas;
            var trampaDetectada = VictoriaValidador
                .DetectarTrampa(marcadasSet, _cantadasEnPartida)
                .ToList();

            if (trampaDetectada.Any())
            {
                trampas = trampaDetectada;
            }
            else if (VictoriaValidador.EsVictoria(tabla, marcadasSet, _formatoActual))
            {
                nombreGanador = jugador.Nombre;
                jugador.Victorias++;
                _juegoIniciado = false;
            }
        }

        if (trampas != null)
            await Clients.Caller.SendAsync("Trampa", trampas);
        else if (nombreGanador != null)
        {
            await Clients.All.SendAsync("HayGanador", nombreGanador);
            await EnviarEstadoJugadores();
            // Auto-reiniciar baraja e historial tras el ganador
            await ReiniciarBarajaInterno();
        }
        else
            await Clients.Caller.SendAsync("FalsaAlarma");
    }

    // =========================================================================
    // NUEVA TABLA — cualquier jugador puede pedir una tabla diferente
    // =========================================================================

    public async Task PedirNuevaTabla()
    {
        List<CasillaDto> casillas;
        lock (_lock)
        {
            if (!_jugadores.ContainsKey(Context.ConnectionId)) return;

            // Generar nueva tabla aleatoria y resetear marcas del jugador
            var tabla = Tabla.GenerarAleatoria(_baraja.ObtenerTodas());
            _tablas[Context.ConnectionId] = tabla;
            _marcas[Context.ConnectionId] = new HashSet<int>();
            casillas = TablaACasillas(tabla);
        }
        await Clients.Caller.SendAsync("TablaAsignada", casillas);
    }

    // =========================================================================
    // CHAT
    // =========================================================================

    public async Task EnviarMensaje(string mensaje)
    {
        if (string.IsNullOrWhiteSpace(mensaje)) return;
        string nombre = _jugadores.TryGetValue(Context.ConnectionId, out var j)
            ? j.Nombre : "Desconocido";
        await Clients.All.SendAsync("MensajeRecibido", nombre, mensaje.Trim());
    }

    // =========================================================================
    // HELPERS
    // =========================================================================

    private async Task EnviarEstadoJugadores()
    {
        List<JugadorDto> lista;
        lock (_lock)
        {
            lista = _jugadores.Values
                .Select(j => new JugadorDto(j.Nombre, j.EsHost, j.Victorias))
                .ToList();
        }
        await Clients.All.SendAsync("JugadoresActualizados", lista);
    }

    private static List<CasillaDto> TablaACasillas(Tabla tabla)
    {
        var lista = new List<CasillaDto>();
        for (int f = 0; f < Tabla.Filas; f++)
            for (int c = 0; c < Tabla.Columnas; c++)
            {
                var carta = tabla.Casillas[f, c];
                lista.Add(new CasillaDto(carta.Numero, carta.Nombre));
            }
        return lista;
    }
}
