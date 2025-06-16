using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Project.Models.Clients;

namespace Project.Models;

public enum ContractStatus
{
    Active,
    Signed,
    Cancelled
}

public class Contract
{
    [Key]
    public int Id { get; set; }
    
    [ForeignKey(nameof(Client))]
    public int ClientId { get; set; }
    public Client Client { get; set; } = null!;
    
    [ForeignKey(nameof(SoftwareSystem))]
    public int SoftwareSystemId { get; set; }
    public SoftwareSystem SoftwareSystem { get; set; } = null!;

    [ForeignKey(nameof(Version))]
    public int SoftwareVersionId { get; set; }
    public SoftwareVersion Version { get; set; } = null!;
    
    [Required]
    public DateTime StartDate { get; set; }
    [Required]
    public DateTime EndDate { get; set; }
    
    [Column(TypeName = "decimal")]
    [Precision(10, 2)]
    [Range(0, double.MaxValue, ErrorMessage = "Price must be non-negative.")]
    public decimal Price { get; set; }

    [Range(0, 3, ErrorMessage = "Support years must be between 0 and 3")]
    public int SupportYears { get; set; } = 0;

    public ContractStatus Status { get; set; } = ContractStatus.Active;

    public ICollection<Payment> Payments { get; set; } = new List<Payment>();

    [NotMapped] 
    public decimal TotalPaid => Payments?.Sum(p => p.Amount) ?? 0;
    [NotMapped]
    public decimal RemainingAmount => Price - TotalPaid;
}