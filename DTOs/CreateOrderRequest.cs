using MiseHub.Api.Models;
using System.ComponentModel.DataAnnotations;

namespace MiseHub.Api.DTOs;

// DTO för POST /api/orders. Id/Date/Items/Total/Status/Paid sätts av servern
// (se FakeOrderService.CreateAsync) — klienten ska inte kunna manipulera dem.
public class CreateOrderRequest
{
    [Required]
    public string SupplierId { get; set; } = string.Empty;
    [Required]
    [MinLength(1,ErrorMessage ="At least one line item is required.")]
    public List<CreateOrderLineItem> LineItems { get; set; } = new();
}
