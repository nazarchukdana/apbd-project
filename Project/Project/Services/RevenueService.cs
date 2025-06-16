using Microsoft.EntityFrameworkCore;
using Project.Data;
using Project.Exceptions;
using Project.Models;

namespace Project.Services;

public class RevenueService : IRevenueService
{
    private readonly DatabaseContext _context;
    private readonly HttpClient _client;

    public RevenueService(DatabaseContext context, HttpClient client)
    {
        _context = context;
        _client = client;
    }

    public async Task<decimal> GetTotalRevenueAsync()
    {
        var totalRevenue = await _context.Contracts
            .Where(c => c.Status == ContractStatus.Signed)
            .SumAsync(c => (decimal?)c.Price) ?? 0m;
        return totalRevenue;
    }

    public async Task<decimal> GetRevenueForProductAsync(int id)
    {
        var productExists = await _context.SoftwareSystems.AnyAsync(p => p.Id == id);
        if (!productExists)
            throw new NotFoundException("Product not found");
        
        var revenue = await _context.Contracts
            .Where(c => c.Status == ContractStatus.Signed && c.SoftwareSystemId == id)
            .SumAsync(c => (decimal?)c.Price) ?? 0m;
        return revenue;
    }

    public async Task<decimal> GetTotalPredictedRevenueAsync()
    {
        var predictedRevenue = await _context.Contracts
            .Where(c => c.Status == ContractStatus.Signed || c.Status == ContractStatus.Active)
            .SumAsync(c => (decimal?)c.Price) ?? 0m;
        return predictedRevenue;
    }

    public async Task<decimal> GetPredictedRevenueForProductAsync(int id)
    {
        var productExists = await _context.SoftwareSystems.AnyAsync(p => p.Id == id);
        if (!productExists)
            throw new NotFoundException("Product not found");
        var predictedRevenue = await _context.Contracts
            .Where(c =>( c.Status == ContractStatus.Signed || c.Status == ContractStatus.Active)
                       && c.SoftwareSystemId == id)
            .SumAsync(c => (decimal?)c.Price) ?? 0m;
        return predictedRevenue;
    }

    public async Task<decimal> GetRevenueInCurrencyAsync(string currencyCode)
    {
        if (string.IsNullOrWhiteSpace(currencyCode))
            throw new BadRequestException("Currency code is required");
        
        var totalRevenuePLN = await GetTotalRevenueAsync();
        
        var url = $"https://api.nbp.pl/api/exchangerates/rates/A/{currencyCode}?format=json";
        var response = await _client.GetAsync(url);
        if (!response.IsSuccessStatusCode)
            throw new BadRequestException("Unable to get exchange rate");
        var result = await response.Content.ReadFromJsonAsync<NbpExchangeRateResponse>();
        if(result?.Rates == null || !result.Rates.Any())
            throw new BadRequestException("No exchange rate for currency");
        decimal rate = result.Rates.First().Mid;
        var converted = Math.Round(totalRevenuePLN / rate, 2);
        return converted;
    }

    private class NbpExchangeRateResponse
    {
        public string Table { get; set; } = null!;
        public string Currency { get; set; } = null!;
        public string Code { get; set; } = null!;
        public List<Rate> Rates { get; set; } = null!;

        public class Rate
        {
            public string No { get; set; } = null!;
            public string EffectiveDate { get; set; } = null!;
            public decimal Mid { get; set; }
        }
    }
}