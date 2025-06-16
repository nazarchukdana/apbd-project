using Microsoft.EntityFrameworkCore;
using Project.Data;
using Project.DTOs;
using Project.Exceptions;
using Project.Models;
using Project.Services;

namespace Tests;

public class ContractServiceTests
{
    private DatabaseContext _context;
    private ContractService _service;

    [SetUp]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<DatabaseContext>()
            .UseInMemoryDatabase("TestDb_" + Guid.NewGuid())
            .Options;

        _context = new DatabaseContext(options);
        _service = new ContractService(_context);

        _context.Clients.Add(new Project.Models.Clients.CompanyClient
        {
            Id = 1,
            CompanyName = "TestCo",
            Email = "company@email.com",
            KrsNumber = "0000000001",
            Address = "Address",
            PhoneNumber = "+48123456789"
        });

        _context.SoftwareSystems.Add(new SoftwareSystem
        {
            Id = 1,
            Name = "TestSoftware",
            Category = "Test",
            Description = "Test Desc",
            SubscriptionCost = 50,
            UpfrontCost = 1000
        });

        _context.Versions.Add(new SoftwareVersion
        {
            Id = 1,
            Name = "1.0",
            SoftwareSystemId = 1,
            IsCurrent = true
        });

        _context.Discounts.Add(new Discount
        {
            Id = 1,
            Name = "Test Discount",
            DiscountType = DiscountType.UpfrontCost,
            Percentage = 10,
            StartDate = DateTime.Today.AddDays(-1),
            EndDate = DateTime.Today.AddDays(1)
        });

        _context.SaveChanges();
    }

    [TearDown]
    public void TearDown()
    {
        _context.Dispose();
    }

    [Test]
    public async Task AddContractAsync_CalculatesPriceWithDiscountsAndSupportYears()
    {
        var dto = new CreateContractDto
        {
            ClientId = 1,
            SoftwareSystemId = 1,
            SoftwareVersionId = 1,
            StartDate = DateTime.Today,
            EndDate = DateTime.Today.AddDays(10),
            SupportYears = 1
        };

        var response = await _service.AddContractAsync(dto);
        
        Assert.AreEqual(1900, response.Price);
    }

    [Test]
    public void AddContractAsync_ThrowsBadRequest_ForInvalidPeriod()
    {
        var dto = new CreateContractDto
        {
            ClientId = 1,
            SoftwareSystemId = 1,
            SoftwareVersionId = 1,
            StartDate = DateTime.Today,
            EndDate = DateTime.Today.AddDays(2),
            SupportYears = 0
        };

        Assert.ThrowsAsync<BadRequestException>(async () =>
        {
            await _service.AddContractAsync(dto);
        });
    }

    [Test]
    public async Task PayForContractAsync_SignsContractOnFullPayment()
    {
        var contract = new Contract
        {
            Id = 1,
            ClientId = 1,
            SoftwareSystemId = 1,
            SoftwareVersionId = 1,
            Price = 500,
            Status = ContractStatus.Active,
            StartDate = DateTime.Today,
            EndDate = DateTime.Today.AddDays(10)
        };
        _context.Contracts.Add(contract);
        await _context.SaveChangesAsync();

        var dto = new CompletePaymentDto
        {
            Amount = 500
        };

        var response = await _service.PayForContractAsync(1, dto);

        Assert.AreEqual("Signed", response.Status);
        Assert.AreEqual(500, response.Paid);
        Assert.AreEqual(0, response.LeftToPay);
    }

    [Test]
    public async Task PayForContractAsync_PartialPayment_DoesNotSign()
    {
        var contract = new Contract
        {
            Id = 2,
            ClientId = 1,
            SoftwareSystemId = 1,
            SoftwareVersionId = 1,
            Price = 500,
            Status = ContractStatus.Active,
            StartDate = DateTime.Today,
            EndDate = DateTime.Today.AddDays(10)
        };
        _context.Contracts.Add(contract);
        await _context.SaveChangesAsync();

        var dto = new CompletePaymentDto
        {
            Amount = 200
        };

        var response = await _service.PayForContractAsync(2, dto);

        Assert.AreEqual("Active", response.Status);
        Assert.AreEqual(200, response.Paid);
        Assert.AreEqual(300, response.LeftToPay);
    }

    [Test]
    public void PayForContractAsync_ThrowsForOverpayment()
    {
        var contract = new Contract
        {
            Id = 3,
            ClientId = 1,
            SoftwareSystemId = 1,
            SoftwareVersionId = 1,
            Price = 500,
            Status = ContractStatus.Active,
            StartDate = DateTime.Today,
            EndDate = DateTime.Today.AddDays(10)
        };
        _context.Contracts.Add(contract);
        _context.SaveChanges();

        var dto = new CompletePaymentDto
        {
            Amount = 600
        };

        Assert.ThrowsAsync<BadRequestException>(async () =>
        {
            await _service.PayForContractAsync(3, dto);
        });
    }

    [Test]
    public async Task PayForContractAsync_ThrowsForCancelledContract_AndCreatesNewOne()
    {
        var contract = new Contract
        {
            Id = 4,
            ClientId = 1,
            SoftwareSystemId = 1,
            SoftwareVersionId = 1,
            Price = 500,
            Status = ContractStatus.Cancelled,
            StartDate = DateTime.Today.AddDays(-10),
            EndDate = DateTime.Today.AddDays(-5)
        };
        _context.Contracts.Add(contract);
        await _context.SaveChangesAsync();

        var dto = new CompletePaymentDto
        {
            Amount = 100
        };

        var ex = Assert.ThrowsAsync<BadRequestException>(async () =>
        {
            await _service.PayForContractAsync(4, dto);
        });

        StringAssert.Contains("New Contract", ex.Message);

        var newContract = _context.Contracts.FirstOrDefault(c => c.Id != 4 && c.ClientId == 1);
        Assert.IsNotNull(newContract);
    }
}