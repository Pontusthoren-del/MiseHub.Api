using MiseHub.Api.Models;
using MiseHub.Api.DTOs;

namespace MiseHub.Api.Interfaces
{
    // Interface = kontrakt. Säger VAD som ska gå att göra (GetAllAsync, GetByIdAsync...)
    // men inte HUR. FakeSupplierService är dagens implementation (hårdkodad data).
    // Den dagen jag bygger DbSupplierService mot en riktig databas byter jag bara
    // ut en rad i Program.cs (Fake → Db) — resten av appen märker ingenting,
    // eftersom controllern bara pratar med interfacet.
    //
    // Task<T> = async-grej, typ Promise<T> i JS. Använder det redan nu även fast
    // fejk-datan svarar direkt, så signaturen inte behöver ändras sen.

    public interface ISupplierService
    {
        Task<List<Supplier>> GetAllAsync();

        // Supplier? (nullable) — kan ge tillbaka null om ingen leverantör
        // med det id:t hittades. Tvingar controllern att hantera "hittades inte".
        Task<Supplier?> GetByIdAsync(string id);

        Task<Supplier> AddAsync(CreateSupplierRequest request);

        // bool = lyckades borttagningen eller inte (fanns id:t?)
        Task<bool> RemoveAsync(string id);
    }
}
