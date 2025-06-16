
namespace Project.DTOs;

public class CompanyResponseDto
{
    public int Id { get; set; }
    public string CompanyName { get; set; } = null!;
    public string Address { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
    public string KrsNumber { get; set; } = null!;
}