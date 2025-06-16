using System.ComponentModel.DataAnnotations;

namespace Project.Models.Clients;

public class IndividualClient : Client
{
    [Required]
    [MaxLength(50)]
    public string FirstName { get; set; } = null!;
    
    [Required]
    [MaxLength(50)]
    public string LastName { get; set; } = null!;
    
    [Required]
    [MaxLength(11)]
    [RegularExpression(@"^\d{11}$", ErrorMessage = "PESEL must be exactly 11 digits.")]
    public string Pesel { get; set; } = null!;

    public IndividualClient()
    {
        ClientType = "Individual";
    }
    public bool IsDeleted { get; set; } = false;

    public void SoftDelete()
    {
        IsDeleted = true;
        FirstName = "[DELETED]";
        LastName = "[DELETED]";
        Address = "[DELETED]";
        Email = $"deleted_{Id}@deleted.com";
        PhoneNumber = "[DELETED]";
    }
}