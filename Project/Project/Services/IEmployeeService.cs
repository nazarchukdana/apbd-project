using Project.DTOs;

namespace Project.Services;

public interface IEmployeeService
{
    Task RegisterAsync(RegisterRequest dto);
    Task<(string Token, string RefreshToken)> LoginAsync(LoginRequest request);
    Task<(string Token, string RefreshToken)> RefreshAsync(RefreshTokenRequest request);
}