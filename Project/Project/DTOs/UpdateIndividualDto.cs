using System.ComponentModel.DataAnnotations;

namespace Project.DTOs;

public class UpdateIndividualDto : ClientDto
{
    [Required]
    [MaxLength(50)]
    public string FirstName { get; set; } = null!;
    
    [Required]
    [MaxLength(50)]
    public string LastName { get; set; } = null!;
}