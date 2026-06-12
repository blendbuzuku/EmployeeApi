using EmployeeApi.Data;
using EmployeeApi.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using static EmployeeApi.Models.EmployeeModel;

namespace EmployeeApi.Repositories;

public class EmployeeRepository : IEmployeeRepository
{
    private readonly AppDbContext _context;

    public EmployeeRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Employee>> GetAllAsync()
    {
        return await _context.Employees
            .Include(e => e.Attachments)
            .Include(e => e.ProfilePicture)  
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Employee?> GetByIdAsync(int id)
    {
        return await _context.Employees
            .Include(e => e.ProfilePicture) 
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<Employee?> GetByIdWithAttachmentsAsync(int id)
    {
        return await _context.Employees
            .Include(e => e.Attachments)
            .Include(e => e.ProfilePicture)  
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<Employee?> GetByEmailAsync(string email)
    {
        return await _context.Employees
            .Include(e => e.ProfilePicture)
            .FirstOrDefaultAsync(x => x.Email == email);
    }

    public async Task<Employee> CreateAsync(Employee employee)
    {
        await _context.Employees.AddAsync(employee);
        await _context.SaveChangesAsync();
        return employee;
    }

    public async Task<Employee?> UpdateAsync(Employee employee)
    {
        var existing = await _context.Employees
            .FirstOrDefaultAsync(x => x.Id == employee.Id);

        if (existing == null)
            return null;

        existing.Name = employee.Name;
        existing.Email = employee.Email;
        existing.Department = employee.Department;
        existing.Salary = employee.Salary;
        existing.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return existing;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var employee = await _context.Employees
            .Include(e => e.Attachments)
            .Include(e => e.ProfilePicture)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (employee == null)
            return false;

        _context.Employees.Remove(employee);
        await _context.SaveChangesAsync();
        return true;
    }
}