using MiseHub.Api.Interfaces;
using MiseHub.Api.Models;
using System.Security.Cryptography;

namespace MiseHub.Api.Services.Fake;

using Microsoft.AspNetCore.Identity;
using MiseHub.Api.DTOs;

public class FakeAuthService : IAuthService
{

    private readonly List<User> _users = new();
    private readonly IConfiguration _config;
    private readonly PasswordHasher<User> _passwordHasher = new();
    public FakeAuthService(IConfiguration config)
    {
        _config= config;
    }

    public Task<AuthResponse?> RegisterAsync(RegisterRequest request)
    {
        var email = request.Email.Trim().ToLowerInvariant();

        if (_users.Any(u => u.Email == email))
        {
            return Task.FromResult<AuthResponse?>(null);
        }

        var user = new User
        {

            //Skapar ett garanterat unikt ID.
            //Guid = "Globally Unique Identifier", i praktiken omöjligt att två anrop råkar generera samma.
            //"N"-formatet gör att den blir en ren sträng utan bindestreck.
            Id = Guid.NewGuid().ToString("N"),
            Email = email,
            RestaurantName = request.RestaurantName.Trim()
        };

        // Salt + 100 000 hashningar + kombinerar allt till en sparbar sträng, i EN rad
        user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);
        _users.Add(user);

        return Task.FromResult<AuthResponse?>(new AuthResponse
        {
            Token = "TODO",
            Email = user.Email,
            RestaurantName = user.RestaurantName,
        });
    }
}
