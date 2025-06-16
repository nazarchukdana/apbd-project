using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project.Models;

public class SoftwareVersion
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = null!;

    public bool IsCurrent { get; set; } = false;
    
    [ForeignKey(nameof(SoftwareSystem))]
    public int SoftwareSystemId { get; set; }
    public SoftwareSystem SoftwareSystem { get; set; } = null!;
    public ICollection<Contract> Contracts { get; set; } = new List<Contract>();
}