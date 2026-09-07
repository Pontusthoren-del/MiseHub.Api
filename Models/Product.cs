namespace MiseHub.Api.Models;

// SupplierId kopplar till en leverantör, men bara som en sträng vi letar
// upp för hand (ingen riktig foreign key ännu — det kommer med EF Core sen).
public class Product
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Unit { get; set; } = string.Empty; // "case", "bottle", "keg" osv.
    public decimal Price { get; set; }
    public string SupplierId { get; set; } = string.Empty;
}