using Microsoft.EntityFrameworkCore;
using Project.Data;
using Project.DTOs;
using Project.Exceptions;
using Project.Models;

namespace Project.Services;

public class EmployeeService : IEmployeeService
{
    private readonly DatabaseContext _context;
    private readonly ITokenService _tokenService;

    public EmployeeService(DatabaseContext context, ITokenService tokenService)
    {
        _context = context;
        _tokenService = tokenService;
    }

    public async Task RegisterAsync(RegisterRequest dto)
    {
        if (await _context.Employees.AnyAsync(e => e.Login == dto.Login))
            throw new BadRequestException("Login already taken");
        
        if (!Enum.TryParse<Role>(dto.Role, ignoreCase: true, out var parsedRole))
            throw new BadRequestException("Invalid role specified. Allowed values: User, Admin.");
        
        var employee = new Employee
        {
            Login = dto.Login,
            PasswordHash = PasswordHelper.HashPassword(dto.Password),
            Role = parsedRole,
            RefreshToken = _tokenService.GenerateRefreshToken(),
            RefreshTokenExpiry = DateTime.UtcNow.AddDays(1)
        };

        _context.Employees.Add(employee);
        await _context.SaveChangesAsync();
    }

    public async Task<(string Token, string RefreshToken)> LoginAsync(LoginRequest request)
    {
        var employee = await _context.Employees
            .FirstOrDefaultAsync(e => e.Login == request.Login);

        if (employee == null || !PasswordHelper.VerifyPassword(request.Password, employee.PasswordHash))
            throw new UnauthorizedException("Invalid credentials");
        
        var token = _tokenService.GenerateToken(employee);
        var refreshToken = _tokenService.GenerateRefreshToken();

        employee.RefreshToken = refreshToken;
        employee.RefreshTokenExpiry = DateTime.UtcNow.AddDays(1);
        await _context.SaveChangesAsync();
        return (token, refreshToken);
    }

    public async Task<(string Token, string RefreshToken)> RefreshAsync(RefreshTokenRequest request)
    {
        var employee = await _context.Employees
            .FirstOrDefaultAsync(e => e.RefreshToken == request.RefreshToken);

        if (employee == null || employee.RefreshTokenExpiry <= DateTime.UtcNow)
            throw new UnauthorizedException("Invalid or expired refresh token");

        var newToken = _tokenService.GenerateToken(employee);
        var newRefreshToken = _tokenService.GenerateRefreshToken();

        employee.RefreshToken = newRefreshToken;
        employee.RefreshTokenExpiry = DateTime.UtcNow.AddDays(1);
        await _context.SaveChangesAsync();
        return (newToken, newRefreshToken);
    }
}