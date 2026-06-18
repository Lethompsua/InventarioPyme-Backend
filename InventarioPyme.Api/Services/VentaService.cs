using InventarioPyme.Api.Data;
using InventarioPyme.Api.DTOs;
using InventarioPyme.Api.DTOs.Ventas;
using InventarioPyme.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace InventarioPyme.Api.Services;

public class VentaService : IVentaService
{
    private readonly InventarioDbContext _db;

    public VentaService(InventarioDbContext db) => _db = db;

    public async Task<PagedResponse<VentaResponse>> ListarAsync(
        DateTime? fechaInicio, DateTime? fechaFin, string? estatus, int pagina, int tamanioPagina)
    {
        var query = _db.Ventas.Include(v => v.Usuario).AsQueryable();

        if (fechaInicio.HasValue)
            query = query.Where(v => v.Fecha >= fechaInicio.Value);
        if (fechaFin.HasValue)
            query = query.Where(v => v.Fecha <= fechaFin.Value);
        if (!string.IsNullOrWhiteSpace(estatus))
            query = query.Where(v => v.Estatus == estatus);

        var total = await query.CountAsync();
        var items = await query
            .OrderByDescending(v => v.Fecha)
            .Skip((pagina - 1) * tamanioPagina)
            .Take(tamanioPagina)
            .ToListAsync();

        var response = items.Select(v => new VentaResponse(
            v.Id, v.Folio, v.Usuario.Nombre,
            v.Subtotal, v.Iva, v.Total, v.Estatus, v.Fecha, null
        ));

        return new PagedResponse<VentaResponse>(response, total, pagina, tamanioPagina);
    }

    public async Task<VentaResponse> CrearAsync(VentaCreateRequest request, Guid usuarioId)
    {
        var detallesList = request.Detalles.ToList();
        var productoIds = detallesList.Select(d => d.ProductoId).ToList();

        var productos = await _db.Productos
            .Where(p => productoIds.Contains(p.Id) && p.Activo)
            .ToDictionaryAsync(p => p.Id, p => p);

        foreach (var item in detallesList)
        {
            if (!productos.TryGetValue(item.ProductoId, out var prod))
                throw new KeyNotFoundException($"Producto {item.ProductoId} no encontrado");
            if (prod.StockActual < item.Cantidad)
                throw new InvalidOperationException($"Stock insuficiente para '{prod.Nombre}' (disponible: {prod.StockActual})");
        }

        var year = DateTime.UtcNow.Year;
        var inicioAno = new DateTime(year, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var inicioSiguiente = new DateTime(year + 1, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var count = await _db.Ventas.CountAsync(v => v.Fecha >= inicioAno && v.Fecha < inicioSiguiente) + 1;
        var folio = $"VTA-{year}-{count:D4}";

        var ventaId = Guid.NewGuid();
        decimal subtotal = 0;
        var detallesEntidad = new List<VentaDetalle>();
        var movimientos = new List<MovimientoInventario>();

        foreach (var item in detallesList)
        {
            var prod = productos[item.ProductoId];
            var itemSubtotal = item.Cantidad * item.PrecioUnitario;
            subtotal += itemSubtotal;

            detallesEntidad.Add(new VentaDetalle
            {
                Id = Guid.NewGuid(),
                VentaId = ventaId,
                ProductoId = item.ProductoId,
                Cantidad = item.Cantidad,
                PrecioUnitario = item.PrecioUnitario,
                Subtotal = itemSubtotal
            });

            prod.StockActual -= item.Cantidad;
            prod.UpdatedAt = DateTime.UtcNow;

            movimientos.Add(new MovimientoInventario
            {
                Id = Guid.NewGuid(),
                ProductoId = item.ProductoId,
                UsuarioId = usuarioId,
                Tipo = "venta",
                Cantidad = item.Cantidad,
                PrecioUnitario = item.PrecioUnitario,
                Referencia = folio,
                Fecha = DateTime.UtcNow
            });
        }

        var iva = subtotal * 0.16m;
        var venta = new Venta
        {
            Id = ventaId,
            UsuarioId = usuarioId,
            Folio = folio,
            Subtotal = subtotal,
            Iva = iva,
            Total = subtotal + iva,
            Estatus = "completada",
            Fecha = DateTime.UtcNow,
            Detalles = detallesEntidad
        };

        _db.Ventas.Add(venta);
        _db.MovimientosInventario.AddRange(movimientos);
        await _db.SaveChangesAsync();

        await _db.Entry(venta).Reference(v => v.Usuario).LoadAsync();

        var detallesResponse = detallesEntidad.Select(d => new VentaDetalleResponse(
            d.ProductoId, productos[d.ProductoId].Nombre,
            d.Cantidad, d.PrecioUnitario, d.Subtotal
        ));

        return new VentaResponse(
            venta.Id, venta.Folio, venta.Usuario.Nombre,
            venta.Subtotal, venta.Iva, venta.Total, venta.Estatus, venta.Fecha,
            detallesResponse
        );
    }

    public async Task<VentaResponse?> CancelarAsync(Guid id, Guid usuarioId)
    {
        var venta = await _db.Ventas
            .Include(v => v.Usuario)
            .Include(v => v.Detalles)
            .FirstOrDefaultAsync(v => v.Id == id);

        if (venta == null) return null;
        if (venta.Estatus == "cancelada")
            throw new InvalidOperationException("La venta ya está cancelada");

        var productoIds = venta.Detalles.Select(d => d.ProductoId).ToList();
        var productos = await _db.Productos
            .Where(p => productoIds.Contains(p.Id))
            .ToDictionaryAsync(p => p.Id, p => p);

        var movimientos = new List<MovimientoInventario>();

        foreach (var detalle in venta.Detalles)
        {
            if (!productos.TryGetValue(detalle.ProductoId, out var prod)) continue;

            prod.StockActual += detalle.Cantidad;
            prod.UpdatedAt = DateTime.UtcNow;

            movimientos.Add(new MovimientoInventario
            {
                Id = Guid.NewGuid(),
                ProductoId = detalle.ProductoId,
                UsuarioId = usuarioId,
                Tipo = "entrada",
                Cantidad = detalle.Cantidad,
                PrecioUnitario = detalle.PrecioUnitario,
                Referencia = $"CANCEL-{venta.Folio}",
                Notas = "Revertido por cancelación de venta",
                Fecha = DateTime.UtcNow
            });
        }

        venta.Estatus = "cancelada";
        _db.MovimientosInventario.AddRange(movimientos);
        await _db.SaveChangesAsync();

        var detallesResponse = venta.Detalles.Select(d => new VentaDetalleResponse(
            d.ProductoId,
            productos.TryGetValue(d.ProductoId, out var p) ? p.Nombre : "",
            d.Cantidad, d.PrecioUnitario, d.Subtotal
        ));

        return new VentaResponse(
            venta.Id, venta.Folio, venta.Usuario.Nombre,
            venta.Subtotal, venta.Iva, venta.Total, venta.Estatus, venta.Fecha,
            detallesResponse
        );
    }
}
