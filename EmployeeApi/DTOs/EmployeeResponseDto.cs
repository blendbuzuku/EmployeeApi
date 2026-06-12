namespace EmployeeApi.DTOs;

public class EmployeeResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public decimal Salary { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public EmployeeProfilePictureDto? ProfilePicture { get; set; }  
    public List<EmployeeAttachmentResponseDto>? Attachments { get; set; }
}