using PedidosApp.Domain;
using PedidosApp.DTO;

namespace PedidosApp.Service.Interface
{
    public interface IOrderService
    {
        Task<ApiResponseDTO<object>> CreateAsync(CreateOrderDTO dto);
        Task<ApiResponseDTO<Order>> GetByIdAsync(Guid id);
        Task<ApiResponseDTO<List<Order>>> GetAllAsync();
    }
}
