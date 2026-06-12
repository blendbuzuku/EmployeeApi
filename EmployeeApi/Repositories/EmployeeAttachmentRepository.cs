using EmployeeApi.Data;
using EmployeeApi.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using static EmployeeApi.Models.EmployeeModel;

namespace EmployeeApi.Repositories;

public class EmployeeAttachmentRepository : IEmployeeAttachmentRepository
{
    private readonly AppDbContext _context;

    public EmployeeAttachmentRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<EmployeeAttachment?> GetByIdAsync(int id)
    {
        return await _context.EmployeeAttachments
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<IEnumerable<EmployeeAttachment>> GetByEmployeeIdAsync(int employeeId)
    {
        return await _context.EmployeeAttachments
            .AsNoTracking()
            .Where(x => x.EmployeeId == employeeId)
            .ToListAsync();
    }

    public async Task<EmployeeAttachment> CreateAsync(EmployeeAttachment attachment)
    {
        await _context.EmployeeAttachments.AddAsync(attachment);
        await _context.SaveChangesAsync();
        return attachment;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var attachment = await _context.EmployeeAttachments
            .FirstOrDefaultAsync(x => x.Id == id);

        if (attachment == null)
            return false;

        _context.EmployeeAttachments.Remove(attachment);
        await _context.SaveChangesAsync();
        return true;
    }
}