using System.ComponentModel.DataAnnotations;

namespace EmployeeApi.DTOs;

public class EmployeeUpdateDto
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Department { get; set; } = string.Empty;

    [Range(0, 1000000)]
    public decimal Salary { get; set; }
}