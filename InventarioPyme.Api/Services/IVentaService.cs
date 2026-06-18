using InventarioPyme.Api.DTOs;
using InventarioPyme.Api.DTOs.Ventas;

namespace InventarioPyme.Api.Services;

public interface IVentaService
{
    Task<PagedResponse<VentaResponse>> ListarAsync(DateTime? fechaInicio, DateTime? fechaFin, string? estatus, int pagina, int tamanioPagina);
    Task<VentaResponse> CrearAsync(VentaCreateRequest request, Guid usuarioId);
    Task<VentaResponse?> CancelarAsync(Guid id, Guid usuarioId);
}
