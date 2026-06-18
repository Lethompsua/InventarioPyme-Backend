using InventarioPyme.Api.Data;
using InventarioPyme.Api.DTOs;
using InventarioPyme.Api.DTOs.Inventario;
using InventarioPyme.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace InventarioPyme.Api.Services;

public class InventarioService : IInventarioService
{
    private readonly InventarioDbContext _db;

    public InventarioService(InventarioDbContext db) => _db = db;

    public async Task<PagedResponse<MovimientoResponse>> ListarMovimientosAsync(
        Guid? productoId, string? tipo, int pagina, int tamanioPagina)
    {
        var query = _db.MovimientosInventario
            .Include(m => m.Producto)
            .Include(m => m.Usuario)
            .AsQueryable();

        if (productoId.HasValue)
            query = query.Where(m => m.ProductoId == productoId);
        if (!string.IsNullOrWhiteSpace(tipo))
            query = query.Where(m => m.Tipo == tipo);

        var total = await query.CountAsync();
        var items = await query
            .OrderByDescending(m => m.Fecha)
            .Skip((pagina - 1) * tamanioPagina)
            .Take(tamanioPagina)
            .ToListAsync();

        var response = items.Select(m => new MovimientoResponse(
            m.Id, m.Producto.Nombre, m.Usuario.Nombre,
            m.Tipo, m.Cantidad, m.PrecioUnitario,
            m.Referencia, m.Notas, m.Fecha
        ));

        return new PagedResponse<MovimientoResponse>(response, total, pagina, tamanioPagina);
    }

    public async Task<MovimientoResponse> RegistrarEntradaAsync(EntradaInventarioRequest request, Guid usuarioId)
    {
        var producto = await _db.Productos.FindAsync(request.ProductoId)
            ?? throw new KeyNotFoundException("Producto no encontrado");

        var movimiento = new MovimientoInventario
        {
            Id = Guid.NewGuid(),
            ProductoId = request.ProductoId,
            UsuarioId = usuarioId,
            Tipo = "entrada",
            Cantidad = request.Cantidad,
            PrecioUnitario = request.PrecioUnitario,
            Referencia = request.Referencia,
            Notas = request.Notas,
            Fecha = DateTime.UtcNow
        };

        producto.StockActual += request.Cantidad;
        producto.UpdatedAt = DateTime.UtcNow;

        _db.MovimientosInventario.Add(movimiento);
        await _db.SaveChangesAsync();

        await _db.Entry(movimiento).Reference(m => m.Producto).LoadAsync();
        await _db.Entry(movimiento).Reference(m => m.Usuario).LoadAsync();

        return new MovimientoResponse(
            movimiento.Id, movimiento.Producto.Nombre, movimiento.Usuario.Nombre,
            movimiento.Tipo, movimiento.Cantidad, movimiento.PrecioUnitario,
            movimiento.Referencia, movimiento.Notas, movimiento.Fecha
        );
    }

    public async Task<MovimientoResponse> RegistrarAjusteAsync(AjusteInventarioRequest request, Guid usuarioId)
    {
        var producto = await _db.Productos.FindAsync(request.ProductoId)
            ?? throw new KeyNotFoundException("Producto no encontrado");

        var diferencia = Math.Abs(request.NuevoStock - producto.StockActual);
        var notas = request.Notas ?? $"Ajuste de stock: {producto.StockActual} → {request.NuevoStock}";

        var movimiento = new MovimientoInventario
        {
            Id = Guid.NewGuid(),
            ProductoId = request.ProductoId,
            UsuarioId = usuarioId,
            Tipo = "ajuste",
            Cantidad = diferencia,
            PrecioUnitario = 0,
            Notas = notas,
            Fecha = DateTime.UtcNow
        };

        producto.StockActual = request.NuevoStock;
        producto.UpdatedAt = DateTime.UtcNow;

        _db.MovimientosInventario.Add(movimiento);
        await _db.SaveChangesAsync();

        await _db.Entry(movimiento).Reference(m => m.Producto).LoadAsync();
        await _db.Entry(movimiento).Reference(m => m.Usuario).LoadAsync();

        return new MovimientoResponse(
            movimiento.Id, movimiento.Producto.Nombre, movimiento.Usuario.Nombre,
            movimiento.Tipo, movimiento.Cantidad, movimiento.PrecioUnitario,
            movimiento.Referencia, movimiento.Notas, movimiento.Fecha
        );
    }
}
