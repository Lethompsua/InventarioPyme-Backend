namespace InventarioPyme.Api.DTOs.Productos;

public record ProductoCreateRequest(
    Guid CategoriaId,
    Guid? ProveedorId,
    string Nombre,
    string Sku,
    string? Descripcion,
    decimal PrecioCompra,
    decimal PrecioVenta,
    int StockMinimo,
    string Unidad
);

public record ProductoUpdateRequest(
    Guid? CategoriaId,
    Guid? ProveedorId,
    string? Nombre,
    string? Sku,
    string? Descripcion,
    decimal? PrecioCompra,
    decimal? PrecioVenta,
    int? StockMinimo,
    string? Unidad
);
