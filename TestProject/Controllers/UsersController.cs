using Microsoft.AspNetCore.Mvc;
using TestProject.Models;
using TestProject.Services;

namespace TestProject.Controllers;

[ApiController]
[Route("[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    
    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    /// <summary>
    /// Get user details by id
    /// </summary>
    /// <param name="userId"></param>
    /// <response code="200">User model</response>
    /// <response code="404">User not found</response>
    [HttpGet("{userId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(string))]
    public async Task<ActionResult<UserViewModel>> GetUser(string userId)
    {
        UserViewModel? user = await _userService.GetUserInfo(userId);

        if (user is null) return NotFound();
        
        return user;
    }

    /// <summary>
    /// Returns user model by login and password
    /// </summary>
    /// <param name="model"></param>
    /// <response code="200">User model</response>
    /// <response code="400">Bad params</response>
    /// <response code="401">Bad credentials</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(string))]
    public async Task<ActionResult<UserViewModel>> Login(UserLoginModel model)
    {
        if (string.IsNullOrEmpty(model.Email)) return BadRequest(nameof(UserLoginModel.Email) + " is empty");
        if (string.IsNullOrEmpty(model.Password)) return BadRequest(nameof(UserLoginModel.Password) + " is empty");

        UserViewModel? user = await _userService.Login(model.Email, model.Password);

        if (user is null) return Unauthorized("wrong email or/and password");

        return user;
    }
}