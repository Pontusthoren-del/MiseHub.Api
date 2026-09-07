using System.ComponentModel.DataAnnotations;

namespace MiseHub.Api.DTOs;

public class RegisterRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
    [Required]
    public string Password { get; set; } = string.Empty;
    [Required]
    public string RestaurantName { get; set; } = string.Empty;
}
