namespace Project.Services;

public class ContractMonitoringBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;

    public ContractMonitoringBackgroundService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = _serviceProvider.CreateScope();
            var monitor = scope.ServiceProvider.GetRequiredService<IContractMonitorService>();

            await monitor.MonitorAndCancelContractsAsync();

            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
        }
    }
}