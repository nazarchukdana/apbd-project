using System.ComponentModel.DataAnnotations;

namespace Project.Models;

public enum DiscountType
{
    UpfrontCost,
    SubscriptionCost
}
public class Discount
{
    [Key]
    public int Id { get; set; }

    [Required, MaxLength(100)]
    public string Name { get; set; } = null!;
    
    [Required]
    public DiscountType DiscountType { get; set; }
    
    [Required, Range(0, 100)]
    public int Percentage { get; set; }

    [Required]
    public DateTime StartDate { get; set; }

    [Required]
    public DateTime EndDate { get; set; }
}