using LoteriaMexicana.Domain;
using LoteriaMexicana.Domain.Enums;

namespace LoteriaMexicana.Services;

public class JuegoService
{
    // ── Tamaños de tabla soportados ───────────────────────────────────────────
    public enum TamañoTabla { Cuatro = 4, Cinco = 5 }

    // ── Configuración de partida ──────────────────────────────────────────────
    private int _filas;
    private int _columnas;

    public bool PermiteDobles { get; private set; } = false;
    public TamañoTabla Tamaño { get; private set; } = TamañoTabla.Cuatro;

    // ── Estado del juego ──────────────────────────────────────────────────────
    private readonly Baraja _baraja = new();
    private readonly List<Jugador> _jugadores = new();

    public bool JuegoActivo { get; private set; }
    public FormatoGanador Formato { get; private set; } = FormatoGanador.Ninguno;
    public Carta? UltimaCarta { get; private set; }
    public Jugador? Griton { get; private set; }

    public IReadOnlyList<Jugador> Jugadores => _jugadores.AsReadOnly();
    public IReadOnlyList<Carta> CartasCantadas => _baraja.CartasCantadas;
    public int CartasRestantes => _baraja.CartasRestantes;

    // ── Configuración antes de iniciar ────────────────────────────────────────
    public void ConfigurarDobles(bool permitir) => PermiteDobles = permitir;

    public void ConfigurarTamaño(TamañoTabla tamaño)
    {
        Tamaño = tamaño;
        _filas = (int)tamaño;
        _columnas = (int)tamaño;
    }

    public void ConfigurarPartida(bool dobles, TamañoTabla tamaño)
    {
        ConfigurarDobles(dobles);
        ConfigurarTamaño(tamaño);
    }

    public void ConfigurarFormato(FormatoGanador f) => Formato = f;

    // ── Jugadores ─────────────────────────────────────────────────────────────
    public void AgregarJugador(string nombre) => _jugadores.Add(new Jugador(nombre.Trim()));

    public void AsignarGriton(Jugador j)
    {
        if (Griton != null) Griton.EsGriton = false;
        j.EsGriton = true;
        Griton = j;
    }

    // ── Partida ───────────────────────────────────────────────────────────────
    public void IniciarPartida()
    {
        _baraja.Barajear();
        UltimaCarta = null;
        JuegoActivo = true;
        foreach (var j in _jugadores) j.LimpiarMarcas();
    }

    public void IniciarPartidaSinTablas()
    {
        _baraja.Barajear();
        UltimaCarta = null;
        JuegoActivo = true;
    }

    public Carta? CantarCarta()
    {
        if (!JuegoActivo) return null;
        return UltimaCarta = _baraja.SacarCarta();
    }

    public void TerminarPartida(Jugador ganador)
    {
        ganador.RegistrarVictoria();
        JuegoActivo = false;
    }

    // ── Tablas ────────────────────────────────────────────────────────────────
    public void GenerarTablaParaJugador(Jugador j)
        => j.AsignarTabla(
            Tabla.GenerarAleatoria(
                _baraja.ObtenerTodas(),
                _filas,
                _columnas,
                PermiteDobles));

    public void GenerarTodasLasTablas()
    {
        foreach (var j in _jugadores) GenerarTablaParaJugador(j);
    }

    /// <summary>Devuelve todas las cartas del mazo (para que el jugador elija).</summary>
    public IReadOnlyList<Carta> ObtenerTodasLasCartas() => _baraja.ObtenerTodas();

    // ── Marcas ────────────────────────────────────────────────────────────────
    public void MarcarCarta(Jugador j, int n) => j.MarcarCarta(n);
    public void DesmarcarCarta(Jugador j, int n) => j.DesmarcarCarta(n);

    // ── Validación ────────────────────────────────────────────────────────────
    public ResultadoValidacion ValidarVictoria(Jugador j)
    {
        var cantadasNums = _baraja.CartasCantadas.Select(c => c.Numero);

        var trampas = VictoriaValidador
            .DetectarTrampa(j.Tabla, j.CartasMarcadas, cantadasNums)
            .ToList();

        if (trampas.Count > 0)
            return ResultadoValidacion.Trampa(trampas);

        return VictoriaValidador.EsVictoria(j.Tabla, j.CartasMarcadas, Formato)
            ? ResultadoValidacion.Victoria()
            : ResultadoValidacion.FallaLimpia();
    }
}