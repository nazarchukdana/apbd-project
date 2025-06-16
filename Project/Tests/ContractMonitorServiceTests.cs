using Microsoft.EntityFrameworkCore;
using Project.Data;
using Project.Models;
using Project.Services;

namespace Tests;

public class ContractMonitorServiceTests
{
    private DatabaseContext _context;
    private ContractMonitorService _service;
    [SetUp]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<DatabaseContext>()
            .UseInMemoryDatabase("TestDb_" + Guid.NewGuid())
            .Options;
        _context = new DatabaseContext(options);
        _service = new ContractMonitorService(_context);
        _context.Contracts.AddRange(
            new Contract
            {
                Id = 1,
                Price = 100,
                Status = ContractStatus.Active,
                EndDate = DateTime.Now.AddDays(-1)
            },
            new Contract
            {
                Id = 2,
                Price = 200,
                Status = ContractStatus.Active,
                EndDate = DateTime.Now.AddDays(5)
            },
            new Contract
            {
                Id = 3,
                Price = 300,
                Status = ContractStatus.Cancelled,
                EndDate = DateTime.Now.AddDays(-10)
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
    public async Task MonitorAndCancelContractsAsync_ChangesExpiredContractsToCancelled()
    {
        await _service.MonitorAndCancelContractsAsync();
        var expired = await _context.Contracts.FindAsync(1);
        var valid = await _context.Contracts.FindAsync(2);
        var alreadyCancelled = await _context.Contracts.FindAsync(3);
        Assert.AreEqual(ContractStatus.Cancelled, expired.Status);
        Assert.AreEqual(ContractStatus.Active, valid.Status);
        Assert.AreEqual(ContractStatus.Cancelled, alreadyCancelled.Status);
    }
    [Test]
    public async Task MonitorAndCancelContractsAsync_DoesNothing_WhenNoExpiredContracts()
    {
        foreach (var c in _context.Contracts)
        {
            c.Status = ContractStatus.Active;
            c.EndDate = DateTime.Now.AddDays(10);
        }
        await _context.SaveChangesAsync();
        await _service.MonitorAndCancelContractsAsync();
        var contracts = _context.Contracts.ToList();
        Assert.IsTrue(contracts.All(c => c.Status == ContractStatus.Active));
    }
}