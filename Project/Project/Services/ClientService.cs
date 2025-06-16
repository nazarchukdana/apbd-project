using Microsoft.EntityFrameworkCore;
using Project.Data;
using Project.DTOs;
using Project.Exceptions;
using Project.Models;
using Project.Models.Clients;

namespace Project.Services;

public class ClientService : IClientService
{
    private readonly DatabaseContext _context;

    public ClientService(DatabaseContext context)
    {
        _context = context;
    }

    public async Task<IndividualResponseDto> GetIndividualByIdAsync(int id)
    {
        var client = await _context.IndividualClients.FindAsync(id);
        if (client == null || client.IsDeleted)
            throw new NotFoundException("Client not found");
        return MapToIndividualResponseDto(client);
        
    }

    public async Task<CompanyResponseDto> GetCompanyByIdAsync(int id)
    {
        var client = await _context.CompanyClients.FindAsync(id);
        if(client == null)
            throw new NotFoundException("Client not found");
        return MapToCompanyResponseDto(client);
        
    }

    public async Task<List<ClientListDto>> GetAllClientsAsync()
    {
        var clients = await _context.Clients.ToListAsync();
        return clients.Select(c => new ClientListDto
        {
            Id = c.Id,
            ClientType = c.ClientType,
            DisplayName = c switch
            {
                IndividualClient ind => ind.FirstName + " " + ind.LastName,
                CompanyClient com => com.CompanyName,
                _ => "Unknown"
            },
            Email = c.Email,
            PhoneNumber = c.PhoneNumber,
            IsDeleted = c is IndividualClient individual && individual.IsDeleted
        }).ToList();
    }

    public async Task<IndividualResponseDto> AddIndividualClientAsync(CreateIndividualDto dto)
    {
        var existingPesel = await _context.IndividualClients
            .AnyAsync(i => i.Pesel == dto.Pesel);
        if (existingPesel)
            throw new ConflictException("Individual client already exists with this Pesel");
        var existingEmail = await _context.Clients
            .AnyAsync(i => i.Email == dto.Email);
        if (existingEmail)
            throw new ConflictException("Client already exists with this email");
        
        var client = new IndividualClient
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            PhoneNumber = dto.PhoneNumber,
            Email = dto.Email,
            Address = dto.Address,
            Pesel = dto.Pesel,
        };
        
        await _context.IndividualClients.AddAsync(client);
        await _context.SaveChangesAsync();
        return MapToIndividualResponseDto(client);
    }

    public async Task<CompanyResponseDto> AddCompanyClientAsync(CreateCompanyDto dto)
    {
        var existingKrs = await _context.CompanyClients
            .AnyAsync(i => i.KrsNumber == dto.KrsNumber);
        if (existingKrs)
            throw new ConflictException("Company client already exists with this KRS number");

        var existingEmail = await _context.Clients
            .AnyAsync(i => i.Email == dto.Email);
        if (existingEmail)
            throw new ConflictException("Client already exists with this email");

        var client = new CompanyClient
        {
            CompanyName = dto.CompanyName,
            Address = dto.Address,
            Email = dto.Email,
            PhoneNumber = dto.PhoneNumber,
            KrsNumber = dto.KrsNumber,
        };
        await _context.CompanyClients.AddAsync(client);
        await _context.SaveChangesAsync();
        return MapToCompanyResponseDto(client);
    }

    public async Task<IndividualResponseDto> UpdateIndividualClientAsync(int id, UpdateIndividualDto dto)
    {
        var client = await _context.IndividualClients.FindAsync(id);
        if (client == null)
            throw new NotFoundException("Client not found");
        var existingEmail = await _context.Clients
            .AnyAsync(c => c.Email == dto.Email && c.Id != id);
        if (existingEmail)
            throw new ConflictException("Client already exists with this email");
        
        client.FirstName = dto.FirstName;
        client.LastName = dto.LastName;
        client.PhoneNumber = dto.PhoneNumber;
        client.Email = dto.Email;
        client.Address = dto.Address;

        await _context.SaveChangesAsync();
        
        return MapToIndividualResponseDto(client);
    }

    public async Task<CompanyResponseDto> UpdateCompanyClientAsync(int id, UpdateCompanyDto dto)
    {
        var client = await _context.CompanyClients.FindAsync(id);
        if(client == null)
            throw new NotFoundException("Client not found");
        
        var existingEmail = await _context.Clients
            .AnyAsync(c => c.Email == dto.Email && c.Id != id);
        if (existingEmail)
            throw new ConflictException("Client already exists with this email");
        
        client.CompanyName = dto.CompanyName;
        client.Address = dto.Address;
        client.PhoneNumber = dto.PhoneNumber;
        client.Email = dto.Email;
        
        await _context.SaveChangesAsync();
        return MapToCompanyResponseDto(client);
    }

    public async Task RemoveClientAsync(int id)
    {
        var client = await _context.Clients
            .Include(c => c.Contracts)
            .FirstOrDefaultAsync(c => c.Id == id);
        if (client == null)
            return;
        if (client is CompanyClient)
            throw new ConflictException("Company cannot be deleted");
        if (client is IndividualClient ind)
        {
            bool hasActiveContracts = ind.Contracts.Any(c => c.Status == ContractStatus.Active);
            if (hasActiveContracts)
                throw new ConflictException("Client cannot be deleted because they have active contracts.");
            ind.SoftDelete();
            await _context.SaveChangesAsync();
        }
    }

    private static IndividualResponseDto MapToIndividualResponseDto(IndividualClient client)
    {
        return new IndividualResponseDto
        {
            Id = client.Id,
            Address = client.Address,
            Email = client.Email,
            FirstName = client.FirstName,
            LastName = client.LastName,
            PhoneNumber = client.PhoneNumber,
            Pesel = client.Pesel,
            IsDeleted = client.IsDeleted,
        };
    }

    private static CompanyResponseDto MapToCompanyResponseDto(CompanyClient client)
    {
        return new CompanyResponseDto
        {
            Id = client.Id,
            Address = client.Address,
            Email = client.Email,
            PhoneNumber = client.PhoneNumber,
            CompanyName = client.CompanyName,
            KrsNumber = client.KrsNumber
        };
    }
}