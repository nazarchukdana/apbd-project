using System.Net;
using Microsoft.EntityFrameworkCore;
using Project.Data;
using Project.Exceptions;
using Project.Models;
using Project.Services;

namespace Tests;

public class RevenueServiceTests
{
    private DatabaseContext _context;
    private RevenueService _service;

    [SetUp]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<DatabaseContext>()
            .UseInMemoryDatabase(databaseName: "TestDb_" + Guid.NewGuid())
            .Options;

        _context = new DatabaseContext(options);
        
        var fakeHandler = new FakeHttpMessageHandler();
        var httpClient = new HttpClient(fakeHandler)
        {
            BaseAddress = new Uri("https://api.nbp.pl")
        };
        _service = new RevenueService(_context, httpClient);
        
        var system = new SoftwareSystem
        {
            Id = 1,
            Name = "TestSystem",
            Description = "Sample description",
            Category = "Finance",
            SubscriptionCost = 10,
            UpfrontCost = 100
        };

        _context.SoftwareSystems.Add(system);
        
        var version = new SoftwareVersion
        {
            Id = 1,
            Name = "1.0.0",
            SoftwareSystemId = system.Id,
            IsCurrent = true
        };
        _context.Versions.Add(version);
        
        _context.Contracts.AddRange(
            new Contract
            {
                Id = 1,
                SoftwareSystemId = system.Id,
                SoftwareVersionId = version.Id,
                Price = 100,
                Status = ContractStatus.Signed
            },
            new Contract
            {
                Id = 2,
                SoftwareSystemId = system.Id,
                SoftwareVersionId = version.Id,
                Price = 200,
                Status = ContractStatus.Active
            },
            new Contract
            {
                Id = 3,
                SoftwareSystemId = system.Id,
                SoftwareVersionId = version.Id,
                Price = 300,
                Status = ContractStatus.Cancelled
            }
        );

        _context.SaveChanges();
    }
    [TearDown]
    public void TearDown()
    {
        _context?.Dispose();
    }
    [Test]
    public async Task GetTotalRevenueAsync_ReturnsOnlySigned()
    {
        var result = await _service.GetTotalRevenueAsync();
        Assert.AreEqual(100, result);
    }
    [Test]
    public async Task GetRevenueForProductAsync_ReturnsRevenueForValidProduct()
    {
        var result = await _service.GetRevenueForProductAsync(1);
        Assert.AreEqual(100, result);
    }
    [Test]
    public void GetRevenueForProductAsync_ThrowsNotFound_ForInvalidProduct()
    {
        Assert.ThrowsAsync<NotFoundException>(async () =>
        {
            await _service.GetRevenueForProductAsync(99);
        });
    }
    [Test]
    public async Task GetTotalPredictedRevenueAsync_ReturnsSignedAndActive()
    {
        var result = await _service.GetTotalPredictedRevenueAsync();
        Assert.AreEqual(100 + 200, result);
    }
    
    [Test]
    public async Task GetPredictedRevenueForProductAsync_ReturnsCorrectSum()
    {
        var result = await _service.GetPredictedRevenueForProductAsync(1);
        Assert.AreEqual(100 + 200, result);
    }
    [Test]
    public void GetPredictedRevenueForProductAsync_ThrowsNotFound_ForInvalidProduct()
    {
        Assert.ThrowsAsync<NotFoundException>(async () =>
        {
            await _service.GetPredictedRevenueForProductAsync(999);
        });
    }
    [Test]
    public async Task GetRevenueInCurrencyAsync_ReturnsConvertedAmount()
    {
        var result = await _service.GetRevenueInCurrencyAsync("USD");
        Assert.AreEqual(20.00m, result);
    }
    private class FakeHttpMessageHandler : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var fakeJson = @"{
                    ""table"": ""A"",
                    ""currency"": ""dolar amerykański"",
                    ""code"": ""USD"",
                    ""rates"": [
                        {
                            ""no"": ""106/A/NBP/2024"",
                            ""effectiveDate"": ""2024-06-06"",
                            ""mid"": 5.0
                        }
                    ]
                }";

            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(fakeJson)
            };
            return Task.FromResult(response);
        }
    }
}