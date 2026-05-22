using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using TrainTicketApi.DTOs.Auth;

namespace TrainTicketApi.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly IConfiguration _configuration;

    public AuthController(
        UserManager<IdentityUser> userManager,
        SignInManager<IdentityUser> signInManager,
        IConfiguration configuration)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _configuration = configuration;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
    {
        var user = await _userManager.FindByNameAsync(request.UserName.Trim());
        if (user is null)
        {
            return Unauthorized(new { message = "Sai tÃªn Ä‘Äƒng nháº­p hoáº·c máº­t kháº©u." });
        }

        var signInResult = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
        if (!signInResult.Succeeded)
        {
            return Unauthorized(new { message = "Sai tÃªn Ä‘Äƒng nháº­p hoáº·c máº­t kháº©u." });
        }

        var roles = await _userManager.GetRolesAsync(user);
        var role = roles.FirstOrDefault() ?? "Staff";
        var token = GenerateJwtToken(user, role);

        return Ok(new
        {
            message = "ÄÄƒng nháº­p thÃ nh cÃ´ng.",
            data = token
        });
    }

    private LoginResponseDto GenerateJwtToken(IdentityUser user, string role)
    {
        var jwtSection = _configuration.GetSection("Jwt");
        var key = jwtSection["Key"] ?? throw new InvalidOperationException("Missing Jwt:Key.");
        var issuer = jwtSection["Issuer"] ?? "TrainTicketApi";
        var audience = jwtSection["Audience"] ?? "TrainTicketWinForms";
        var expireMinutes = int.TryParse(jwtSection["ExpireMinutes"], out var minutes) ? minutes : 480;

        var expiresAt = DateTime.UtcNow.AddMinutes(expireMinutes);
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id),
            new(JwtRegisteredClaimNames.UniqueName, user.UserName ?? string.Empty),
            new(ClaimTypes.Name, user.UserName ?? string.Empty),
            new(ClaimTypes.Role, role)
        };

        var jwtToken = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: expiresAt,
            signingCredentials: credentials);

        var tokenValue = new JwtSecurityTokenHandler().WriteToken(jwtToken);
        return new LoginResponseDto
        {
            AccessToken = tokenValue,
            ExpiresAtUtc = expiresAt,
            UserName = user.UserName ?? string.Empty,
            Role = role
        };
    }
}
