using InventarioPyme.Api.Data;
using InventarioPyme.Api.DTOs.Dashboard;
using Microsoft.EntityFrameworkCore;

namespace InventarioPyme.Api.Services;

public class DashboardService : IDashboardService
{
    private readonly InventarioDbContext _db;

    public DashboardService(InventarioDbContext db) => _db = db;

    public async Task<DashboardResumen> ObtenerResumenAsync()
    {
        var hoy = DateTime.UtcNow.Date;
        var manana = hoy.AddDays(1);

        var totalProductos = await _db.Productos.CountAsync(p => p.Activo);

        var stockBajo = await _db.Productos
            .CountAsync(p => p.Activo && p.StockActual <= p.StockMinimo);

        var ventasHoy = await _db.Ventas
            .CountAsync(v => v.Fecha >= hoy && v.Fecha < manana && v.Estatus != "cancelada");

        var ingresosHoy = await _db.Ventas
            .Where(v => v.Fecha >= hoy && v.Fecha < manana && v.Estatus != "cancelada")
            .SumAsync(v => (decimal?)v.Total) ?? 0m;

        return new DashboardResumen(totalProductos, stockBajo, ventasHoy, ingresosHoy);
    }
}
