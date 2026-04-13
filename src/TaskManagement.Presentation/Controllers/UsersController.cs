using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.Application.Services;
using TaskManagement.Core.Entities;
using TaskManagement.Presentation.Security;

namespace TaskManagement.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly UserService _userService;
    private readonly JwtTokenService _tokenService;

    public UsersController(UserService userService, JwtTokenService tokenService)
    {
        _userService = userService;
        _tokenService = tokenService;
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] RegisterUserRequest request)
    {
        try
        {
            var user = new User
            {
                Username = request.Username,
                Email = request.Email,
                PasswordHash = request.Password
            };
            await _userService.RegisterUserAsync(user);
            return CreatedAtAction(nameof(GetById), new { id = user.Id }, new { user.Id, user.Username, user.Email });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var user = await _userService.ValidateUserAsync(request.Username, request.Password);
        if (user == null)
            return Unauthorized("Invalid credentials");

        var token = _tokenService.CreateToken(user);
        return Ok(new
        {
            token,
            user = new { user.Id, user.Username, user.Email }
        });
    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<IActionResult> GetById(int id)
    {
        var user = await _userService.GetUserByIdAsync(id);
        if (user == null)
            return NotFound();

        return Ok(new { user.Id, user.Username, user.Email });
    }

    [HttpGet("public-ping")]
    [AllowAnonymous]
    public IActionResult PublicPing()
    {
        return Ok(new { Message = "Public endpoint reachable" });
    }

    [HttpGet("me")]
    [Authorize]
    public IActionResult Me()
    {
        var userId = User.FindFirstValue("userId");
        var username = User.Identity?.Name;

        return Ok(new
        {
            UserId = userId,
            Username = username,
            Message = "Authorized endpoint reachable"
        });
    }
}

public class RegisterUserRequest
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class LoginRequest
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}