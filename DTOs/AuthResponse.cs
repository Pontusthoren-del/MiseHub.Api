namespace MiseHub.Api.DTOs;

public class AuthResponse
{
    public string Token { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string RestaurantName { get; set; } = string.Empty;
}
