using InventarioPyme.Api.Data;
using InventarioPyme.Api.DTOs;
using InventarioPyme.Api.DTOs.Productos;
using InventarioPyme.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace InventarioPyme.Api.Services;

public class ProductoService : IProductoService
{
    private readonly InventarioDbContext _db;

    public ProductoService(InventarioDbContext db) => _db = db;

    public async Task<PagedResponse<ProductoResponse>> ListarAsync(
        string? nombre, string? sku, Guid? categoriaId, int pagina, int tamanioPagina)
    {
        var query = _db.Productos
            .Include(p => p.Categoria)
            .Include(p => p.Proveedor)
            .Where(p => p.Activo)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(nombre))
            query = query.Where(p => p.Nombre.ToLower().Contains(nombre.ToLower()));
        if (!string.IsNullOrWhiteSpace(sku))
            query = query.Where(p => p.Sku.ToLower().Contains(sku.ToLower()));
        if (categoriaId.HasValue)
            query = query.Where(p => p.CategoriaId == categoriaId);

        var total = await query.CountAsync();
        var items = await query
            .OrderBy(p => p.Nombre)
            .Skip((pagina - 1) * tamanioPagina)
            .Take(tamanioPagina)
            .ToListAsync();

        return new PagedResponse<ProductoResponse>(items.Select(MapToResponse), total, pagina, tamanioPagina);
    }

    public async Task<ProductoResponse?> ObtenerPorIdAsync(Guid id)
    {
        var p = await _db.Productos
            .Include(p => p.Categoria)
            .Include(p => p.Proveedor)
            .FirstOrDefaultAsync(p => p.Id == id && p.Activo);

        return p == null ? null : MapToResponse(p);
    }

    public async Task<ProductoResponse> CrearAsync(ProductoCreateRequest request)
    {
        var producto = new Producto
        {
            Id = Guid.NewGuid(),
            CategoriaId = request.CategoriaId,
            ProveedorId = request.ProveedorId,
            Nombre = request.Nombre,
            Sku = request.Sku,
            Descripcion = request.Descripcion,
            PrecioCompra = request.PrecioCompra,
            PrecioVenta = request.PrecioVenta,
            StockActual = 0,
            StockMinimo = request.StockMinimo,
            Unidad = request.Unidad,
            Activo = true,
            UpdatedAt = DateTime.UtcNow
        };

        _db.Productos.Add(producto);
        await _db.SaveChangesAsync();

        await _db.Entry(producto).Reference(p => p.Categoria).LoadAsync();
        if (producto.ProveedorId.HasValue)
            await _db.Entry(producto).Reference(p => p.Proveedor).LoadAsync();

        return MapToResponse(producto);
    }

    public async Task<ProductoResponse?> ActualizarAsync(Guid id, ProductoUpdateRequest request)
    {
        var producto = await _db.Productos
            .Include(p => p.Categoria)
            .Include(p => p.Proveedor)
            .FirstOrDefaultAsync(p => p.Id == id && p.Activo);

        if (producto == null) return null;

        if (request.CategoriaId.HasValue) producto.CategoriaId = request.CategoriaId.Value;
        if (request.ProveedorId.HasValue) producto.ProveedorId = request.ProveedorId;
        if (request.Nombre != null) producto.Nombre = request.Nombre;
        if (request.Sku != null) producto.Sku = request.Sku;
        if (request.Descripcion != null) producto.Descripcion = request.Descripcion;
        if (request.PrecioCompra.HasValue) producto.PrecioCompra = request.PrecioCompra.Value;
        if (request.PrecioVenta.HasValue) producto.PrecioVenta = request.PrecioVenta.Value;
        if (request.StockMinimo.HasValue) producto.StockMinimo = request.StockMinimo.Value;
        if (request.Unidad != null) producto.Unidad = request.Unidad;
        producto.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();

        await _db.Entry(producto).Reference(p => p.Categoria).LoadAsync();
        if (producto.ProveedorId.HasValue)
            await _db.Entry(producto).Reference(p => p.Proveedor).LoadAsync();

        return MapToResponse(producto);
    }

    public async Task<bool> EliminarAsync(Guid id)
    {
        var producto = await _db.Productos.FirstOrDefaultAsync(p => p.Id == id && p.Activo);
        if (producto == null) return false;

        producto.Activo = false;
        producto.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return true;
    }

    private static ProductoResponse MapToResponse(Producto p) => new(
        p.Id, p.Nombre, p.Sku, p.Descripcion,
        p.PrecioCompra, p.PrecioVenta, p.StockActual, p.StockMinimo,
        p.Unidad, p.Activo, p.StockActual <= p.StockMinimo,
        p.Categoria.Nombre, p.Proveedor?.Nombre, p.UpdatedAt
    );
}
