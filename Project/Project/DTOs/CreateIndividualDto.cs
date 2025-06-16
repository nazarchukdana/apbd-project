using System.ComponentModel.DataAnnotations;

namespace Project.DTOs;

public class CreateIndividualDto : ClientDto
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
}