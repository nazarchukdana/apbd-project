using System.ComponentModel.DataAnnotations;

namespace Project.DTOs;

public class CreateContractDto
{
    [Required]
    public int ClientId { get; set; }
    
    [Required]
    public int SoftwareSystemId { get; set; }
    
    [Required]
    public int SoftwareVersionId { get; set; }

    [Required]
    public DateTime StartDate { get; set; }
    
    [Required]
    public DateTime EndDate { get; set; }

    [Required]
    [Range(0, 3, ErrorMessage = "Support years must be between 0 and 3")]
    public int SupportYears { get; set; }
}