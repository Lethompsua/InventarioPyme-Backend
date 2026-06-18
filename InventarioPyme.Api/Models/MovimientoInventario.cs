namespace InventarioPyme.Api.Models;

public class MovimientoInventario
{
    public Guid Id { get; set; }
    public Guid ProductoId { get; set; }
    public Guid UsuarioId { get; set; }
    public string Tipo { get; set; } = string.Empty;
    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
    public string? Referencia { get; set; }
    public string? Notas { get; set; }
    public DateTime Fecha { get; set; }
    public Producto Producto { get; set; } = null!;
    public Usuario Usuario { get; set; } = null!;
}
