using EmployeeApi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FileUploadController : ControllerBase
{
    private readonly IEmployeeService _service;

    public FileUploadController(IEmployeeService service)
    {
        _service = service;
    }

    [HttpPost("upload-profile/{employeeId}")]
    public async Task<IActionResult> UploadProfilePicture(int employeeId, IFormFile file)
    {
        try
        {
            if (file == null || file.Length == 0)
                return BadRequest(new { message = "No file uploaded" });

            using var stream = file.OpenReadStream();
            var profilePicture = await _service.UploadProfilePictureAsync(
                employeeId,
                stream,
                file.FileName,
                file.ContentType);

            if (profilePicture == null)
                return NotFound(new { message = "Employee not found" });

            return Ok(new
            {
                id = profilePicture.Id,
                employeeId = profilePicture.EmployeeId,
                filePath = profilePicture.FilePath,
                fileName = profilePicture.FileName,
                fileType = profilePicture.FileType,
                fileSize = profilePicture.FileSize,
                uploadedAt = profilePicture.UploadedAt,
                message = "Profile picture uploaded successfully"
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = $"Error uploading file: {ex.Message}" });
        }
    }

    [HttpPost("upload-attachment/{employeeId}")]
    public async Task<IActionResult> UploadAttachment(int employeeId, IFormFile file)
    {
        try
        {
            if (file == null || file.Length == 0)
                return BadRequest(new { message = "No file uploaded" });

            using var stream = file.OpenReadStream();
            var attachment = await _service.UploadAttachmentAsync(
                employeeId,
                stream,
                file.FileName,
                file.ContentType);

            if (attachment == null)
                return NotFound(new { message = "Employee not found" });

            return Ok(new
            {
                attachment.Id,
                attachment.EmployeeId,
                attachment.FileName,
                attachment.FilePath,
                attachment.FileType,
                attachment.FileSize,
                attachment.UploadedAt,
                message = "Attachment uploaded successfully"
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = $"Error uploading file: {ex.Message}" });
        }
    }

    [HttpDelete("delete-attachment/{attachmentId}")]
    public async Task<IActionResult> DeleteAttachment(int attachmentId)
    {
        try
        {
            var success = await _service.DeleteAttachmentAsync(attachmentId);
            if (!success)
                return NotFound(new { message = "Attachment not found" });

            return Ok(new { message = "Attachment deleted successfully" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = $"Error deleting file: {ex.Message}" });
        }
    }

    [HttpGet("download-attachment/{attachmentId}")]
    public async Task<IActionResult> DownloadAttachment(int attachmentId)
    {
        try
        {
            var result = await _service.DownloadAttachmentAsync(attachmentId);
            if (result == null)
                return NotFound(new { message = "Attachment not found" });

            return File(result.FileBytes, result.ContentType, result.FileName);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = $"Error downloading file: {ex.Message}" });
        }
    }

    [HttpDelete("delete-profile-picture/{profilePictureId}")]
    public async Task<IActionResult> DeleteProfilePicture(int profilePictureId)
    {
        try
        {
            var success = await _service.DeleteProfilePictureAsync(profilePictureId);
            if (!success)
                return NotFound(new { message = "Profile picture not found" });

            return Ok(new { message = "Profile picture deleted successfully" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = $"Error deleting file: {ex.Message}" });
        }
    }
}