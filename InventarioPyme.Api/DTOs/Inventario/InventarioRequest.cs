namespace InventarioPyme.Api.DTOs.Inventario;

public record EntradaInventarioRequest(
    Guid ProductoId,
    int Cantidad,
    decimal PrecioUnitario,
    string? Referencia,
    string? Notas
);

public record AjusteInventarioRequest(
    Guid ProductoId,
    int NuevoStock,
    string? Notas
);
