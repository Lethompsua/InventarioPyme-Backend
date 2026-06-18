namespace InventarioPyme.Api.DTOs.Ventas;

public record VentaDetalleResponse(
    Guid ProductoId,
    string Producto,
    int Cantidad,
    decimal PrecioUnitario,
    decimal Subtotal
);

public record VentaResponse(
    Guid Id,
    string Folio,
    string Usuario,
    decimal Subtotal,
    decimal Iva,
    decimal Total,
    string Estatus,
    DateTime Fecha,
    IEnumerable<VentaDetalleResponse>? Detalles
);
