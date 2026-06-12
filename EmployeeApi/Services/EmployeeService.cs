using EmployeeApi.DTOs;
using EmployeeApi.Repositories.Interfaces;
using EmployeeApi.Services.Interfaces;
using static EmployeeApi.Models.EmployeeModel;

namespace EmployeeApi.Services;

public class EmployeeService : IEmployeeService
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IEmployeeAttachmentRepository _attachmentRepository;
    private readonly IEmployeeProfilePictureRepository _profilePictureRepository;
    private readonly IWebHostEnvironment _environment;

    public EmployeeService(
        IEmployeeRepository employeeRepository,
        IEmployeeAttachmentRepository attachmentRepository,
        IEmployeeProfilePictureRepository profilePictureRepository,
        IWebHostEnvironment environment)
    {
        _employeeRepository = employeeRepository;
        _attachmentRepository = attachmentRepository;
        _profilePictureRepository = profilePictureRepository;
        _environment = environment;
    }

    public async Task<IEnumerable<EmployeeResponseDto>> GetAllAsync()
    {
        var employees = await _employeeRepository.GetAllAsync();
        var result = new List<EmployeeResponseDto>();

        foreach (var employee in employees)
        {
            var profilePicture = await _profilePictureRepository.GetByEmployeeIdAsync(employee.Id);
            result.Add(MapToResponseDto(employee, profilePicture));
        }

        return result;
    }

    public async Task<EmployeeResponseDto?> GetByIdAsync(int id)
    {
        var employee = await _employeeRepository.GetByIdWithAttachmentsAsync(id);
        if (employee == null)
            return null;

        var profilePicture = await _profilePictureRepository.GetByEmployeeIdAsync(id);
        return MapToResponseDto(employee, profilePicture);
    }

    public async Task<EmployeeResponseDto> CreateAsync(EmployeeCreateDto employeeDto)
    {
        var employee = new Employee
        {
            Name = employeeDto.Name,
            Email = employeeDto.Email,
            Department = employeeDto.Department,
            Salary = employeeDto.Salary,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var created = await _employeeRepository.CreateAsync(employee);
        return MapToResponseDto(created, null);
    }

    public async Task<EmployeeResponseDto?> UpdateAsync(int id, EmployeeUpdateDto employeeDto)
    {
        var existingEmployee = await _employeeRepository.GetByIdAsync(id);
        if (existingEmployee == null)
            return null;

        existingEmployee.Name = employeeDto.Name;
        existingEmployee.Email = employeeDto.Email;
        existingEmployee.Department = employeeDto.Department;
        existingEmployee.Salary = employeeDto.Salary;
        existingEmployee.UpdatedAt = DateTime.UtcNow;

        var updated = await _employeeRepository.UpdateAsync(existingEmployee);
        if (updated == null)
            return null;

        var profilePicture = await _profilePictureRepository.GetByEmployeeIdAsync(id);
        return MapToResponseDto(updated, profilePicture);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var employee = await _employeeRepository.GetByIdWithAttachmentsAsync(id);
        if (employee == null)
            return false;

        // Delete profile picture from DB and storage
        var profilePicture = await _profilePictureRepository.GetByEmployeeIdAsync(id);
        if (profilePicture != null)
        {
            var profilePath = Path.Combine(_environment.WebRootPath ?? "wwwroot", profilePicture.FilePath.TrimStart('/'));
            if (System.IO.File.Exists(profilePath))
                System.IO.File.Delete(profilePath);
            await _profilePictureRepository.DeleteByEmployeeIdAsync(id);
        }

        // Delete employee folder and all files
        var employeeProfileDir = Path.Combine(_environment.WebRootPath ?? "wwwroot", "uploads", "profiles", id.ToString());
        if (Directory.Exists(employeeProfileDir))
            Directory.Delete(employeeProfileDir, true);

        var employeeAttachmentDir = Path.Combine(_environment.WebRootPath ?? "wwwroot", "uploads", "attachments", id.ToString());
        if (Directory.Exists(employeeAttachmentDir))
            Directory.Delete(employeeAttachmentDir, true);

        return await _employeeRepository.DeleteAsync(id);
    }

    public async Task<EmployeeProfilePictureDto?> UploadProfilePictureAsync(int employeeId, Stream fileStream, string fileName, string contentType)
    {
        var employee = await _employeeRepository.GetByIdAsync(employeeId);
        if (employee == null)
            return null;

        // Validate file type
        var allowedTypes = new[] { "image/jpeg", "image/jpg", "image/png", "image/gif" };
        if (!allowedTypes.Contains(contentType.ToLower()))
            throw new InvalidOperationException("Only image files (JPEG, PNG, GIF) are allowed");

        // Validate file size (5MB)
        if (fileStream.Length > 5 * 1024 * 1024)
            throw new InvalidOperationException("File size must be less than 5MB");

        // Create employee-specific directory
        var webRoot = _environment.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
        var employeeProfileDir = Path.Combine(webRoot, "uploads", "profiles", employeeId.ToString());
        if (!Directory.Exists(employeeProfileDir))
            Directory.CreateDirectory(employeeProfileDir);

        // Delete old profile picture if exists
        var existingPicture = await _profilePictureRepository.GetByEmployeeIdAsync(employeeId);
        if (existingPicture != null)
        {
            var oldFilePath = Path.Combine(webRoot, existingPicture.FilePath.TrimStart('/'));
            if (System.IO.File.Exists(oldFilePath))
                System.IO.File.Delete(oldFilePath);
        }

        // Generate unique filename with timestamp
        var extension = Path.GetExtension(fileName);
        var timestamp = DateTime.Now.Ticks;
        var newFileName = $"profile_{employeeId}_{timestamp}{extension}";
        var filePath = Path.Combine(employeeProfileDir, newFileName);

        // Save file
        fileStream.Position = 0;
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await fileStream.CopyToAsync(stream);
        }

        // Create profile picture record
        var profilePicture = new EmployeeProfilePicture
        {
            EmployeeId = employeeId,
            FilePath = $"/uploads/profiles/{employeeId}/{newFileName}",
            FileName = fileName,
            FileType = extension.TrimStart('.'),
            FileSize = fileStream.Length
        };

        var saved = await _profilePictureRepository.CreateOrUpdateAsync(profilePicture);

        // Update employee UpdatedAt timestamp
        employee.UpdatedAt = DateTime.UtcNow;
        await _employeeRepository.UpdateAsync(employee);

        return new EmployeeProfilePictureDto
        {
            Id = saved.Id,
            EmployeeId = saved.EmployeeId,
            FilePath = saved.FilePath,
            FileName = saved.FileName,
            FileType = saved.FileType,
            FileSize = saved.FileSize,
            UploadedAt = saved.UploadedAt
        };
    }

    public async Task<EmployeeAttachmentResponseDto?> UploadAttachmentAsync(int employeeId, Stream fileStream, string fileName, string contentType)
    {
        var employee = await _employeeRepository.GetByIdAsync(employeeId);
        if (employee == null)
            return null;

        // Validate file extension
        var allowedExtensions = new[] { ".pdf", ".doc", ".docx", ".txt" };
        var extension = Path.GetExtension(fileName).ToLower();
        if (!allowedExtensions.Contains(extension))
            throw new InvalidOperationException("File type not allowed. Allowed: PDF, DOC, DOCX, TXT");

        // Validate file size (10MB)
        if (fileStream.Length > 10 * 1024 * 1024)
            throw new InvalidOperationException("File size must be less than 10MB");

        // Create employee-specific directory
        var webRoot = _environment.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
        var employeeAttachmentDir = Path.Combine(webRoot, "uploads", "attachments", employeeId.ToString());
        if (!Directory.Exists(employeeAttachmentDir))
            Directory.CreateDirectory(employeeAttachmentDir);

        // Generate unique filename with timestamp
        var timestamp = DateTime.Now.Ticks;
        var newFileName = $"{timestamp}_{fileName}";
        var filePath = Path.Combine(employeeAttachmentDir, newFileName);

        // Save file
        fileStream.Position = 0;
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await fileStream.CopyToAsync(stream);
        }

        // Create attachment record
        var attachment = new EmployeeAttachment
        {
            EmployeeId = employeeId,
            FileName = fileName,
            FilePath = $"/uploads/attachments/{employeeId}/{newFileName}",
            FileType = extension.TrimStart('.'),
            FileSize = fileStream.Length,
            UploadedAt = DateTime.UtcNow
        };

        var created = await _attachmentRepository.CreateAsync(attachment);

        // Update employee UpdatedAt timestamp
        employee.UpdatedAt = DateTime.UtcNow;
        await _employeeRepository.UpdateAsync(employee);

        return new EmployeeAttachmentResponseDto
        {
            Id = created.Id,
            EmployeeId = created.EmployeeId,
            FileName = created.FileName,
            FilePath = created.FilePath,
            FileType = created.FileType,
            FileSize = created.FileSize,
            UploadedAt = created.UploadedAt
        };
    }

    public async Task<bool> DeleteAttachmentAsync(int attachmentId)
    {
        var attachment = await _attachmentRepository.GetByIdAsync(attachmentId);
        if (attachment == null)
            return false;

        // Delete physical file
        var filePath = Path.Combine(_environment.WebRootPath ?? "wwwroot", attachment.FilePath.TrimStart('/'));
        if (System.IO.File.Exists(filePath))
            System.IO.File.Delete(filePath);

        // Delete empty attachment folder
        var folderPath = Path.GetDirectoryName(filePath);
        if (Directory.Exists(folderPath) && !Directory.GetFiles(folderPath).Any())
            Directory.Delete(folderPath);

        return await _attachmentRepository.DeleteAsync(attachmentId);
    }

    public async Task<DownloadAttachmentResult?> DownloadAttachmentAsync(int attachmentId)
    {
        var attachment = await _attachmentRepository.GetByIdAsync(attachmentId);
        if (attachment == null)
            return null;

        var filePath = Path.Combine(_environment.WebRootPath ?? "wwwroot", attachment.FilePath.TrimStart('/'));
        if (!System.IO.File.Exists(filePath))
            return null;

        var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
        var mimeType = attachment.FileType switch
        {
            "pdf" => "application/pdf",
            "doc" => "application/msword",
            "docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            "txt" => "text/plain",
            _ => "application/octet-stream"
        };

        return new DownloadAttachmentResult
        {
            FileBytes = fileBytes,
            FileName = attachment.FileName,
            ContentType = mimeType
        };
    }

    private EmployeeResponseDto MapToResponseDto(Employee employee, EmployeeProfilePicture? profilePicture)
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
            ProfilePicture = profilePicture != null ? new EmployeeProfilePictureDto
            {
                Id = profilePicture.Id,
                EmployeeId = profilePicture.EmployeeId,
                FilePath = profilePicture.FilePath,
                FileName = profilePicture.FileName,
                FileType = profilePicture.FileType,
                FileSize = profilePicture.FileSize,
                UploadedAt = profilePicture.UploadedAt
            } : null,
            Attachments = employee.Attachments?.Select(a => new EmployeeAttachmentResponseDto
            {
                Id = a.Id,
                EmployeeId = a.EmployeeId,
                FileName = a.FileName,
                FilePath = a.FilePath,
                FileType = a.FileType,
                FileSize = a.FileSize,
                UploadedAt = a.UploadedAt
            }).ToList()
        };
    }

    public async Task<bool> DeleteProfilePictureAsync(int profilePictureId)
    {
        var profilePicture = await _profilePictureRepository.GetByIdAsync(profilePictureId);
        if (profilePicture == null)
            return false;

        // Delete physical file
        var filePath = Path.Combine(_environment.WebRootPath ?? "wwwroot", profilePicture.FilePath.TrimStart('/'));
        if (System.IO.File.Exists(filePath))
            System.IO.File.Delete(filePath);

        // Delete empty profile folder
        var folderPath = Path.GetDirectoryName(filePath);
        if (Directory.Exists(folderPath) && !Directory.GetFiles(folderPath).Any())
            Directory.Delete(folderPath);

        return await _profilePictureRepository.DeleteByEmployeeIdAsync(profilePicture.EmployeeId);
    }
}