namespace MiseHub.Api.Models;

public class User
{
    public string Id { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string RestaurantName { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
}