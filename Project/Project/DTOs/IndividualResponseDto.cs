namespace Project.DTOs;

public class IndividualResponseDto
{
    public int Id { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Address { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
    public string Pesel { get; set; } = null!;
    public bool IsDeleted { get; set; }
}