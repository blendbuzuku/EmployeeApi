using EmployeeApi.Data;
using EmployeeApi.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using static EmployeeApi.Models.EmployeeModel;

namespace EmployeeApi.Repositories;

public class EmployeeProfilePictureRepository : IEmployeeProfilePictureRepository
{
    private readonly AppDbContext _context;

    public EmployeeProfilePictureRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<EmployeeProfilePicture?> GetByEmployeeIdAsync(int employeeId)
    {
        return await _context.EmployeeProfilePictures
            .FirstOrDefaultAsync(x => x.EmployeeId == employeeId);
    }

    public async Task<EmployeeProfilePicture> CreateOrUpdateAsync(EmployeeProfilePicture profilePicture)
    {
        var existing = await _context.EmployeeProfilePictures
            .FirstOrDefaultAsync(x => x.EmployeeId == profilePicture.EmployeeId);

        if (existing != null)
        {
            // Update existing
            existing.FilePath = profilePicture.FilePath;
            existing.FileName = profilePicture.FileName;
            existing.FileType = profilePicture.FileType;
            existing.FileSize = profilePicture.FileSize;
            existing.UploadedAt = DateTime.UtcNow;

            _context.EmployeeProfilePictures.Update(existing);
        }
        else
        {
            // Create new
            profilePicture.UploadedAt = DateTime.UtcNow;
            await _context.EmployeeProfilePictures.AddAsync(profilePicture);
        }

        await _context.SaveChangesAsync();
        return existing ?? profilePicture;
    }

    public async Task<bool> DeleteByEmployeeIdAsync(int employeeId)
    {
        var profilePicture = await _context.EmployeeProfilePictures
            .FirstOrDefaultAsync(x => x.EmployeeId == employeeId);

        if (profilePicture == null)
            return false;

        _context.EmployeeProfilePictures.Remove(profilePicture);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<EmployeeProfilePicture?> GetByIdAsync(int id)
    {
        return await _context.EmployeeProfilePictures
            .FirstOrDefaultAsync(x => x.Id == id);
    }
}