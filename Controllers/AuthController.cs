using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApiBase.Models.DTOs;
using WebApiBase.Services;

namespace WebApiBase.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// Login user
    /// </summary>
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<AuthResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<AuthResponse>>> Login([FromBody] LoginRequest request)
    {
        var result = await _authService.LoginAsync(request);

        if (result == null)
        {
            return Unauthorized(new ApiResponse<AuthResponse>
            {
                Success = false,
                Message = "Email hoặc mật khẩu không đúng"
            });
        }

        return Ok(new ApiResponse<AuthResponse>
        {
            Success = true,
            Message = "Đăng nhập thành công",
            Data = result
        });
    }

    /// <summary>
    /// Register new user
    /// </summary>
    [HttpPost("register")]
    [ProducesResponseType(typeof(ApiResponse<AuthResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<AuthResponse>>> Register([FromBody] RegisterRequest request)
    {
        var result = await _authService.RegisterAsync(request);

        if (result == null)
        {
            return BadRequest(new ApiResponse<AuthResponse>
            {
                Success = false,
                Message = "Email đã được sử dụng"
            });
        }

        return CreatedAtAction(nameof(Register), new ApiResponse<AuthResponse>
        {
            Success = true,
            Message = "Đăng ký thành công",
            Data = result
        });
    }
}