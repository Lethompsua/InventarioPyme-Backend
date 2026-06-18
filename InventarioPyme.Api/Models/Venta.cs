namespace InventarioPyme.Api.Models;

public class Venta
{
    public Guid Id { get; set; }
    public Guid UsuarioId { get; set; }
    public string Folio { get; set; } = string.Empty;
    public decimal Subtotal { get; set; }
    public decimal Iva { get; set; }
    public decimal Total { get; set; }
    public string Estatus { get; set; } = string.Empty;
    public DateTime Fecha { get; set; }
    public Usuario Usuario { get; set; } = null!;
    public ICollection<VentaDetalle> Detalles { get; set; } = new List<VentaDetalle>();
}
