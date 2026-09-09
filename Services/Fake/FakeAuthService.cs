using MiseHub.Api.Interfaces;
using MiseHub.Api.Models;
using Microsoft.AspNetCore.Identity;
using MiseHub.Api.DTOs;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace MiseHub.Api.Services.Fake;

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
            Token = GenerateToken(user),
            Email = user.Email,
            RestaurantName = user.RestaurantName,
        });
    }

    public Task<AuthResponse?> LoginAsync(LoginRequest request)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        var user = _users.FirstOrDefault(u => u.Email == email); // null om mailet inte finns

        if (user is null)
        {
            return Task.FromResult<AuthResponse?>(null); // samma null oavsett fel — se nedan
        }

        // Räknar om hash från request.Password och jämför mot user.PasswordHash internt
        var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
        if (result == PasswordVerificationResult.Failed)
        {
            return Task.FromResult<AuthResponse?>(null); // fel lösenord — samma null som "mail saknas"
        }
        return Task.FromResult<AuthResponse?>(new AuthResponse
        {
            Token = GenerateToken(user),
            Email = user.Email,
            RestaurantName = user.RestaurantName,
        });
    }

    private string GenerateToken(User user)
    {
        // "Claims" = påståenden om användaren som lagras i token:en.
        // OBS: bara läsbar info här — aldrig PasswordHash eller annat känsligt,
        // JWT är bara base64-kodad, inte krypterad, vem som helst kan läsa innehållet.
        var claims = new[]
        {
        new Claim(JwtRegisteredClaimNames.Sub, user.Id),
        new Claim(JwtRegisteredClaimNames.Email, user.Email),
        new Claim("restaurant_name", user.RestaurantName)
    };

        // Hemliga nyckeln från appsettings.json, gjord om till ett kryptografiskt nyckel-objekt
        var key = _config["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key missing in config");
        var signingKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(key));
        var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(8), // token slutar gälla efter 8h — måste logga in igen då
            signingCredentials: credentials
            );

        // Gör om token-objektet till den riktiga JWT-textsträngen som skickas till klienten
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

