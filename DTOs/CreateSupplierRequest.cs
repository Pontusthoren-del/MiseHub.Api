using System.ComponentModel.DataAnnotations;

namespace MiseHub.Api.DTOs;

// DTO för POST /api/suppliers. Klienten ska inte kunna sätta Id/Initials/JoinedAt
// själv — de genereras av servern (se FakeSupplierService.AddAsync).
public class CreateSupplierRequest
{
    [Required]
    [StringLength(200, MinimumLength = 1)]
    public string Name { get; set; } = string.Empty;
    public string? Category { get; set; }
    public string? Description { get; set; }
    public string? ContactName { get; set; }
    [EmailAddress]
    public string? Email { get; set; }
    [Phone]
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public string? Website { get; set; }
    public string? DeliveryDays { get; set; }
    [Range(0, 10_000_000, ErrorMessage = "MinOrder must be a positive number.")]
    public decimal? MinOrder { get; set; }
}
