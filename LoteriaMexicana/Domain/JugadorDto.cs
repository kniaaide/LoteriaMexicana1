namespace LoteriaMexicana.Domain;

/// DTO para transferir datos de jugador por SignalR
public record JugadorDto(string Nombre, bool EsGriton, int Victorias, bool EsHost = false);