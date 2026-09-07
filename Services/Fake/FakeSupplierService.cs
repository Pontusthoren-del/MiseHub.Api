using MiseHub.Api.Interfaces;
using MiseHub.Api.Models;
using MiseHub.Api.DTOs;

namespace MiseHub.Api.Services.Fake;

// "class FakeSupplierService : ISupplierService" betyder "denna klass
// LOVAR att uppfylla ISupplierService-kontraktet". Kompilatorn kollar
// automatiskt att alla metoder i interfacet faktiskt finns implementerade
// här nedan — annars vägrar den bygga.
//
// Registreras som SINGLETON i Program.cs, vilket betyder EN instans delas
// mellan ALLA requests under körning. Det är därför _suppliers-listan
// "kommer ihåg" nya leverantörer du lägger till tills du startar om
// dotnet run — det finns ingen riktig databas som sparar permanent än.
public class FakeSupplierService : ISupplierService
{
    private static readonly string[] Palette =
    {
        "#8b5a2b", "#a37244", "#7a3b2e", "#5f7d3a", "#6b4423", "#9c6b3f", "#4d3319", "#c08457"
    };

    // Startdata — exakt samma 4 leverantörer som fanns i Lovable-appens
    // mock-data.ts. "readonly" betyder att SJÄLVA LISTOBJEKTET inte kan
    // bytas ut mot en helt annan lista, men innehållet (.Add, .Remove) får
    // fortfarande ändras — det är skillnaden mellan readonly och immutable.
    private readonly List<Supplier> _suppliers = new()
    {
        new Supplier { Id = "martin-servera", Name = "Martin & Servera", Category = "Food & Dry Goods", Initials = "MS", Color = "#8b5a2b",
            Description = "Full-range food service wholesaler.", ContactName = "Erik Lundqvist", Email = "orders@martinservera.se",
            Phone = "+46 8 722 40 00", Address = "Solna Strandväg 78, 171 54 Solna", Website = "martinservera.se",
            MinOrder = 1500, DeliveryDays = "Mon, Wed, Fri", JoinedAt = new DateOnly(2024, 3, 12) },
        new Supplier { Id = "altia", Name = "Altia", Category = "Spirits & Wine", Initials = "AL", Color = "#7a3b2e",
            Description = "Nordic spirits and wine distributor.", ContactName = "Sofia Berg", Email = "b2b@altia.se",
            Phone = "+46 8 555 130 00", Address = "Kungsgatan 12, 111 43 Stockholm", Website = "altia.com",
            MinOrder = 2000, DeliveryDays = "Tue, Thu", JoinedAt = new DateOnly(2024, 5, 2) },
        new Supplier { Id = "carlsberg", Name = "Carlsberg", Category = "Beer & Beverages", Initials = "CB", Color = "#5f7d3a",
            Description = "Beer, cider and soft drinks.", ContactName = "Jonas Wik", Email = "hospitality@carlsberg.se",
            Phone = "+46 8 757 70 00", Address = "Bryggerivägen 10, 161 86 Bromma", Website = "carlsberg.se",
            MinOrder = 1000, DeliveryDays = "Mon, Thu", JoinedAt = new DateOnly(2024, 6, 18) },
        new Supplier { Id = "spendrups", Name = "Spendrups", Category = "Beer & Beverages", Initials = "SP", Color = "#a37244",
            Description = "Swedish brewery — beer, water and wine.", ContactName = "Anna Holm", Email = "kundtjanst@spendrups.se",
            Phone = "+46 8 610 40 00", Address = "Sundbybergsvägen 1, 171 73 Solna", Website = "spendrups.se",
            MinOrder = 1200, DeliveryDays = "Wed, Fri", JoinedAt = new DateOnly(2024, 7, 4) },
    };

    // "=>" här är en "expression-bodied method" — kortform för
    // { return Task.FromResult(_suppliers.ToList()); }
    // Task.FromResult wrappar in ett värde vi redan HAR i en Task, eftersom
    // interfacet kräver Task<List<Supplier>> trots att vi inte faktiskt
    // väntar på något asynkront (ingen databas eller nätverk ännu).
    //
    // .ToList() gör en KOPIA av listan — så att den som anropar metoden
    // inte kan mutera vår interna _suppliers-lista av misstag utifrån.
    public Task<List<Supplier>> GetAllAsync() => Task.FromResult(_suppliers.ToList());

    // FirstOrDefault = LINQ-metod, hittar första matchande elementet
    // eller null om inget matchar (motsvarar Array.find i JS)
    public Task<Supplier?> GetByIdAsync(string id) =>
        Task.FromResult(_suppliers.FirstOrDefault(s => s.Id == id));

    public Task<Supplier> AddAsync(CreateSupplierRequest request)
    {
        // Bygger en URL-vänlig "slug" från namnet, t.ex. "Acme Wholesale" → "acme-wholesale"
        var slug = System.Text.RegularExpressions.Regex.Replace(request.Name.ToLowerInvariant(), "[^a-z0-9]+", "-").Trim('-');
        // Lägger till 4 slumpmässiga tecken på slutet så id:t blir unikt
        // även om två leverantörer skulle heta likadant
        var id = $"{slug}-{Guid.NewGuid().ToString("N")[..4]}";

        // Tar de två första orden i namnet, plockar första bokstaven i
        // varje, gör versaler → "Acme Wholesale" blir "AW"
        var initials = string.Concat(request.Name.Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .Take(2).Select(w => char.ToUpperInvariant(w[0])));

        var supplier = new Supplier
        {
            Id = id,
            Name = request.Name,
            Category = string.IsNullOrWhiteSpace(request.Category) ? "General" : request.Category!,
            Initials = string.IsNullOrWhiteSpace(initials) ? "SU" : initials,
            Color = Palette[_suppliers.Count % Palette.Length],
            Description = string.IsNullOrWhiteSpace(request.Description) ? "New supplier added to your network." : request.Description!,
            ContactName = request.ContactName,
            Email = request.Email,
            Phone = request.Phone,
            Address = request.Address,
            Website = request.Website,
            DeliveryDays = request.DeliveryDays,
            MinOrder = request.MinOrder,
            JoinedAt = DateOnly.FromDateTime(DateTime.UtcNow),
        };
        _suppliers.Add(supplier);
        return Task.FromResult(supplier);
    }

    public Task<bool> RemoveAsync(string id)
    {
        // RemoveAll returnerar ANTAL borttagna element (0 eller 1 här,
        // eftersom id är unikt) — "> 0" gör om det till en bool
        var removed = _suppliers.RemoveAll(s => s.Id == id) > 0;
        return Task.FromResult(removed);
    }
}
