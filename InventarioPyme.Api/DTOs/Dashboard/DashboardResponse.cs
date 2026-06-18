namespace InventarioPyme.Api.DTOs.Dashboard;

public record DashboardResumen(
    int TotalProductos,
    int ProductosStockBajo,
    int VentasHoy,
    decimal IngresosHoy
);
