using MiseHub.Api.Models;
using MiseHub.Api.DTOs;

namespace MiseHub.Api.Interfaces
{
    public interface IProductService
    {
        // query/supplierId är valfria filter (samma som ?query=&supplierId= i URL:en)
        Task<List<Product>> GetAllAsync(string? query, string? supplierId);
        Task<List<Product>> GetBySupplierAsync(string supplierId);

        // Motsvarar "Sync catalog"-knappen i Lovable-appen. Returnerar antal
        // NYA produkter som lades till, så frontend kan visa "Synced 6 new products".
        Task<int> SyncCatalogAsync(string supplierId);
    }
}
