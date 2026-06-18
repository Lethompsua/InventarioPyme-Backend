using InventarioPyme.Api.DTOs;
using InventarioPyme.Api.DTOs.Productos;

namespace InventarioPyme.Api.Services;

public interface IProductoService
{
    Task<PagedResponse<ProductoResponse>> ListarAsync(string? nombre, string? sku, Guid? categoriaId, int pagina, int tamanioPagina);
    Task<ProductoResponse?> ObtenerPorIdAsync(Guid id);
    Task<ProductoResponse> CrearAsync(ProductoCreateRequest request);
    Task<ProductoResponse?> ActualizarAsync(Guid id, ProductoUpdateRequest request);
    Task<bool> EliminarAsync(Guid id);
}
