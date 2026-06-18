using System.Security.Claims;
using InventarioPyme.Api.DTOs.Ventas;
using InventarioPyme.Api.Services;

namespace InventarioPyme.Api.Controllers;

public static class VentaEndpoints
{
    public static void MapVentaEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/ventas").WithTags("Ventas").RequireAuthorization();

        group.MapGet("/", async (
            IVentaService ventaService,
            DateTime? fechaInicio,
            DateTime? fechaFin,
            string? estatus,
            int pagina = 1,
            int tamanioPagina = 20) =>
        {
            var result = await ventaService.ListarAsync(fechaInicio, fechaFin, estatus, pagina, tamanioPagina);
            return Results.Ok(result);
        });

        group.MapPost("/", async (
            VentaCreateRequest request,
            IVentaService ventaService,
            ClaimsPrincipal user) =>
        {
            var userId = Guid.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await ventaService.CrearAsync(request, userId);
            return Results.Created($"/api/ventas/{result.Id}", result);
        });

        group.MapPut("/{id:guid}/cancelar", async (
            Guid id,
            IVentaService ventaService,
            ClaimsPrincipal user) =>
        {
            var userId = Guid.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await ventaService.CancelarAsync(id, userId);
            if (result == null)
                return Results.Json(new { error = "Venta no encontrada" }, statusCode: 404);
            return Results.Ok(result);
        });
    }
}
