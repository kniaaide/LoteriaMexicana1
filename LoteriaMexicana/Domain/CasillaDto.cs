namespace LoteriaMexicana.Domain;

/// <summary>
/// DTO para transferir una casilla de la tabla por SignalR.
/// </summary>
public record CasillaDto(int Numero, string Nombre, int Fila, int Columna);