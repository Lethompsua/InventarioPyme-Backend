namespace InventarioPyme.Api.DTOs.Auth;

public record LoginResponse(string Token, string Nombre, string Rol, DateTime Expira);
