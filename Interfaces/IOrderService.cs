using MiseHub.Api.Models;
using MiseHub.Api.DTOs;
namespace MiseHub.Api.Interfaces
{
    public interface IOrderService
    {
        Task<List<Order>> GetAllAsync();
        Task<Order?> GetByIdAsync(string id);
        Task<List<Order>> GetBySupplierAsync(string supplierId);
        Task<Order> CreateAsync(CreateOrderRequest request);

        // Motsvarar "Pay all"-knappen på Orders-sidan
        Task PayAllAsync();

        // Motsvarar att betala EN specifik order (t.ex. från orderdetaljsidan)
        Task<bool> PayOneAsync(string id);
    }
}
