using System.ComponentModel.DataAnnotations;

namespace Project.DTOs;

public class RegisterRequest
{
    [Required] 
    [MaxLength(50)]
    public string Login { get; set; } = null!;
    [Required]
    public string Password { get; set; } = null!;
    [Required]
    public string Role { get; set; } = null!;
}
