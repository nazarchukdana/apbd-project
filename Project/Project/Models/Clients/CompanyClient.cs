using System.ComponentModel.DataAnnotations;

namespace Project.Models.Clients;

public class CompanyClient : Client
{
    [Required]
    [MaxLength(100)]
    public string CompanyName { get; set; } = null!;
    
    [Required]
    [StringLength(10)]
    [RegularExpression(@"^\d{10}$", ErrorMessage = "KRS must be exactly 10 digits.")]
    public string KrsNumber { get; set; } = null!;

    public CompanyClient()
    {
        ClientType = "Company";
    }
    
}