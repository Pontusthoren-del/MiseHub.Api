using MiseHub.Api.DTOs;

namespace MiseHub.Api.Interfaces;

public interface IAuthService
{
    // null = registreringen misslyckades (t.ex. mailet redan taget)
    Task<AuthResponse?> RegisterAsync(RegisterRequest request);

    // null = fel email/lösenord (samma svar för båda, avslöjar inte vilket)
    Task<AuthResponse?> LoginAsync(LoginRequest request);
}