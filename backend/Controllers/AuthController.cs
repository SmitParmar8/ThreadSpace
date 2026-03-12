using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ThreadSpace.API.Data;
using ThreadSpace.API.DTOs;
using ThreadSpace.API.Models;

namespace ThreadSpace.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IConfiguration _config;

    public AuthController(AppDbContext db, IConfiguration config)
    {
        _db = db;
        _config = config;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        if (_db.Users.Any(u => u.Email == dto.Email))
            return BadRequest(new { message = "Email already in use" });

        if (_db.Users.Any(u => u.Username == dto.Username))
            return BadRequest(new { message = "Username already taken" });

        var validLevels = new[] { "Student", "Fresher", "Mid-Level", "Pro" };
        var userLevel = validLevels.Contains(dto.UserLevel) ? dto.UserLevel : "Student";

        var user = new User
        {
            Name = dto.Name,
            Email = dto.Email,
            Username = dto.Username,
            Password = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            UserLevel = userLevel,
            Role = "User"
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        var token = GenerateToken(user);
        return Ok(new { token, user = new { user.Id, user.Username, user.Email, user.Name, user.UserLevel, user.Role } });
    }

    [HttpPost("login")]
    public IActionResult Login(LoginDto dto)
    {
        var user = _db.Users.FirstOrDefault(u => u.Email == dto.Email);
        if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.Password))
            return Unauthorized(new { message = "Invalid credentials" });

        var token = GenerateToken(user);
        return Ok(new { token, user = new { user.Id, user.Username, user.Email, user.Name, user.UserLevel, user.Role } });
    }

    [Authorize]
    [HttpPost("logout")]
    public IActionResult Logout()
    {
        return Ok(new { message = "Logged out successfully" });
    }

    private string GenerateToken(User user)
    {
        var key = Encoding.ASCII.GetBytes(_config["Jwt:Secret"]!);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role)
            }),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };
        var handler = new JwtSecurityTokenHandler();
        return handler.WriteToken(handler.CreateToken(tokenDescriptor));
    }
}
