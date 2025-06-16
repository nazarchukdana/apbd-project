namespace Project.Services;

public interface IRevenueService
{
    Task<decimal> GetTotalRevenueAsync();
    Task<decimal> GetRevenueForProductAsync(int id);
    
    Task<decimal> GetTotalPredictedRevenueAsync();
    Task<decimal> GetPredictedRevenueForProductAsync(int id);

    Task<decimal> GetRevenueInCurrencyAsync(string currencyCode);
}