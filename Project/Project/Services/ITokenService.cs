using Project.Models;

namespace Project.Services;

public interface ITokenService
{
    string GenerateToken(Employee employee);
    string GenerateRefreshToken();
}