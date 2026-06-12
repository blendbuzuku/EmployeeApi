using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EmployeeApi.Models;

public class EmployeeModel
{
    public class Employee
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [MaxLength(150)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Department { get; set; } = string.Empty;

        [Range(0, 1000000)]
        public decimal Salary { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public virtual ICollection<EmployeeAttachment>? Attachments { get; set; }
        public virtual EmployeeProfilePicture? ProfilePicture { get; set; }
    }

    public class EmployeeAttachment
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public string FileType { get; set; } = string.Empty;
        public long FileSize { get; set; }
        public DateTime UploadedAt { get; set; }
        public virtual Employee? Employee { get; set; }
    }

    public class EmployeeProfilePicture
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int EmployeeId { get; set; }

        [Required]
        [MaxLength(500)]
        public string FilePath { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string FileName { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string FileType { get; set; } = string.Empty;

        public long FileSize { get; set; }

        public DateTime UploadedAt { get; set; }

        [ForeignKey("EmployeeId")]
        public virtual Employee? Employee { get; set; }
    }
}