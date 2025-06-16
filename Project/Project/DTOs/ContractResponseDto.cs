
namespace Project.DTOs;

public class ContractResponseDto
{
    public int Id { get; set; }
    public int ClientId { get; set; }
    
    public string SoftwareName{ get; set; } = null!;
    
    public string SoftwareVersion{ get; set; } = null!;

    public DateTime StartDate { get; set; }
    
    public DateTime EndDate { get; set; }

    public int SupportYears { get; set; }
    public string Status { get; set; } = null!;
    public decimal Price { get; set; }
    public decimal Paid { get; set; }
    public decimal LeftToPay { get; set; }
}