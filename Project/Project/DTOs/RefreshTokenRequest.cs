using System.ComponentModel.DataAnnotations;

namespace Project.DTOs;
public class RefreshTokenRequest
{
    [Required] public string RefreshToken { get; set; } = null!;
}
