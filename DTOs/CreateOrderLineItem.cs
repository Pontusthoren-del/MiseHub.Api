using System.ComponentModel.DataAnnotations;

namespace MiseHub.Api.DTOs;

public class CreateOrderLineItem
{
    [Required]
    public string ProductId { get; set; } = string.Empty;
    [Required]
    [StringLength(50,MinimumLength = 5)]
    public string Name { get; set; } = string.Empty;
    [Required]
    [StringLength(50)]
    public string Unit { get; set; } = string.Empty;
    [Range(0.01, 1_000_000, ErrorMessage = "Price must be greater than 0.")]
    public decimal Price { get; set; }

    [Range(1, 10_000, ErrorMessage = "Quantity must be between 1 and 10 000.")]
    public int Quantity { get; set; }
}
