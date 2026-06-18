namespace InventarioPyme.Api.Models;

public class Proveedor
{
    public Guid Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Contacto { get; set; }
    public string? Telefono { get; set; }
    public string? Rfc { get; set; }
    public DateTime CreatedAt { get; set; }
    public ICollection<Producto> Productos { get; set; } = new List<Producto>();
}
