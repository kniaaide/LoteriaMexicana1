using System.Linq;
using LoteriaMexicana.Domain;
using LoteriaMexicana.Domain.Enums;

namespace LoteriaMexicana.Services;

public class JuegoService
{
    private readonly Baraja _baraja = new();
    private readonly List<Jugador> _jugadores = new();

    public bool JuegoActivo { get; private set; }
    public FormatoGanador Formato { get; private set; } = FormatoGanador.Ninguno;
    public Carta? UltimaCarta { get; private set; }
    public Jugador? Griton { get; private set; }
    public IReadOnlyList<Jugador> Jugadores => _jugadores.AsReadOnly();
    public IReadOnlyList<Carta> CartasCantadas => _baraja.CartasCantadas;
    public int CartasRestantes => _baraja.CartasRestantes;

    public void AgregarJugador(string nombre) => _jugadores.Add(new Jugador(nombre.Trim()));

    public void AsignarGriton(Jugador j)
    {
        if (Griton != null) Griton.EsGriton = false;
        j.EsGriton = true; Griton = j;
    }

    public void ConfigurarFormato(FormatoGanador f) => Formato = f;

    public void IniciarPartida()
    {
        _baraja.Barajear(); UltimaCarta = null; JuegoActivo = true;
        foreach (var j in _jugadores) j.LimpiarMarcas();
    }

    public Carta? CantarCarta()
    {
        if (!JuegoActivo) return null;
        return UltimaCarta = _baraja.SacarCarta();
    }

    public void GenerarTablaParaJugador(Jugador j)
        => j.AsignarTabla(Tabla.GenerarAleatoria(_baraja.ObtenerTodas()));

    public void GenerarTodasLasTablas()
    {
        foreach (var j in _jugadores) GenerarTablaParaJugador(j);
    }

    public void MarcarCarta(Jugador j, int n) => j.MarcarCarta(n);
    public void DesmarcarCarta(Jugador j, int n) => j.DesmarcarCarta(n);

    public ResultadoValidacion ValidarVictoria(Jugador j)
    {
        var trampas = VictoriaValidador.DetectarTrampa(j.CartasMarcadas, _baraja.CartasCantadas.Select(c => c.Numero)).ToList();
        if (trampas.Count > 0) return ResultadoValidacion.Trampa(trampas);
        return VictoriaValidador.EsVictoria(j.Tabla, j.CartasMarcadas, Formato)
            ? ResultadoValidacion.Victoria()
            : ResultadoValidacion.FallaLimpia();
    }

    // Solo baraja y estado de juego - las tablas y marcas las maneja el Hub
    public void IniciarPartidaSinTablas()
    {
        _baraja.Barajear(); UltimaCarta = null; JuegoActivo = true;
    }

    public void TerminarPartida(Jugador ganador)
    {
        ganador.RegistrarVictoria(); JuegoActivo = false;
    }
}
