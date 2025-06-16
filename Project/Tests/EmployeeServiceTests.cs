using Microsoft.EntityFrameworkCore;
using Project.Data;
using Project.DTOs;
using Project.Exceptions;
using Project.Models;
using Project.Services;

namespace Tests;

public class EmployeeServiceTests
{
    private DatabaseContext _context;
    private EmployeeService _service;
    private FakeTokenService _fakeTokenService;

    [SetUp]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<DatabaseContext>()
            .UseInMemoryDatabase("TestDb_" + Guid.NewGuid())
            .Options;

        _context = new DatabaseContext(options);

        _fakeTokenService = new FakeTokenService();
        _service = new EmployeeService(_context, _fakeTokenService);
    }

    [TearDown]
    public void TearDown()
    {
        _context?.Dispose();
    }

    [Test]
    public async Task RegisterAsync_AddsEmployee_WithValidData()
    {
        var request = new RegisterRequest
        {
            Login = "testuser",
            Password = "Password123",
            Role = "User"
        };

        await _service.RegisterAsync(request);

        var employee = await _context.Employees.FirstOrDefaultAsync(e => e.Login == "testuser");
        Assert.IsNotNull(employee);
        Assert.AreEqual(Role.User, employee.Role);
        Assert.IsNotEmpty(employee.PasswordHash);
        Assert.IsNotEmpty(employee.RefreshToken);
    }

    [Test]
    public async Task RegisterAsync_Throws_WhenLoginTaken()
    {
        _context.Employees.Add(new Employee
        {
            Login = "existing",
            PasswordHash = "hash",
            Role = Role.User,
            RefreshToken = "initial-refresh",
            RefreshTokenExpiry = DateTime.UtcNow.AddMinutes(10)
        });
        await _context.SaveChangesAsync();

        var request = new RegisterRequest
        {
            Login = "existing",
            Password = "Password123",
            Role = "User"
        };

        Assert.ThrowsAsync<BadRequestException>(async () =>
        {
            await _service.RegisterAsync(request);
        });
    }

    [Test]
    public void RegisterAsync_Throws_WhenRoleInvalid()
    {
        var request = new RegisterRequest
        {
            Login = "newuser",
            Password = "Password123",
            Role = "SuperAdmin"
        };

        Assert.ThrowsAsync<BadRequestException>(async () =>
        {
            await _service.RegisterAsync(request);
        });
    }

    [Test]
    public async Task LoginAsync_ReturnsTokenAndRefresh_WhenValid()
    {
        var employee = new Employee
        {
            Login = "user1",
            PasswordHash = PasswordHelper.HashPassword("Password123"),
            Role = Role.User,
            
            RefreshToken = "initial-refresh",
            RefreshTokenExpiry = DateTime.UtcNow.AddMinutes(10)
        };
        await _context.Employees.AddAsync(employee);
        await _context.SaveChangesAsync();

        var request = new LoginRequest
        {
            Login = "user1",
            Password = "Password123"
        };

        var (token, refresh) = await _service.LoginAsync(request);

        Assert.AreEqual("fake-jwt", token);
        Assert.AreEqual("fake-refresh", refresh);

        var updated = await _context.Employees.FirstAsync(e => e.Login == "user1");
        Assert.AreEqual("fake-refresh", updated.RefreshToken);
    }

    [Test]
    public void LoginAsync_Throws_WhenInvalid()
    {
        var request = new LoginRequest
        {
            Login = "nouser",
            Password = "Password123"
        };

        Assert.ThrowsAsync<UnauthorizedException>(async () =>
        {
            await _service.LoginAsync(request);
        });
    }

    [Test]
    public async Task RefreshAsync_ReturnsNewTokens_WhenValid()
    {
        var employee = new Employee
        {
            Login = "user2",
            PasswordHash = "hash",
            Role = Role.User,
            RefreshToken = "valid-refresh",
            RefreshTokenExpiry = DateTime.UtcNow.AddMinutes(10)
        };
        _context.Employees.Add(employee);
        await _context.SaveChangesAsync();

        var request = new RefreshTokenRequest
        {
            RefreshToken = "valid-refresh"
        };

        var (token, newRefresh) = await _service.RefreshAsync(request);

        Assert.AreEqual("fake-jwt", token);
        Assert.AreEqual("fake-refresh", newRefresh);
    }

    [Test]
    public void RefreshAsync_Throws_WhenInvalidOrExpired()
    {
        var employee = new Employee
        {
            Login = "user3",
            PasswordHash = "hash",
            Role = Role.User,
            RefreshToken = "expired-refresh",
            RefreshTokenExpiry = DateTime.UtcNow.AddMinutes(-5)
        };
        _context.Employees.Add(employee);
        _context.SaveChanges();

        var request = new RefreshTokenRequest
        {
            RefreshToken = "expired-refresh"
        };

        Assert.ThrowsAsync<UnauthorizedException>(async () =>
        {
            await _service.RefreshAsync(request);
        });
    }

    private class FakeTokenService : ITokenService
    {
        public string GenerateToken(Employee employee) => "fake-jwt";
        public string GenerateRefreshToken() => "fake-refresh";
    }
}
