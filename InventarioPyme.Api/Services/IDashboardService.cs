using InventarioPyme.Api.DTOs.Dashboard;

namespace InventarioPyme.Api.Services;

public interface IDashboardService
{
    Task<DashboardResumen> ObtenerResumenAsync();
}
