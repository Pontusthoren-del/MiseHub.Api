namespace MiseHub.Api.Models;

public class Supplier
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Initials { get; set; } = string.Empty;
    public string Color { get; set; } = "#8b5a2b";
    public string Description { get; set; } = string.Empty;

    // string? = nullable, fältet får vara null (valfria fält)
    public string? ContactName { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public string? Website { get; set; }
    public decimal? MinOrder { get; set; } // decimal för pengar, aldrig double/float
    public string? DeliveryDays { get; set; }
    public DateOnly? JoinedAt { get; set; }
}

