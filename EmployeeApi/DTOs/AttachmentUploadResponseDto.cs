namespace EmployeeApi.DTOs;

public class AttachmentUploadResponseDto
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public string FileType { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public DateTime UploadedAt { get; set; }
    public string Message { get; set; } = string.Empty;
}