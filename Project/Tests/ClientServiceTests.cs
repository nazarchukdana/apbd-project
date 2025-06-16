using Microsoft.EntityFrameworkCore;
using Project.Data;
using Project.DTOs;
using Project.Exceptions;
using Project.Models;
using Project.Models.Clients;
using Project.Services;

namespace Tests;

public class ClientServiceTests
{
    private DatabaseContext _context;
    private ClientService _service;

    [SetUp]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<DatabaseContext>()
            .UseInMemoryDatabase("TestDb_" + Guid.NewGuid())
            .Options;

        _context = new DatabaseContext(options);
        _service = new ClientService(_context);

        _context.IndividualClients.Add(new IndividualClient
        {
            Id = 1,
            FirstName = "Jan",
            LastName = "Kowalski",
            Email = "jan@email.com",
            PhoneNumber = "+48123456789",
            Address = "Warsaw",
            Pesel = "12345678901"
        });

        _context.CompanyClients.Add(new CompanyClient
        {
            Id = 2,
            CompanyName = "TestCompany",
            Email = "company@email.com",
            PhoneNumber = "+48111222333",
            Address = "Krakow",
            KrsNumber = "0000000001"
        });

        _context.SaveChanges();
    }

    [TearDown]
    public void TearDown()
    {
        _context?.Dispose();
    }

    [Test]
    public async Task GetIndividualByIdAsync_ReturnsClient()
    {
        var result = await _service.GetIndividualByIdAsync(1);
        Assert.AreEqual("Jan", result.FirstName);
    }

    [Test]
    public void GetIndividualByIdAsync_ThrowsNotFound()
    {
        Assert.ThrowsAsync<NotFoundException>(async () => { await _service.GetIndividualByIdAsync(99); });
    }

    [Test]
    public async Task GetCompanyByIdAsync_ReturnsClient()
    {
        var result = await _service.GetCompanyByIdAsync(2);
        Assert.AreEqual("TestCompany", result.CompanyName);
    }

    [Test]
    public void GetCompanyByIdAsync_ThrowsNotFound()
    {
        Assert.ThrowsAsync<NotFoundException>(async () => { await _service.GetCompanyByIdAsync(99); });
    }

    [Test]
    public async Task GetAllClientsAsync_ReturnsBoth()
    {
        var list = await _service.GetAllClientsAsync();
        Assert.AreEqual(2, list.Count);
    }

    [Test]
    public async Task AddIndividualClientAsync_AddsClient()
    {
        var dto = new CreateIndividualDto
        {
            FirstName = "Anna",
            LastName = "Nowak",
            Email = "anna@email.com",
            PhoneNumber = "+48123456700",
            Address = "Gdansk",
            Pesel = "22222222222"
        };

        var result = await _service.AddIndividualClientAsync(dto);
        Assert.AreEqual("Anna", result.FirstName);
    }

    [Test]
    public void AddIndividualClientAsync_ThrowsConflict_OnDuplicatePesel()
    {
        var dto = new CreateIndividualDto
        {
            FirstName = "Test",
            LastName = "User",
            Email = "new@email.com",
            PhoneNumber = "+48123456788",
            Address = "Poznan",
            Pesel = "12345678901"
        };

        Assert.ThrowsAsync<ConflictException>(async () => { await _service.AddIndividualClientAsync(dto); });
    }

    [Test]
    public void AddIndividualClientAsync_ThrowsConflict_OnDuplicateEmail()
    {
        var dto = new CreateIndividualDto
        {
            FirstName = "Test",
            LastName = "User",
            Email = "jan@email.com",
            PhoneNumber = "+48123456788",
            Address = "Poznan",
            Pesel = "55555555555"
        };

        Assert.ThrowsAsync<ConflictException>(async () => { await _service.AddIndividualClientAsync(dto); });
    }

    [Test]
    public async Task AddCompanyClientAsync_AddsClient()
    {
        var dto = new CreateCompanyDto
        {
            CompanyName = "NewCo",
            Email = "newco@email.com",
            PhoneNumber = "+4844445555",
            Address = "Lodz",
            KrsNumber = "0000000002"
        };

        var result = await _service.AddCompanyClientAsync(dto);
        Assert.AreEqual("NewCo", result.CompanyName);
    }

    [Test]
    public void AddCompanyClientAsync_ThrowsConflict_OnDuplicateKrs()
    {
        var dto = new CreateCompanyDto
        {
            CompanyName = "Another",
            Email = "another@email.com",
            PhoneNumber = "+4844445555",
            Address = "Lodz",
            KrsNumber = "0000000001"
        };

        Assert.ThrowsAsync<ConflictException>(async () => { await _service.AddCompanyClientAsync(dto); });
    }

    [Test]
    public void AddCompanyClientAsync_ThrowsConflict_OnDuplicateEmail()
    {
        var dto = new CreateCompanyDto
        {
            CompanyName = "Another",
            Email = "company@email.com",
            PhoneNumber = "+4844445555",
            Address = "Lodz",
            KrsNumber = "0000000002"
        };

        Assert.ThrowsAsync<ConflictException>(async () => { await _service.AddCompanyClientAsync(dto); });
    }

    [Test]
    public async Task UpdateIndividualClientAsync_UpdatesClient()
    {
        var dto = new UpdateIndividualDto
        {
            FirstName = "Updated",
            LastName = "Kowalski",
            Email = "updated@email.com",
            PhoneNumber = "+4899998888",
            Address = "Warsaw"
        };

        var result = await _service.UpdateIndividualClientAsync(1, dto);
        Assert.AreEqual("Updated", result.FirstName);
    }

    [Test]
    public void UpdateIndividualClientAsync_ThrowsNotFound()
    {
        var dto = new UpdateIndividualDto
        {
            FirstName = "X",
            LastName = "Y",
            Email = "x@y.com",
            PhoneNumber = "+4811111111",
            Address = "Nowhere"
        };

        Assert.ThrowsAsync<NotFoundException>(async () => { await _service.UpdateIndividualClientAsync(999, dto); });
    }

    [Test]
    public async Task RemoveClientAsync_SoftDeletesIndividual()
    {
        await _service.RemoveClientAsync(1);
        var client = await _context.IndividualClients.FindAsync(1);
        Assert.IsTrue(client.IsDeleted);
    }

    [Test]
    public void RemoveClientAsync_ThrowsConflict_ForCompany()
    {
        Assert.ThrowsAsync<ConflictException>(async () => { await _service.RemoveClientAsync(2); });
    }
    [Test]
    public async Task RemoveClientAsync_ThrowsConflict_WhenIndividualHasActiveContracts()
    {
        var client = await _context.IndividualClients.FindAsync(1);
        client.Contracts.Add(new Contract
        {
            ClientId = client.Id,
            SoftwareSystemId = 1,
            SoftwareVersionId = 1,
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(10),
            Price = 100,
            Status = ContractStatus.Active
        });
        await _context.SaveChangesAsync();

        var ex = Assert.ThrowsAsync<ConflictException>(async () => await _service.RemoveClientAsync(1));
        Assert.That(ex.Message, Does.Contain("active contracts"));
    }
    [Test]
    public async Task RemoveClientAsync_SoftDeletesIndividual_WhenNoActiveContracts()
    {
        var client = await _context.IndividualClients.FindAsync(1);
        client.Contracts.Add(new Contract
        {
            ClientId = client.Id,
            SoftwareSystemId = 1,
            SoftwareVersionId = 1,
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(10),
            Price = 100,
            Status = ContractStatus.Cancelled
        });
        await _context.SaveChangesAsync();

        await _service.RemoveClientAsync(1);

        var softDeleted = await _context.IndividualClients.FindAsync(1);
        Assert.IsTrue(softDeleted.IsDeleted);
    }
}