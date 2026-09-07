using MiseHub.Api.Interfaces;
using MiseHub.Api.Models;
using MiseHub.Api.DTOs;

namespace MiseHub.Api.Services.Fake;

public class FakeOrderService : IOrderService
{
    private readonly List<Order> _orders;

    // Räknare för att generera nästa order-id ("ORD-1044", "ORD-1045"...).
    // Privat fält på instansen — eftersom servicen är singleton delas
    // räknaren korrekt mellan alla requests.
    private int _counter = 1043;

    // Konstruktorn körs EN gång, när Program.cs skapar singleton-instansen
    // vid appstart. Här sätter vi upp start-datan (samma 4 ordrar som fanns
    // i original-appens mock-data.ts).
    public FakeOrderService()
    {
        _orders = new List<Order>
        {
            new Order { Id = "ORD-1042", Date = new DateOnly(2026, 6, 8), SupplierId = "martin-servera", Items = 12, Total = 4820, Status = OrderStatus.Delivered, Paid = true,
                LineItems = new() {
                    new() { ProductId = "p1", Name = "Olive Oil Extra Virgin 5L", Unit = "case", Price = 489, Quantity = 6 },
                    new() { ProductId = "p2", Name = "San Marzano Tomatoes 2.5kg", Unit = "tin", Price = 79, Quantity = 4 },
                    new() { ProductId = "p3", Name = "Parmigiano Reggiano 24m, 1kg", Unit = "kg", Price = 329, Quantity = 2 },
                }},
            new Order { Id = "ORD-1041", Date = new DateOnly(2026, 6, 7), SupplierId = "carlsberg", Items = 4, Total = 3120, Status = OrderStatus.Delivered, Paid = true,
                LineItems = new() {
                    new() { ProductId = "p8", Name = "Carlsberg Pilsner 30L Keg", Unit = "keg", Price = 1290, Quantity = 2 },
                    new() { ProductId = "p9", Name = "Somersby Apple Cider 24x33cl", Unit = "case", Price = 459, Quantity = 2 },
                }},
            new Order { Id = "ORD-1040", Date = new DateOnly(2026, 6, 6), SupplierId = "altia", Items = 6, Total = 1845, Status = OrderStatus.Shipped, Paid = false,
                LineItems = new() {
                    new() { ProductId = "p5", Name = "Koskenkorva Vodka 1L", Unit = "bottle", Price = 219, Quantity = 3 },
                    new() { ProductId = "p6", Name = "Larsen VS Cognac 0.7L", Unit = "bottle", Price = 389, Quantity = 3 },
                }},
            new Order { Id = "ORD-1039", Date = new DateOnly(2026, 6, 5), SupplierId = "spendrups", Items = 8, Total = 2310, Status = OrderStatus.Pending, Paid = false,
                LineItems = new() {
                    new() { ProductId = "p10", Name = "Ramlösa Sparkling 24x33cl", Unit = "case", Price = 199, Quantity = 4 },
                    new() { ProductId = "p11", Name = "Norrlands Guld 30L Keg", Unit = "keg", Price = 1190, Quantity = 1 },
                    new() { ProductId = "p12", Name = "Loka Crush Lemon 24x33cl", Unit = "case", Price = 239, Quantity = 3 },
                }},
        };
    }

    public Task<List<Order>> GetAllAsync() =>
        Task.FromResult(_orders.OrderByDescending(o => o.Date).ToList());

    public Task<Order?> GetByIdAsync(string id) =>
        Task.FromResult(_orders.FirstOrDefault(o => o.Id == id));

    public Task<List<Order>> GetBySupplierAsync(string supplierId) =>
        Task.FromResult(_orders.Where(o => o.SupplierId == supplierId).ToList());

    // Detta är servern som räknar ut Items/Total — INTE något klienten
    // skickar in. Klienten skickar bara SupplierId + LineItems, resten
    // beräknas här. Viktigt mönster: lita aldrig på att klienten räknat
    // rätt, räkna om det som spelar roll (pengar!) på servern.
    public Task<Order> CreateAsync(CreateOrderRequest request)
    {
        _counter++;
        var order = new Order
        {
            Id = $"ORD-{_counter}",
            Date = DateOnly.FromDateTime(DateTime.UtcNow),
            SupplierId = request.SupplierId,
            Items = request.LineItems.Sum(i => i.Quantity),           // summerar antal
            Total = request.LineItems.Sum(i => i.Price * i.Quantity), // pris × antal, summerat
            Status = OrderStatus.Pending,
            Paid = false,
            LineItems = request.LineItems.Select(i => new OrderLineItem
            {
                ProductId = i.ProductId,
                Name = i.Name,
                Unit = i.Unit,
                Price = i.Price,
                Quantity = i.Quantity,
            }).ToList(),
        };
        // Insert(0, ...) lägger den NYA ordern FÖRST i listan (nyast överst)
        _orders.Insert(0, order);
        return Task.FromResult(order);
    }

    // Task.CompletedTask = "klart", används när metoden inte returnerar
    // något värde (motsvarar en async void/Promise<void> i JS)
    public Task PayAllAsync()
    {
        foreach (var o in _orders.Where(o => !o.Paid)) o.Paid = true;
        return Task.CompletedTask;
    }

    public Task<bool> PayOneAsync(string id)
    {
        var order = _orders.FirstOrDefault(o => o.Id == id);
        if (order is null) return Task.FromResult(false);
        order.Paid = true;
        return Task.FromResult(true);
    }
}
