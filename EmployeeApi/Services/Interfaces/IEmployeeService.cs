// Services/Interfaces/IEmployeeService.cs
using EmployeeApi.DTOs;

namespace EmployeeApi.Services.Interfaces;

public interface IEmployeeService
{
    Task<IEnumerable<EmployeeResponseDto>> GetAllAsync();
    Task<EmployeeResponseDto?> GetByIdAsync(int id);
    Task<EmployeeResponseDto> CreateAsync(EmployeeCreateDto employeeDto);
    Task<EmployeeResponseDto?> UpdateAsync(int id, EmployeeUpdateDto employeeDto);
    Task<bool> DeleteAsync(int id);

    Task<EmployeeProfilePictureDto?> UploadProfilePictureAsync(int employeeId, Stream fileStream, string fileName, string contentType);
    Task<EmployeeAttachmentResponseDto?> UploadAttachmentAsync(int employeeId, Stream fileStream, string fileName, string contentType);
    Task<bool> DeleteAttachmentAsync(int attachmentId);
    Task<DownloadAttachmentResult?> DownloadAttachmentAsync(int attachmentId);
    Task<bool> DeleteProfilePictureAsync(int profilePictureId);
}

public class DownloadAttachmentResult
{
    public byte[] FileBytes { get; set; } = Array.Empty<byte>();
    public string FileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
}