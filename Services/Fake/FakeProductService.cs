using MiseHub.Api.Interfaces;
using MiseHub.Api.Models;

namespace MiseHub.Api.Services.Fake;

// Samma mönster som FakeSupplierService — singleton, in-memory data.
public class FakeProductService : IProductService
{
    private readonly List<Product> _products = new()
    {
        new Product { Id = "p1", Name = "Olive Oil Extra Virgin 5L", Unit = "case", Price = 489, SupplierId = "martin-servera" },
        new Product { Id = "p2", Name = "San Marzano Tomatoes 2.5kg", Unit = "tin", Price = 79, SupplierId = "martin-servera" },
        new Product { Id = "p3", Name = "Parmigiano Reggiano 24m, 1kg", Unit = "kg", Price = 329, SupplierId = "martin-servera" },
        new Product { Id = "p4", Name = "Sourdough Flour T65 25kg", Unit = "sack", Price = 425, SupplierId = "martin-servera" },
        new Product { Id = "p5", Name = "Koskenkorva Vodka 1L", Unit = "bottle", Price = 219, SupplierId = "altia" },
        new Product { Id = "p6", Name = "Larsen VS Cognac 0.7L", Unit = "bottle", Price = 389, SupplierId = "altia" },
        new Product { Id = "p7", Name = "Blossa Glögg 0.75L", Unit = "bottle", Price = 119, SupplierId = "altia" },
        new Product { Id = "p8", Name = "Carlsberg Pilsner 30L Keg", Unit = "keg", Price = 1290, SupplierId = "carlsberg" },
        new Product { Id = "p9", Name = "Somersby Apple Cider 24x33cl", Unit = "case", Price = 459, SupplierId = "carlsberg" },
        new Product { Id = "p10", Name = "Ramlösa Sparkling 24x33cl", Unit = "case", Price = 199, SupplierId = "spendrups" },
        new Product { Id = "p11", Name = "Norrlands Guld 30L Keg", Unit = "keg", Price = 1190, SupplierId = "spendrups" },
        new Product { Id = "p12", Name = "Loka Crush Lemon 24x33cl", Unit = "case", Price = 239, SupplierId = "spendrups" },
    };

    // Dictionary<string, string[]> = motsvarar en TypeScript
    // Record<string, string[]>. Används bara av SyncCatalogAsync nedan för
    // att generera relevanta fejk-produktnamn per leverantör.
    private static readonly Dictionary<string, string[]> SupplierQueries = new()
    {
        ["martin-servera"] = new[] { "olive oil", "pasta", "flour", "tomato", "rice", "salt" },
        ["altia"] = new[] { "wine", "vodka", "whisky", "gin", "rum", "champagne" },
        ["carlsberg"] = new[] { "beer", "cider", "lager", "ale", "stout", "soft drink" },
        ["spendrups"] = new[] { "sparkling water", "mineral water", "juice", "soda", "lemonade", "tonic" },
    };

    // Motsvarar exakt filtreringslogiken i original-appens search.tsx
    // (products.filter(...)), fast nu körd på servern istället för i
    // webbläsaren — samma princip som search-endpointen i /api/products.
    public Task<List<Product>> GetAllAsync(string? query, string? supplierId)
    {
        // .AsEnumerable() gör listan "lazy" — filtren nedan (Where) bygger
        // bara upp en KEDJA av villkor, de körs inte förrän .ToList() anropas
        var result = _products.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(supplierId) && supplierId != "all")
            result = result.Where(p => p.SupplierId == supplierId);

        // StringComparison.OrdinalIgnoreCase = skiftlägesokänslig sökning,
        // t.ex. "olive" matchar "Olive Oil Extra Virgin 5L"
        if (!string.IsNullOrWhiteSpace(query))
            result = result.Where(p => p.Name.Contains(query, StringComparison.OrdinalIgnoreCase));

        return Task.FromResult(result.ToList());
    }

    public Task<List<Product>> GetBySupplierAsync(string supplierId) =>
        Task.FromResult(_products.Where(p => p.SupplierId == supplierId).ToList());

    // Simulerar en "katalog-synk" mot ett leverantörs-API. Genererar
    // DETERMINISTISKA fejk-produkter (samma indata ger alltid samma
    // produkter/priser, se PriceFromCode nedan) tills ni har ett riktigt
    // API att anropa. Motsvarar Open Food Facts-hacket i Lovable-appens
    // products-store.ts, fast utan det riktiga nätverksanropet.
    public Task<int> SyncCatalogAsync(string supplierId)
    {
        var queries = SupplierQueries.GetValueOrDefault(supplierId, new[] { "food" });
        var existingIds = _products.Select(p => p.Id).ToHashSet();
        var added = 0;

        foreach (var (q, qi) in queries.Select((q, i) => (q, i)))
        {
            for (var i = 1; i <= 6; i++)
            {
                var code = $"{supplierId}-{qi}-{i}";
                var id = $"syn-{code}";
                if (existingIds.Contains(id)) continue;

                var size = new[] { 250, 500, 750, 1000 }[i % 4];
                var units = new[] { "bottle", "case", "pack", "carton", "keg", "box" };
                _products.Add(new Product
                {
                    Id = id,
                    Name = $"{System.Globalization.CultureInfo.InvariantCulture.TextInfo.ToTitleCase(q)} {size}ml #{i}",
                    Unit = units[i % units.Length],
                    Price = PriceFromCode(code),
                    SupplierId = supplierId,
                });
                added++;
            }
        }

        return Task.FromResult(added);
    }

    // En enkel "hash"-funktion: gör om en textsträng till ett "slumpmässigt"
    // men FÖRUTSÄGBART tal mellan 49–500. Samma code ger alltid samma pris —
    // praktiskt för fejk-data så att priser inte hoppar runt varje gång man syncar.
    // "unchecked" tillåter heltalsöverspill utan att C# kastar ett undantag.
    private static decimal PriceFromCode(string code)
    {
        uint h = 0;
        foreach (var c in code) h = unchecked(h * 31 + c);
        return 49 + (h % 451);
    }
}
