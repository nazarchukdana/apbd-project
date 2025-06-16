using Project.DTOs;

namespace Project.Services;

public interface IContractService
{
    Task<ContractResponseDto> AddContractAsync(CreateContractDto dto);
    Task<ContractResponseDto> GetContractAsync(int id);
    Task<List<ContractResponseDto>> GetContractsAsync();
    Task<ContractResponseDto> PayForContractAsync(int id, CompletePaymentDto dto);
}