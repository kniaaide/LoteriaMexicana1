using LoteriaMexicana.Domain;

namespace LoteriaMexicana.Services;

public class ServicioMultijugador
{
    private List<string> _jugadores;
    private int _jugadorActualIndex;
    private Dictionary<string, Tabla> _tablasJugadores;
    private Dictionary<string, HashSet<int>> _cartasColocadas;
    private Dictionary<string, bool> _jugadoresActivos;

    public ServicioMultijugador()
    {
        _jugadores = new List<string>();
        _jugadorActualIndex = 0;
        _tablasJugadores = new Dictionary<string, Tabla>();
        _cartasColocadas = new Dictionary<string, HashSet<int>>();
        _jugadoresActivos = new Dictionary<string, bool>();
    }

    // ============ Gestión de Jugadores ============
    public void AgregarJugador(string nombre)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            throw new ArgumentException("El nombre no puede estar vacío", nameof(nombre));

        if (_jugadores.Contains(nombre))
            throw new InvalidOperationException($"El jugador {nombre} ya existe");

        _jugadores.Add(nombre);
        _cartasColocadas[nombre] = new HashSet<int>();
        _jugadoresActivos[nombre] = true;
    }

    public void EliminarJugador(string nombre)
    {
        if (_jugadores.Contains(nombre))
        {
            _jugadores.Remove(nombre);
            _cartasColocadas.Remove(nombre);
            _tablasJugadores.Remove(nombre);
            _jugadoresActivos.Remove(nombre);
        }
    }

    public List<string> ObtenerJugadores() => new(_jugadores);

    public int ObtenerCantidadJugadores() => _jugadores.Count;

    public bool ExisteJugador(string nombre) => _jugadores.Contains(nombre);

    // ============ Tablas Personalizadas ============
    public void AsignarTabla(string nombreJugador, Tabla tabla)
    {
        if (!_jugadores.Contains(nombreJugador))
            throw new ArgumentException($"El jugador {nombreJugador} no existe");

        _tablasJugadores[nombreJugador] = tabla;
    }

    public Tabla ObtenerTabla(string nombreJugador)
    {
        if (_tablasJugadores.ContainsKey(nombreJugador))
            return _tablasJugadores[nombreJugador];

        return null;
    }

    public bool TieneTablaBersonalizada(string nombreJugador)
    {
        return _tablasJugadores.ContainsKey(nombreJugador) && _tablasJugadores[nombreJugador] != null;
    }

    // ============ Cartas Colocadas ============
    public void ColocarCarta(string nombreJugador, int numeroCarta)
    {
        if (!_jugadores.Contains(nombreJugador))
            throw new ArgumentException($"El jugador {nombreJugador} no existe");

        _cartasColocadas[nombreJugador].Add(numeroCarta);
    }

    public void RemoverCarta(string nombreJugador, int numeroCarta)
    {
        if (_cartasColocadas.ContainsKey(nombreJugador))
            _cartasColocadas[nombreJugador].Remove(numeroCarta);
    }

    public HashSet<int> ObtenerCartasColocadas(string nombreJugador)
    {
        if (_cartasColocadas.ContainsKey(nombreJugador))
            return new HashSet<int>(_cartasColocadas[nombreJugador]);

        return new HashSet<int>();
    }

    public void LimpiarCartasColocadas(string nombreJugador)
    {
        if (_cartasColocadas.ContainsKey(nombreJugador))
            _cartasColocadas[nombreJugador].Clear();
    }

    public void LimpiarCartasTodosJugadores()
    {
        foreach (var jugador in _jugadores)
        {
            _cartasColocadas[jugador].Clear();
        }
    }

    // ============ Estado de Jugadores ============
    public void DesactivarJugador(string nombreJugador)
    {
        if (_jugadoresActivos.ContainsKey(nombreJugador))
            _jugadoresActivos[nombreJugador] = false;
    }

    public void ActivarJugador(string nombreJugador)
    {
        if (_jugadoresActivos.ContainsKey(nombreJugador))
            _jugadoresActivos[nombreJugador] = true;
    }

    public bool EstaJugadorActivo(string nombreJugador)
    {
        return _jugadoresActivos.ContainsKey(nombreJugador) && _jugadoresActivos[nombreJugador];
    }

    public List<string> ObtenerJugadoresActivos()
    {
        return _jugadores.Where(j => EstaJugadorActivo(j)).ToList();
    }

    public List<string> ObtenerJugadoresInactivos()
    {
        return _jugadores.Where(j => !EstaJugadorActivo(j)).ToList();
    }

    public int ObtenerCantidadJugadoresActivos()
    {
        return ObtenerJugadoresActivos().Count;
    }

    public void ReiniciarEstadoJugadores()
    {
        foreach (var jugador in _jugadores)
        {
            _jugadoresActivos[jugador] = true;
        }
    }

    // ============ Turnos (Opcional para futura expansión) ============
    public string ObtenerJugadorActual()
    {
        if (_jugadores.Count == 0)
            return null;

        return _jugadores[_jugadorActualIndex];
    }

    public void AvanzarTurno()
    {
        if (_jugadores.Count > 0)
        {
            _jugadorActualIndex = (_jugadorActualIndex + 1) % _jugadores.Count;
        }
    }

    public void ReiniciarTurno()
    {
        _jugadorActualIndex = 0;
    }

    // ============ Información General ============
    public string ObtenerInfoJugadores()
    {
        if (_jugadores.Count == 0)
            return "No hay jugadores registrados";

        var activos = ObtenerJugadoresActivos();
        var inactivos = ObtenerJugadoresInactivos();

        var sb = new System.Text.StringBuilder();
        sb.AppendLine($"Total jugadores: {_jugadores.Count}");
        sb.AppendLine($"Activos: {activos.Count}");
        sb.AppendLine($"Inactivos: {inactivos.Count}");

        return sb.ToString();
    }

    public void Reiniciar()
    {
        _jugadores.Clear();
        _jugadorActualIndex = 0;
        _tablasJugadores.Clear();
        _cartasColocadas.Clear();
        _jugadoresActivos.Clear();
    }
}
