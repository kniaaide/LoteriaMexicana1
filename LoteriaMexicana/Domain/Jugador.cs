namespace LoteriaMexicana.Domain;

public class Jugador
{
    public string Nombre { get; }
    public Tabla Tabla { get; private set; } = Tabla.Vacia();
    public int Victorias { get; private set; }
    public bool EsGriton { get; set; }
    private readonly HashSet<int> _marcadas = new();
    public IReadOnlySet<int> CartasMarcadas => _marcadas;
    public Jugador(string nombre) { Nombre = nombre; }
    public void AsignarTabla(Tabla t) => Tabla = t;
    public void MarcarCarta(int n) => _marcadas.Add(n);
    public void DesmarcarCarta(int n) => _marcadas.Remove(n);
    public void LimpiarMarcas() => _marcadas.Clear();
    public void RegistrarVictoria() => Victorias++;
    public override string ToString() => Nombre;
}
