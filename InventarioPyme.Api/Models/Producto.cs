namespace InventarioPyme.Api.Models;

public class Producto
{
    public Guid Id { get; set; }
    public Guid CategoriaId { get; set; }
    public Guid? ProveedorId { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Sku { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public decimal PrecioCompra { get; set; }
    public decimal PrecioVenta { get; set; }
    public int StockActual { get; set; }
    public int StockMinimo { get; set; }
    public string Unidad { get; set; } = string.Empty;
    public bool Activo { get; set; }
    public DateTime UpdatedAt { get; set; }
    public Categoria Categoria { get; set; } = null!;
    public Proveedor? Proveedor { get; set; }
}
