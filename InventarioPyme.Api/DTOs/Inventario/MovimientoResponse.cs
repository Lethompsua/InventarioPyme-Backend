namespace InventarioPyme.Api.DTOs.Inventario;

public record MovimientoResponse(
    Guid Id,
    string Producto,
    string Usuario,
    string Tipo,
    int Cantidad,
    decimal PrecioUnitario,
    string? Referencia,
    string? Notas,
    DateTime Fecha
);
