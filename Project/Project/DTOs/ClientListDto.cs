namespace Project.DTOs;

public class ClientListDto
{
    public int Id { get; set; }
    public string ClientType { get; set; } = null!;
    public string DisplayName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
    public bool IsDeleted { get; set; }
}