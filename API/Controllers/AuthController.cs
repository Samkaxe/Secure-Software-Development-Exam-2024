using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(IUserService userService) : Controller
{
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDTO request)
    {
        try
        {
            var token = await userService.LoginAsync(request.Email, request.Password);
            return Ok(token);
        }
        catch (Exception e)
        {
            return Unauthorized(new { Message = e.Message });
        }
    }
    
    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] CreateUserDTO registerUserDTO)
    {
        // Validate the input
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // Attempt registration
        try
        {
            var user = await userService.CreateUserAsync(registerUserDTO);
            return CreatedAtAction(nameof(Login), new { id = user.Id }, new
            {
                user.Id,
                user.Email,
                user.Role
            });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }
}