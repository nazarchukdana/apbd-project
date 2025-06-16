using Microsoft.EntityFrameworkCore;
using Project.Data;
using Project.DTOs;
using Project.Exceptions;
using Project.Models;

namespace Project.Services;

public class ContractService : IContractService
{
    private readonly DatabaseContext _context;
    private const int ReturningClientDiscount = 5;
    private const decimal SupportYearCost = 1000;

    public ContractService(DatabaseContext context)
    {
        _context = context;
    }

    public async Task<ContractResponseDto> AddContractAsync(CreateContractDto dto)
    {
        var days = (dto.EndDate - dto.StartDate).TotalDays;
        if (days < 3 || days > 30)
            throw new BadRequestException("Contract period must be between 3 and 30 days");
        if (dto.SupportYears < 0 || dto.SupportYears > 3)
            throw new BadRequestException("Support years must be between 0 and 3");
        var client = await _context.Clients.FindAsync(dto.ClientId);
        if (client == null)
            throw new NotFoundException("Client not found");
        
        var software = await _context.SoftwareSystems.FindAsync(dto.SoftwareSystemId);
        if (software == null)
            throw new NotFoundException("Software System does not exist");
        if(software.UpfrontCost == null)
            throw new BadRequestException("Software System is not available for upfront cost");
        var version = await _context.Versions.FirstOrDefaultAsync( v => v.Id == dto.SoftwareVersionId && v.SoftwareSystemId == dto.SoftwareSystemId);
        if (version == null)
            throw new NotFoundException("Software Version not found");
        
        bool hasActiveContract = await _context.Contracts
            .AnyAsync(c => c.ClientId == dto.ClientId 
                           && c.SoftwareSystemId == dto.SoftwareSystemId
                           &&(c.Status == ContractStatus.Active));
        
        if(hasActiveContract)
            throw new ConflictException("Client already has an active contract for this software.");
        
        var basePrice = software.UpfrontCost;
        var today = DateTime.Today;

        var maxDiscount = await _context.Discounts
            .Where(d => d.DiscountType == DiscountType.UpfrontCost &&
                        d.StartDate <= today &&
                        d.EndDate >= today)
            .Select(d => (int?)d.Percentage)
            .MaxAsync() ?? 0;
        
        bool isReturningClient =
            await _context.Contracts.AnyAsync(c => c.ClientId == dto.ClientId
                                                   && c.Status == ContractStatus.Signed);

        if (isReturningClient)
            maxDiscount += ReturningClientDiscount;
        
        if(maxDiscount > 100)
            maxDiscount = 100;
        var discountedPrice = basePrice.Value * (1 - maxDiscount / 100m);
        discountedPrice += dto.SupportYears * SupportYearCost;
        discountedPrice = Math.Round(discountedPrice, 2);
        var contract = new Contract
        {
            ClientId = dto.ClientId,
            SoftwareSystemId = dto.SoftwareSystemId,
            SoftwareVersionId = dto.SoftwareVersionId,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            SupportYears = dto.SupportYears,
            Price = discountedPrice,

        };
        await _context.Contracts.AddAsync(contract);
        await _context.SaveChangesAsync();
        return MapToContractResponseDto(contract);
    }

    public async Task<ContractResponseDto> GetContractAsync(int id)
    {
        var contract = await _context.Contracts.Include(c => c.SoftwareSystem)
            .Include(c => c.Version).FirstOrDefaultAsync(c => c.Id == id);
        if(contract == null)
            throw new NotFoundException("Contract not found");
        return MapToContractResponseDto(contract);
    }

    public async Task<List<ContractResponseDto>> GetContractsAsync()
    {
        var contracts = await _context.Contracts
            .Include(c => c.SoftwareSystem)
            .Include(c => c.Version).ToListAsync();
        return contracts.Select(MapToContractResponseDto).ToList();
    }

    public async Task<ContractResponseDto> PayForContractAsync(int id, CompletePaymentDto dto)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var contract = await _context.Contracts
                .Include(c => c.Payments)
                .Include(c => c.SoftwareSystem)
                .Include(c => c.Version)
                .FirstOrDefaultAsync(c => c.Id == id);
            if (contract == null)
                throw new NotFoundException("Contract not found");
            if (contract.Status == ContractStatus.Cancelled)
            {
                var activeContract = await _context.Contracts.FirstOrDefaultAsync(c =>
                    c.ClientId == contract.ClientId &&
                    c.SoftwareSystemId == contract.SoftwareSystemId &&
                    c.Status == ContractStatus.Active);
                if (activeContract != null)
                    throw new BadRequestException(
                        $"This contract has been cancelled, you already have created active contract for this software. Pay for it by id {activeContract.Id}");
                var newContract = new Contract
                {
                    ClientId = contract.ClientId,
                    SoftwareSystemId = contract.SoftwareSystemId,
                    SoftwareVersionId = contract.SoftwareVersionId,
                    StartDate = DateTime.Now,
                    EndDate = DateTime.Now.AddDays(15),
                    SupportYears = contract.SupportYears,
                    Price = contract.Price,
                };
                await _context.Contracts.AddAsync(newContract);
                foreach (var pay in contract.Payments)
                {
                    pay.ContractId = newContract.Id;
                }

                await _context.SaveChangesAsync();
                throw new BadRequestException(
                    $"Contract has been cancelled. New Contract with id {newContract.Id} has been created. Please pay for the new contract.");
            }

            if (contract.Status == ContractStatus.Signed)
            {
                throw new BadRequestException("Contract is already signed");
            }

            if (dto.Amount <= 0)
                throw new BadRequestException("Payment amount must be positive.");
            decimal totalPaid = contract.TotalPaid;
            decimal remaining = contract.RemainingAmount;

            if (dto.Amount > remaining)
                throw new BadRequestException("Payment amount cannot be greater than the remaining amount");
            var payment = new Payment()
            {
                ContractId = contract.Id,
                Amount = dto.Amount,
                PaymentDate = DateTime.Now
            };
            _context.Payments.Add(payment);
            totalPaid += payment.Amount;
            if (totalPaid == contract.Price)
                contract.Status = ContractStatus.Signed;
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            return MapToContractResponseDto(contract);
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    private static ContractResponseDto MapToContractResponseDto(Contract contract)
    {
        return new ContractResponseDto
        {
            Id = contract.Id,
            ClientId = contract.ClientId,
            EndDate = contract.EndDate,
            SoftwareName = contract.SoftwareSystem.Name,
            SupportYears = contract.SupportYears,
            SoftwareVersion = contract.Version.Name,
            StartDate = contract.StartDate,
            Status = contract.Status.ToString(),
            Price = contract.Price,
            Paid = contract.TotalPaid,
            LeftToPay = contract.RemainingAmount
        };
    }
    
    
}