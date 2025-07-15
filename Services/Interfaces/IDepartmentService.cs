using Inventory_Mgmt_System.Dtos;
using Inventory_Mgmt_System.Models;

namespace Inventory_Mgmt_System.Services.Interfaces
{
    public interface IDepartmentService
    {
        Task<List<Department>> GetAllDepartmentsAsync();
        Task<Department?> GetDepartmentByIdAsync(Guid id);
        Task<Department?> GetByIdAsync(Guid id);               // Redundant, but included as per implementation
        Task<Department?> GetByNameAsync(string name);

        Task<Department> CreateDepartmentAsync(DepartmentDto dto, Guid performedByUserId);
        Task<Department?> UpdateDepartmentAsync(Guid id, DepartmentDto dto, Guid performedByUserId);
        Task<bool> DeleteDepartmentAsync(Guid id, Guid performedByUserId);
    }
}
