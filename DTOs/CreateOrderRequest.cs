using MiseHub.Api.Models;

namespace MiseHub.Api.DTOs;

// DTO för POST /api/orders. Id/Date/Items/Total/Status/Paid sätts av servern
// (se FakeOrderService.CreateAsync) — klienten ska inte kunna manipulera dem.
public class CreateOrderRequest
{
    public string SupplierId { get; set; } = string.Empty;
    public List<CreateOrderLineItem> LineItems { get; set; } = new();
}
