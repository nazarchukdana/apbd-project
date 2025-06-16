namespace Project.Services;

public interface IContractMonitorService
{
    Task MonitorAndCancelContractsAsync();
}