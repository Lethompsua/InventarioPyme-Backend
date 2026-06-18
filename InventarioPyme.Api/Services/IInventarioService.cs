using InventarioPyme.Api.DTOs;
using InventarioPyme.Api.DTOs.Inventario;

namespace InventarioPyme.Api.Services;

public interface IInventarioService
{
    Task<PagedResponse<MovimientoResponse>> ListarMovimientosAsync(Guid? productoId, string? tipo, int pagina, int tamanioPagina);
    Task<MovimientoResponse> RegistrarEntradaAsync(EntradaInventarioRequest request, Guid usuarioId);
    Task<MovimientoResponse> RegistrarAjusteAsync(AjusteInventarioRequest request, Guid usuarioId);
}
