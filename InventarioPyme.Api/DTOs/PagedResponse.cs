namespace InventarioPyme.Api.DTOs;

public record PagedResponse<T>(
    IEnumerable<T> Items,
    int Total,
    int Pagina,
    int TamanioPagina
);
