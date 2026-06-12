using static EmployeeApi.Models.EmployeeModel;

namespace EmployeeApi.Repositories.Interfaces;

public interface IEmployeeRepository
{
    Task<IEnumerable<Employee>> GetAllAsync();
    Task<Employee?> GetByIdAsync(int id);
    Task<Employee?> GetByEmailAsync(string email);
    Task<Employee> CreateAsync(Employee employee);
    Task<Employee?> UpdateAsync(Employee employee);
    Task<bool> DeleteAsync(int id);
    Task<Employee?> GetByIdWithAttachmentsAsync(int id);
}