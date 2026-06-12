namespace EmployeeApi.DTOs
{
    public class EmployeeProfilePictureDto
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public string FilePath { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public string FileType { get; set; } = string.Empty;
        public long FileSize { get; set; }
        public DateTime UploadedAt { get; set; }
    }
}
