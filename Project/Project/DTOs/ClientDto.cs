using System.ComponentModel.DataAnnotations;

namespace Project.DTOs;

public class ClientDto
{
    [Required]
    [MaxLength(200)]
    public string Address { get; set; } = null!;
    [Required]
    [EmailAddress]
    [MaxLength(100)]
    public string Email { get; set; } = null!;
    [Required]
    [Phone]
    [MaxLength(50)]
    public string PhoneNumber { get; set; } = null!;
}