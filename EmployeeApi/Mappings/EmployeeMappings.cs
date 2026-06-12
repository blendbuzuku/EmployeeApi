using EmployeeApi.DTOs;
using static EmployeeApi.Models.EmployeeModel;

namespace EmployeeApi.Mappings;

public static class EmployeeMappings
{
    public static EmployeeResponseDto ToResponse(this Employee employee)
    {
        return new EmployeeResponseDto
        {
            Id = employee.Id,
            Name = employee.Name,
            Email = employee.Email,
            Department = employee.Department,
            Salary = employee.Salary,
            CreatedAt = employee.CreatedAt,
            UpdatedAt = employee.UpdatedAt,
            ProfilePicture = employee.ProfilePicture != null ? new EmployeeProfilePictureDto
            {
                Id = employee.ProfilePicture.Id,
                EmployeeId = employee.ProfilePicture.EmployeeId,
                FilePath = employee.ProfilePicture.FilePath,
                FileName = employee.ProfilePicture.FileName,
                FileType = employee.ProfilePicture.FileType,
                FileSize = employee.ProfilePicture.FileSize,
                UploadedAt = employee.ProfilePicture.UploadedAt
            } : null,
            Attachments = employee.Attachments?.Select(a => a.ToResponse()).ToList()
        };
    }

    public static Employee ToEntity(this EmployeeCreateDto dto)
    {
        return new Employee
        {
            Name = dto.Name,
            Email = dto.Email,
            Department = dto.Department,
            Salary = dto.Salary,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public static Employee ToEntity(this EmployeeUpdateDto dto, Employee existingEmployee)
    {
        existingEmployee.Name = dto.Name;
        existingEmployee.Email = dto.Email;
        existingEmployee.Department = dto.Department;
        existingEmployee.Salary = dto.Salary;
        existingEmployee.UpdatedAt = DateTime.UtcNow;
        return existingEmployee;
    }

    // Attachment mappings
    public static EmployeeAttachmentResponseDto ToResponse(this EmployeeAttachment attachment)
    {
        return new EmployeeAttachmentResponseDto
        {
            Id = attachment.Id,
            EmployeeId = attachment.EmployeeId,
            FileName = attachment.FileName,
            FilePath = attachment.FilePath,
            FileType = attachment.FileType,
            FileSize = attachment.FileSize,
            UploadedAt = attachment.UploadedAt
        };
    }

    public static EmployeeAttachment ToEntity(this EmployeeAttachmentCreateDto dto, int employeeId)
    {
        return new EmployeeAttachment
        {
            EmployeeId = employeeId,
            FileName = dto.FileName,
            FilePath = dto.FilePath,
            FileType = dto.FileType,
            FileSize = dto.FileSize,
            UploadedAt = DateTime.UtcNow
        };
    }
}