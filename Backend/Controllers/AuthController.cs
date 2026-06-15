using Microsoft.AspNetCore.Mvc;
using WebApi.Services;
using WebApi.Models;

[ApiController]
[Route("api/[controller]")]  // /api/auth
public class AuthController : ControllerBase
{
    private readonly ILogger<AuthController> _logger;
    private readonly ILoginService _loginService;

    public AuthController(ILoginService loginService, ILogger<AuthController> logger)
    {
        _loginService = loginService;
        _logger = logger;
    }

    // POST: /api/auth/login
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {

        var clientIp = HttpContext.Connection.RemoteIpAddress?.ToString();
        _logger.LogInformation($"[*] {DateTime.Now} | {clientIp} -> POST /api/auth/login ({request.Login})");

        string token = await _loginService.ValidateUserLogin(request.Login, request.Password);

        return token != null ? Ok(new { accessToken = token }) : Unauthorized();
    }

    // GET: /api/auth/profile
    [HttpGet("profile")]
    public async Task<IActionResult> Profile()
    {
        // Вытаскивает токен из заголовка Authorization
        var authorizationHeader = Request.Headers["Authorization"].ToString();

        if (string.IsNullOrEmpty(authorizationHeader))
            return BadRequest("Authorization header is missing");

        if (!authorizationHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            return BadRequest("Invalid authorization header format. Expected 'Bearer <token>'");

        var token = authorizationHeader.Substring("Bearer ".Length).Trim();

        if (string.IsNullOrEmpty(token))
            return BadRequest("Token is empty");

        var clientIp = HttpContext.Connection.RemoteIpAddress?.ToString();
        _logger.LogInformation($"[*] {DateTime.Now} | {clientIp} -> GET /api/auth/profile");

        bool result = await _loginService.CheckJwtToken(token);

        return result == true ? Ok() : Unauthorized();
    }
}