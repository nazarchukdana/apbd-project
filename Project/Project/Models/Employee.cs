using System.ComponentModel.DataAnnotations;

namespace Project.Models;
public enum Role{
    User,
    Admin
}

public class Employee
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    [MaxLength(50)]
    public string Login { get; set; } = null!;
    
    [Required]
    public string PasswordHash { get; set; } = null!;
    
    [Required]
    public Role Role { get; set; }
    
    public string RefreshToken { get; set; }
    public DateTime RefreshTokenExpiry { get; set; }
}