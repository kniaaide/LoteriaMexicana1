namespace LoteriaMexicana.Domain;

public class HistorialCartas
{
    private readonly List<Carta> _cartas = new();
    private int _indiceActual = -1;

    public Carta? CartaActual => _indiceActual >= 0 && _indiceActual < _cartas.Count ? _cartas[_indiceActual] : null;
    public bool PuedoRetroceder => _indiceActual > 0;
    public bool PuedoAvanzar => _indiceActual < _cartas.Count - 1;
    public int Total => _cartas.Count;
    public int PosicionActual => _indiceActual + 1;

    public void AgregarCarta(Carta carta)
    {
        // Si estamos en el medio del historial, eliminar todo lo que viene después
        if (_indiceActual >= 0 && _indiceActual < _cartas.Count - 1)
            _cartas.RemoveRange(_indiceActual + 1, _cartas.Count - _indiceActual - 1);

        _cartas.Add(carta);
        _indiceActual = _cartas.Count - 1;
    }

    public Carta? Retroceder()
    {
        if (PuedoRetroceder)
        {
            _indiceActual--;
            return CartaActual;
        }
        return null;
    }

    public Carta? Avanzar()
    {
        if (PuedoAvanzar)
        {
            _indiceActual++;
            return CartaActual;
        }
        return null;
    }

    public List<Carta> ObtenerTodas() => new(_cartas);

    public void Limpiar()
    {
        _cartas.Clear();
        _indiceActual = -1;
    }
}
