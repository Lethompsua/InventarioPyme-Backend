using System.Security.Claims;
using InventarioPyme.Api.DTOs.Inventario;
using InventarioPyme.Api.Services;

namespace InventarioPyme.Api.Controllers;

public static class InventarioEndpoints
{
    public static void MapInventarioEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/inventario").WithTags("Inventario").RequireAuthorization();

        group.MapGet("/movimientos", async (
            IInventarioService inventarioService,
            Guid? productoId,
            string? tipo,
            int pagina = 1,
            int tamanioPagina = 20) =>
        {
            var result = await inventarioService.ListarMovimientosAsync(productoId, tipo, pagina, tamanioPagina);
            return Results.Ok(result);
        });

        group.MapPost("/entrada", async (
            EntradaInventarioRequest request,
            IInventarioService inventarioService,
            ClaimsPrincipal user) =>
        {
            var userId = Guid.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await inventarioService.RegistrarEntradaAsync(request, userId);
            return Results.Created("/api/inventario/movimientos", result);
        });

        group.MapPost("/ajuste", async (
            AjusteInventarioRequest request,
            IInventarioService inventarioService,
            ClaimsPrincipal user) =>
        {
            if (!user.IsInRole("admin"))
                return Results.Json(new { error = "Se requiere rol de administrador" }, statusCode: 403);
            var userId = Guid.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await inventarioService.RegistrarAjusteAsync(request, userId);
            return Results.Ok(result);
        });
    }
}
