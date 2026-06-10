namespace LoteriaMexicana.Services;

public class GestorPuntajes
{
    private Dictionary<string, int> _puntajes = new();
    private int _rondasJugadas = 0;

    public GestorPuntajes()
    {
        Inicializar();
    }

    public void Inicializar()
    {
        _puntajes.Clear();
        _rondasJugadas = 0;
    }

    public void AgregarJugador(string nombre)
    {
        if (!_puntajes.ContainsKey(nombre))
            _puntajes[nombre] = 0;
    }

    public void RegistrarVictoria(string nombre)
    {
        if (_puntajes.ContainsKey(nombre))
            _puntajes[nombre]++;
    }

    public void RegistrarRonda()
    {
        _rondasJugadas++;
    }

    public int ObtenerPuntaje(string nombre)
    {
        return _puntajes.ContainsKey(nombre) ? _puntajes[nombre] : 0;
    }

    public Dictionary<string, int> ObtenerTodosPuntajes()
    {
        return new Dictionary<string, int>(_puntajes);
    }

    public List<(string Nombre, int Puntaje)> ObtenerRanking()
    {
        return _puntajes
            .OrderByDescending(kv => kv.Value)
            .Select(kv => (kv.Key, kv.Value))
            .ToList();
    }

    public int RondasJugadas => _rondasJugadas;

    public string ObtenerResumen()
    {
        var ranking = ObtenerRanking();
        if (ranking.Count == 0)
            return "No hay datos de puntajes.";

        var sb = new System.Text.StringBuilder();
        sb.AppendLine($"📊 Puntajes después de {_rondasJugadas} ronda(s):");
        sb.AppendLine();

        int posicion = 1;
        foreach (var (nombre, puntaje) in ranking)
        {
            string medalla = posicion switch
            {
                1 => "🥇",
                2 => "🥈",
                3 => "🥉",
                _ => $"#{posicion}"
            };
            sb.AppendLine($"{medalla} {nombre}: {puntaje} victoria{(puntaje != 1 ? "s" : "")}");
            posicion++;
        }

        return sb.ToString();
    }

    public void ReiniciarPuntajes()
    {
        _puntajes.Clear();
        _rondasJugadas = 0;
    }
}
