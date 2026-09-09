using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiseHub.Api.DTOs;
using MiseHub.Api.Interfaces;

namespace MiseHub.Api.Controllers;


[ApiController]
[Route("api/[controller]")]
[AllowAnonymous]
public class AuthController : ControllerBase
{
    private readonly IAuthService _auth;

    public AuthController(IAuthService auth)
    {
        _auth = auth;
    }

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register(RegisterRequest request)
    {
        var result = await _auth.RegisterAsync(request);
        return result is null ? Conflict("An account with this email already exist") : Ok(result) ;
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest request)
    {
        var result = await _auth.LoginAsync(request);
        return result is null ? Unauthorized("Invalid email or password") : Ok(result) ;
    }

}
