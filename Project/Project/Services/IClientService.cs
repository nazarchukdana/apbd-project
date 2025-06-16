using Project.DTOs;

namespace Project.Services;

public interface IClientService
{
    Task<IndividualResponseDto> GetIndividualByIdAsync(int id);
    Task<CompanyResponseDto> GetCompanyByIdAsync(int id);
    Task<IndividualResponseDto> AddIndividualClientAsync(CreateIndividualDto dto);
    Task<CompanyResponseDto> AddCompanyClientAsync(CreateCompanyDto dto);
    Task<IndividualResponseDto> UpdateIndividualClientAsync(int id, UpdateIndividualDto dto);
    Task<CompanyResponseDto> UpdateCompanyClientAsync(int id, UpdateCompanyDto dto);
    Task<List<ClientListDto>> GetAllClientsAsync();
    Task RemoveClientAsync(int id);
}