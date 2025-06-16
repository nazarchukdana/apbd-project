using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Project.Models;

public class SoftwareSystem
{
    [Key]
    public int Id { get; set; }

    [Required, MaxLength(100)]
    public string Name { get; set; } = null!;

    [MaxLength(500)]
    public string Description { get; set; } = null!;


    [Required, MaxLength(50)]
    public string Category { get; set; } = null!;
    
    [Column(TypeName = "decimal")]
    [Precision(10, 2)]
    [Range(0, double.MaxValue, ErrorMessage = "Price should be non-negative.")]
    public decimal? UpfrontCost { get; set; }
    
    [Column(TypeName = "decimal")]
    [Precision(10, 2)]
    [Range(0, double.MaxValue, ErrorMessage = "Price should be non-negative.")]
    public decimal? SubscriptionCost { get; set; }
    
    public ICollection<SoftwareVersion> Versions { get; set; } = new List<SoftwareVersion>();
    
    [NotMapped]
    public SoftwareVersion? CurrentVersion => Versions?.FirstOrDefault(v => v.IsCurrent);
    
    public ICollection<Contract> Contracts { get; set; } = new List<Contract>();

}