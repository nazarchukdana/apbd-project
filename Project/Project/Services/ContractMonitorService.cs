using Microsoft.EntityFrameworkCore;
using Project.Data;
using Project.Models;

namespace Project.Services;

public class ContractMonitorService : IContractMonitorService
{
    private readonly DatabaseContext _context;

    public ContractMonitorService(DatabaseContext context)
    {
        _context = context;
    }

    public async Task MonitorAndCancelContractsAsync()
    {
        var now = DateTime.Now;
        var expiredContracts = await _context.Contracts
            .Where(c => c.Status == ContractStatus.Active
                        && c.EndDate < now)
            .ToListAsync();
        foreach (var contract in expiredContracts)
        {
            contract.Status = ContractStatus.Cancelled;
            
        }
        if(expiredContracts.Any())
            await _context.SaveChangesAsync();
    }
}