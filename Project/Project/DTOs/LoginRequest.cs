using System.ComponentModel.DataAnnotations;

namespace Project.DTOs;
public class LoginRequest
{
    [Required] [MaxLength(50)] 
    public string Login { get; set; } = null!;
    [Required]
    public string Password { get; set; }
}
