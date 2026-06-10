using LoteriaMexicana.Domain;
using LoteriaMexicana.Domain.Enums;
using LoteriaMexicana.Services;

namespace LoteriaMexicana;

public class LoteriaHub
{
    private Tabla _tablaActual;
    private HistorialCartas _historial;
    private ConfiguracionJuego _config;
    private GestorPuntajes _puntajes;
    private ServicioDesempate _desempate;
    private ValidacionCartas _validacion;
    private List<Carta> _cartasDisponibles;
    private HashSet<int> _cartasCantadas;
    private Dictionary<string, bool> _fichasActivas; // Para desempate
    private List<string> _ganadoresActuales;

    public LoteriaHub(List<Carta> cartasDisponibles)
    {
        _cartasDisponibles = cartasDisponibles;
        _historial = new HistorialCartas();
        _config = new ConfiguracionJuego();
        _puntajes = new GestorPuntajes();
        _desempate = new ServicioDesempate();
        _validacion = new ValidacionCartas();
        _cartasCantadas = new HashSet<int>();
        _fichasActivas = new Dictionary<string, bool>();
        _ganadoresActuales = new List<string>();
        
        GenerarTabla();
    }

    // ============ Configuración ============
    public ConfiguracionJuego ObtenerConfiguracion() => _config;

    public void ActualizarConfiguracion(ConfiguracionJuego nuevaConfig)
    {
        _config = nuevaConfig ?? throw new ArgumentNullException(nameof(nuevaConfig));
        GenerarTabla();
    }

    // ============ Tabla y Generación ============
    public Tabla ObtenerTabla() => _tablaActual;

    public void GenerarTabla()
    {
        _tablaActual = Tabla.GenerarAleatoria(_cartasDisponibles, _config.TamañoTabla, _config.PermitirCartasDobles);
        _cartasCantadas.Clear();
        _historial.Limpiar();
        _ganadoresActuales.Clear();
    }

    public void GenerarTablaPersonalizada(Tabla tabla)
    {
        if (tabla == null)
            throw new ArgumentNullException(nameof(tabla));
        
        _tablaActual = tabla;
        _cartasCantadas.Clear();
        _historial.Limpiar();
        _ganadoresActuales.Clear();
    }

    // ============ Cartas Cantadas ============
    public void CantarCarta(Carta carta)
    {
        if (carta == null)
            throw new ArgumentNullException(nameof(carta));

        if (!_cartasCantadas.Contains(carta.Numero))
        {
            _cartasCantadas.Add(carta.Numero);
            _historial.AgregarCarta(carta);
        }
    }

    public HashSet<int> ObtenerCartasCantadas() => new(_cartasCantadas);

    public Carta? ObtenerCartaActual() => _historial.CartaActual;

    public bool PuedoRetroceder() => _historial.PuedoRetroceder;

    public bool PuedoAvanzar() => _historial.PuedoAvanzar;

    public Carta? RetrocederHistorial()
    {
        return _historial.Retroceder();
    }

    public Carta? AvanzarHistorial()
    {
        return _historial.Avanzar();
    }

    public int ObtenerPosicionHistorial() => _historial.PosicionActual;

    public int ObtenerTotalCartasCantadas() => _historial.Total;

    // ============ Validación de Cartas ============
    public List<int> ObtenerCartasInvalidas(HashSet<int> cartasColocadas)
    {
        return ValidacionCartas.ObtenerCartasInvalidas(cartasColocadas, _cartasCantadas);
    }

    public List<int> ObtenerCartasValidas(HashSet<int> cartasColocadas)
    {
        return ValidacionCartas.ObtenerCartasValidas(cartasColocadas, _cartasCantadas);
    }

    public bool EsCartaValida(int numeroCarta)
    {
        return ValidacionCartas.EsValida(numeroCarta, _cartasCantadas);
    }

    // ============ Verificación de Ganancias ============
    public bool VerificarGanancia(HashSet<int> cartasColocadas, FormatoGanador formato)
    {
        // Aquí iría la lógica de verificación del formato
        // Por ahora retorna false como placeholder
        return false;
    }

    public List<FormatoGanador> ObtenerFormatosActivos() => new(_config.FormatosActivos);

    // ============ Desempate ============
    public void RegistrarGanador(string nombreJugador)
    {
        if (!_ganadoresActuales.Contains(nombreJugador))
            _ganadoresActuales.Add(nombreJugador);

        _puntajes.RegistrarVictoria(nombreJugador);
    }

    public List<string> ObtenerGanadoresRonda() => new(_ganadoresActuales);

    public bool HayDesempate() => _desempate.HayDesempate(_ganadoresActuales);

    public string ObtenerMensajeDesempate()
    {
        return _desempate.ObtenerMensajeDesempate(_ganadoresActuales);
    }

    public void ActivarFichasParaDesempate(List<string> todosLosJugadores)
    {
        _fichasActivas = _desempate.GenerarFichasActivas(_ganadoresActuales, todosLosJugadores);
    }

    public bool EstaJugadorActivo(string nombre)
    {
        if (_fichasActivas.Count == 0)
            return true; // Si no hay desempate, todos están activos
        
        return _fichasActivas.ContainsKey(nombre) && _fichasActivas[nombre];
    }

    public Dictionary<string, bool> ObtenerFichasActivas() => new(_fichasActivas);

    // ============ Puntajes ============
    public void AgregarJugador(string nombre)
    {
        _puntajes.AgregarJugador(nombre);
    }

    public int ObtenerPuntaje(string nombre)
    {
        return _puntajes.ObtenerPuntaje(nombre);
    }

    public Dictionary<string, int> ObtenerTodosPuntajes()
    {
        return _puntajes.ObtenerTodosPuntajes();
    }

    public List<(string Nombre, int Puntaje)> ObtenerRanking()
    {
        return _puntajes.ObtenerRanking();
    }

    public string ObtenerResumenPuntajes()
    {
        return _puntajes.ObtenerResumen();
    }

    public void RegistrarRonda()
    {
        _puntajes.RegistrarRonda();
        _ganadoresActuales.Clear();
        _fichasActivas.Clear();
    }

    public void ReiniciarPuntajes()
    {
        _puntajes.ReiniciarPuntajes();
    }

    // ============ Información General ============
    public string ObtenerInfoJuego()
    {
        return $"Tabla: {_config.TamañoTabla}x{_config.TamañoTabla} | " +
               $"Cartas cantadas: {_cartasCantadas.Count} | " +
               $"Jugadores: {_puntajes.ObtenerTodosPuntajes().Count}";
    }
}
