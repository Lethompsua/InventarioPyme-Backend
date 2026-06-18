namespace InventarioPyme.Api.DTOs.Ventas;

public record VentaDetalleRequest(
    Guid ProductoId,
    int Cantidad,
    decimal PrecioUnitario
);

public record VentaCreateRequest(IEnumerable<VentaDetalleRequest> Detalles);
