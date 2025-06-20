﻿using Inventory_Mgmt_System.Dtos;
using Inventory_Mgmt_System.Models;

namespace Inventory_Mgmt_System.Services.Interfaces
{
    public interface IDepartmentService
    {
        Task<List<Department>> GetAllDepartmentsAsync();
        Task<Department?> GetDepartmentByIdAsync(Guid id);
        Task<Department> CreateDepartmentAsync(DepartmentDto dto);
        Task<bool> DeleteDepartmentAsync(Guid id);
        // Task<bool> UpdateDepartmentAsync(Guid id, DepartmentDto dto);
        Task<Department?> GetByNameAsync(string name);


        Task<Department?> GetByIdAsync(Guid id);
        Task<Department> UpdateDepartmentAsync(Guid id, DepartmentDto dto);
        // Task<Department> UpdateDepartmentAsync(Guid id, DepartmentDto dto);
    }
}
