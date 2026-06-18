namespace InventarioPyme.Api.DTOs.Productos;

public record ProductoResponse(
    Guid Id,
    string Nombre,
    string Sku,
    string? Descripcion,
    decimal PrecioCompra,
    decimal PrecioVenta,
    int StockActual,
    int StockMinimo,
    string Unidad,
    bool Activo,
    bool StockBajo,
    string Categoria,
    string? Proveedor,
    DateTime UpdatedAt
);
