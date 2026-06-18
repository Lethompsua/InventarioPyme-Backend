using System.Security.Claims;
using InventarioPyme.Api.DTOs.Productos;
using InventarioPyme.Api.Services;

namespace InventarioPyme.Api.Controllers;

public static class ProductoEndpoints
{
    public static void MapProductoEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/productos").WithTags("Productos").RequireAuthorization();

        group.MapGet("/", async (
            string? nombre,
            string? sku,
            Guid? categoriaId,
            IProductoService productoService,
            int pagina = 1,
            int tamanioPagina = 20) =>
        {
            var result = await productoService.ListarAsync(nombre, sku, categoriaId, pagina, tamanioPagina);
            return Results.Ok(result);
        });

        group.MapGet("/{id:guid}", async (Guid id, IProductoService productoService) =>
        {
            var producto = await productoService.ObtenerPorIdAsync(id);
            if (producto == null)
                return Results.Json(new { error = "Producto no encontrado" }, statusCode: 404);
            return Results.Ok(producto);
        });

        group.MapPost("/", async (
            ProductoCreateRequest request,
            IProductoService productoService,
            ClaimsPrincipal user) =>
        {
            if (!user.IsInRole("admin"))
                return Results.Json(new { error = "Se requiere rol de administrador" }, statusCode: 403);
            var result = await productoService.CrearAsync(request);
            return Results.Created($"/api/productos/{result.Id}", result);
        });

        group.MapPut("/{id:guid}", async (
            Guid id,
            ProductoUpdateRequest request,
            IProductoService productoService,
            ClaimsPrincipal user) =>
        {
            if (!user.IsInRole("admin"))
                return Results.Json(new { error = "Se requiere rol de administrador" }, statusCode: 403);
            var result = await productoService.ActualizarAsync(id, request);
            if (result == null)
                return Results.Json(new { error = "Producto no encontrado" }, statusCode: 404);
            return Results.Ok(result);
        });

        group.MapDelete("/{id:guid}", async (
            Guid id,
            IProductoService productoService,
            ClaimsPrincipal user) =>
        {
            if (!user.IsInRole("admin"))
                return Results.Json(new { error = "Se requiere rol de administrador" }, statusCode: 403);
            var eliminado = await productoService.EliminarAsync(id);
            if (!eliminado)
                return Results.Json(new { error = "Producto no encontrado" }, statusCode: 404);
            return Results.NoContent();
        });
    }
}
