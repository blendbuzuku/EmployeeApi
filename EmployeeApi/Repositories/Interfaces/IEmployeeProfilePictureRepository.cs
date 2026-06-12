using static EmployeeApi.Models.EmployeeModel;

namespace EmployeeApi.Repositories.Interfaces;

public interface IEmployeeProfilePictureRepository
{
    Task<EmployeeProfilePicture?> GetByEmployeeIdAsync(int employeeId);
    Task<EmployeeProfilePicture> CreateOrUpdateAsync(EmployeeProfilePicture profilePicture);
    Task<bool> DeleteByEmployeeIdAsync(int employeeId);
    Task<EmployeeProfilePicture?> GetByIdAsync(int id);
}