using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Project.Models;

public class Payment
{
    [Key]
    public int Id { get; set; }
    
    [ForeignKey(nameof(Contract))]
    public int ContractId { get; set; }
    public Contract Contract { get; set; } = null!;

    [Required] public DateTime PaymentDate { get; set; } = DateTime.Now;
    
    [Required]
    [Column(TypeName = "decimal")]
    [Precision(10, 2)]
    [Range(0, double.MaxValue, ErrorMessage = "Amount must be non-negative.")]
    public decimal Amount { get; set; }
}