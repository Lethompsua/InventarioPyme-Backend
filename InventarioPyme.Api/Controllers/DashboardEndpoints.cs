using InventarioPyme.Api.Services;

namespace InventarioPyme.Api.Controllers;

public static class DashboardEndpoints
{
    public static void MapDashboardEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/dashboard").WithTags("Dashboard").RequireAuthorization();

        group.MapGet("/resumen", async (IDashboardService dashboardService) =>
        {
            var result = await dashboardService.ObtenerResumenAsync();
            return Results.Ok(result);
        });
    }
}
