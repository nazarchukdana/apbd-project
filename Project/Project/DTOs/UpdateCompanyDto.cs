using System.ComponentModel.DataAnnotations;

namespace Project.DTOs;

public class UpdateCompanyDto : ClientDto
{
    [Required]
    [MaxLength(100)]
    public string CompanyName { get; set; } = null!;
}