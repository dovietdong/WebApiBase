using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApiBase.Models.DTOs;
using WebApiBase.Services;

namespace WebApiBase.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    /// <summary>
    /// Get all users (Admin only)
    /// </summary>
    [HttpGet]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(typeof(ApiResponse<List<UserDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<List<UserDto>>>> GetAllUsers()
    {
        var users = await _userService.GetAllUsersAsync();

        return Ok(new ApiResponse<List<UserDto>>
        {
            Success = true,
            Message = "Lấy danh sách user thành công",
            Data = users
        });
    }

    /// <summary>
    /// Get user by ID (User can only get their own info, Admin can get any)
    /// </summary>
    [HttpGet("{id}")]
    [Authorize(Policy = "UserOrAdmin")]
    [ProducesResponseType(typeof(ApiResponse<UserDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<UserDto>>> GetUserById(int id)
    {
        // Check if user is trying to access their own info or is Admin
        var currentUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        var currentUserRole = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;

        if (currentUserRole != "Admin" && currentUserId != id.ToString())
        {
            return Forbid();
        }

        var user = await _userService.GetUserByIdAsync(id);

        if (user == null)
        {
            return NotFound(new ApiResponse<UserDto>
            {
                Success = false,
                Message = "Không tìm thấy user"
            });
        }

        return Ok(new ApiResponse<UserDto>
        {
            Success = true,
            Message = "Lấy thông tin user thành công",
            Data = user
        });
    }
}