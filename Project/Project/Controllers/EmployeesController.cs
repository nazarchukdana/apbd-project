using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project.DTOs;
using Project.Exceptions;
using Project.Services;

namespace Project.Controllers;

[ApiController]
[Route("api/[controller]")]
[AllowAnonymous]
public class EmployeesController : ControllerBase
{
    private readonly IEmployeeService _employeeService;

    public EmployeesController(IEmployeeService employeeService)
    {
        _employeeService = employeeService;
    }
    [HttpPost("register")]
    [ProducesResponseType(typeof(string), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        try
        {
            await _employeeService.RegisterAsync(request);
            return Ok("Registered");
        }
        catch (BadRequestException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }
    [HttpPost("login")]
    [ProducesResponseType(typeof(object), 200)]
    [ProducesResponseType(401)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        try
        {
            var (token, refreshToken) = await _employeeService.LoginAsync(request);
            return Ok(new { Token = token, RefreshToken = refreshToken });
        }
        catch (UnauthorizedException ex)
        {
            return Unauthorized(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }
    
    [HttpPost("refresh")]
    [ProducesResponseType(typeof(object), 200)]
    [ProducesResponseType(401)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> Refresh(RefreshTokenRequest request)
    {
        try
        {
            var (token, refreshToken) = await _employeeService.RefreshAsync(request);
            return Ok(new { Token = token, RefreshToken = refreshToken });
        }
        catch (UnauthorizedException ex)
        {
            return Unauthorized(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }
    
}