using InventarioPyme.Api.DTOs.Auth;

namespace InventarioPyme.Api.Services;

public interface IAuthService
{
    Task<LoginResponse?> LoginAsync(LoginRequest request);
}
