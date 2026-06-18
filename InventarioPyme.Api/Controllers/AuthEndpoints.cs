using InventarioPyme.Api.DTOs.Auth;
using InventarioPyme.Api.Services;

namespace InventarioPyme.Api.Controllers;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/auth").WithTags("Auth");

        group.MapPost("/login", async (LoginRequest request, IAuthService authService) =>
        {
            var result = await authService.LoginAsync(request);
            if (result == null)
                return Results.Json(new { error = "Credenciales inválidas" }, statusCode: 401);
            return Results.Ok(result);
        });
    }
}
