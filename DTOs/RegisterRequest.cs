using System.ComponentModel.DataAnnotations;

namespace MiseHub.Api.DTOs;

public class RegisterRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
    [Required]
    [StringLength(100,MinimumLength =8,ErrorMessage ="Password must atleast be 8 characters.")]
    public string Password { get; set; } = string.Empty;
    [Required]
    [StringLength(100,MinimumLength =1)]
    public string RestaurantName { get; set; } = string.Empty;
}
