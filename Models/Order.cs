namespace MiseHub.Api.Models;

// Enum = fast lista med tillåtna värden istället för fri text (stavfel-säkert,
// ger autocomplete). Motsvarar TS `type OrderStatus = "pending" | "shipped" | "delivered"`
public enum OrderStatus
{
    Pending,
    Shipped,
    Delivered
}

// En rad i en order, t.ex. "6 st Olive Oil á 489 kr". Namn/pris kopieras hit
// istället för att bara peka på ProductId, så orderhistoriken visar vad som
// gällde DÅ även om produktens pris ändras senare.
public class OrderLineItem
{
    public string ProductId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Unit { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Quantity { get; set; }
}

public class Order
{
    public string Id { get; set; } = string.Empty; // t.ex. "ORD-1043"
    public DateOnly Date { get; set; }
    public string SupplierId { get; set; } = string.Empty;
    public int Items { get; set; }
    public decimal Total { get; set; }
    public OrderStatus Status { get; set; } = OrderStatus.Pending;
    public bool Paid { get; set; }

    // = new() ger en tom lista direkt, aldrig null
    public List<OrderLineItem> LineItems { get; set; } = new();
}
