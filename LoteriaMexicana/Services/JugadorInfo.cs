namespace LoteriaMexicana.Services
{
    public class JugadorInfo
    {
        public string Nombre { get; set; } = "";
        public bool Listo { get; set; } = false;
        public bool EsHost { get; set; } = false;
        public int Victorias { get; set; } = 0;
    }
}
