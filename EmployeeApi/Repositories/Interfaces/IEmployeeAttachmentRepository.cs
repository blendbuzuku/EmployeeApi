using static EmployeeApi.Models.EmployeeModel;

namespace EmployeeApi.Repositories.Interfaces;

public interface IEmployeeAttachmentRepository
{
    Task<EmployeeAttachment?> GetByIdAsync(int id);
    Task<EmployeeAttachment> CreateAsync(EmployeeAttachment attachment);
    Task<bool> DeleteAsync(int id);
    Task<IEnumerable<EmployeeAttachment>> GetByEmployeeIdAsync(int employeeId);
}