using InventarioPyme.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace InventarioPyme.Api.Data;

public class InventarioDbContext : DbContext
{
    public InventarioDbContext(DbContextOptions<InventarioDbContext> options) : base(options) { }

    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Categoria> Categorias => Set<Categoria>();
    public DbSet<Proveedor> Proveedores => Set<Proveedor>();
    public DbSet<Producto> Productos => Set<Producto>();
    public DbSet<MovimientoInventario> MovimientosInventario => Set<MovimientoInventario>();
    public DbSet<Venta> Ventas => Set<Venta>();
    public DbSet<VentaDetalle> VentaDetalles => Set<VentaDetalle>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Usuario>(e =>
        {
            e.ToTable("usuarios");
            e.Property(x => x.Id).HasColumnName("id");
            e.Property(x => x.Nombre).HasColumnName("nombre");
            e.Property(x => x.Email).HasColumnName("email");
            e.Property(x => x.PasswordHash).HasColumnName("password_hash");
            e.Property(x => x.Rol).HasColumnName("rol");
            e.Property(x => x.Activo).HasColumnName("activo");
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
        });

        modelBuilder.Entity<Categoria>(e =>
        {
            e.ToTable("categorias");
            e.Property(x => x.Id).HasColumnName("id");
            e.Property(x => x.Nombre).HasColumnName("nombre");
            e.Property(x => x.Descripcion).HasColumnName("descripcion");
            e.Property(x => x.Activa).HasColumnName("activa");
        });

        modelBuilder.Entity<Proveedor>(e =>
        {
            e.ToTable("proveedores");
            e.Property(x => x.Id).HasColumnName("id");
            e.Property(x => x.Nombre).HasColumnName("nombre");
            e.Property(x => x.Contacto).HasColumnName("contacto");
            e.Property(x => x.Telefono).HasColumnName("telefono");
            e.Property(x => x.Rfc).HasColumnName("rfc");
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
        });

        modelBuilder.Entity<Producto>(e =>
        {
            e.ToTable("productos");
            e.Property(x => x.Id).HasColumnName("id");
            e.Property(x => x.CategoriaId).HasColumnName("categoria_id");
            e.Property(x => x.ProveedorId).HasColumnName("proveedor_id");
            e.Property(x => x.Nombre).HasColumnName("nombre");
            e.Property(x => x.Sku).HasColumnName("sku");
            e.Property(x => x.Descripcion).HasColumnName("descripcion");
            e.Property(x => x.PrecioCompra).HasColumnName("precio_compra").HasColumnType("numeric(12,2)");
            e.Property(x => x.PrecioVenta).HasColumnName("precio_venta").HasColumnType("numeric(12,2)");
            e.Property(x => x.StockActual).HasColumnName("stock_actual");
            e.Property(x => x.StockMinimo).HasColumnName("stock_minimo");
            e.Property(x => x.Unidad).HasColumnName("unidad");
            e.Property(x => x.Activo).HasColumnName("activo");
            e.Property(x => x.UpdatedAt).HasColumnName("updated_at");
            e.HasOne(x => x.Categoria).WithMany(c => c.Productos).HasForeignKey(x => x.CategoriaId);
            e.HasOne(x => x.Proveedor).WithMany(p => p.Productos).HasForeignKey(x => x.ProveedorId);
        });

        modelBuilder.Entity<MovimientoInventario>(e =>
        {
            e.ToTable("movimientos_inventario");
            e.Property(x => x.Id).HasColumnName("id");
            e.Property(x => x.ProductoId).HasColumnName("producto_id");
            e.Property(x => x.UsuarioId).HasColumnName("usuario_id");
            e.Property(x => x.Tipo).HasColumnName("tipo");
            e.Property(x => x.Cantidad).HasColumnName("cantidad");
            e.Property(x => x.PrecioUnitario).HasColumnName("precio_unitario").HasColumnType("numeric(12,2)");
            e.Property(x => x.Referencia).HasColumnName("referencia");
            e.Property(x => x.Notas).HasColumnName("notas");
            e.Property(x => x.Fecha).HasColumnName("fecha");
            e.HasOne(x => x.Producto).WithMany().HasForeignKey(x => x.ProductoId);
            e.HasOne(x => x.Usuario).WithMany().HasForeignKey(x => x.UsuarioId);
        });

        modelBuilder.Entity<Venta>(e =>
        {
            e.ToTable("ventas");
            e.Property(x => x.Id).HasColumnName("id");
            e.Property(x => x.UsuarioId).HasColumnName("usuario_id");
            e.Property(x => x.Folio).HasColumnName("folio");
            e.Property(x => x.Subtotal).HasColumnName("subtotal").HasColumnType("numeric(12,2)");
            e.Property(x => x.Iva).HasColumnName("iva").HasColumnType("numeric(12,2)");
            e.Property(x => x.Total).HasColumnName("total").HasColumnType("numeric(12,2)");
            e.Property(x => x.Estatus).HasColumnName("estatus");
            e.Property(x => x.Fecha).HasColumnName("fecha");
            e.HasOne(x => x.Usuario).WithMany().HasForeignKey(x => x.UsuarioId);
            e.HasMany(x => x.Detalles).WithOne(d => d.Venta).HasForeignKey(d => d.VentaId);
        });

        modelBuilder.Entity<VentaDetalle>(e =>
        {
            e.ToTable("venta_detalles");
            e.Property(x => x.Id).HasColumnName("id");
            e.Property(x => x.VentaId).HasColumnName("venta_id");
            e.Property(x => x.ProductoId).HasColumnName("producto_id");
            e.Property(x => x.Cantidad).HasColumnName("cantidad");
            e.Property(x => x.PrecioUnitario).HasColumnName("precio_unitario").HasColumnType("numeric(12,2)");
            e.Property(x => x.Subtotal).HasColumnName("subtotal").HasColumnType("numeric(12,2)");
            e.HasOne(x => x.Producto).WithMany().HasForeignKey(x => x.ProductoId);
        });
    }
}
