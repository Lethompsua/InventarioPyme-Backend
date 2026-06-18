namespace InventarioPyme.Api.Models;

public class Categoria
{
    public Guid Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public bool Activa { get; set; }
    public ICollection<Producto> Productos { get; set; } = new List<Producto>();
}
