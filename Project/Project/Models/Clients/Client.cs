using System.ComponentModel.DataAnnotations;

namespace Project.Models.Clients;

public abstract class Client
{
    [Key]
    public int Id { get; set; }

    [Required] [MaxLength(200)] 
    public string Address { get; set; } = null!;
    
    [Required]
    [EmailAddress] [MaxLength(100)]
    public string Email { get; set; } = null!;
    
    [Required]
    [Phone] [MaxLength(50)]
    public string PhoneNumber { get; set; } = null!;
    
    public string ClientType { get; set; } = null!;
    
    public ICollection<Contract> Contracts { get; set; } = new List<Contract>();
}