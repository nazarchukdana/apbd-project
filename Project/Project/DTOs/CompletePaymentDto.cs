using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Project.DTOs;

public class CompletePaymentDto
{
    [Required]
    [Precision(10, 2)]
    [Range(0, double.MaxValue, ErrorMessage = "Amount must be non-negative.")]
    public decimal Amount { get; set; }
}