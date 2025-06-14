using Inventory_Mgmt_System.Dtos;
using Inventory_Mgmt_System.Models;
using Inventory_Mgmt_System.Repositories.Interfaces;
using Inventory_Mgmt_System.Services.Interfaces;

namespace Inventory_Mgmt_System.Services
{

    public class DepartmentService : IDepartmentService
        {
            private readonly IDepartmentRepository _repository;

            public DepartmentService(IDepartmentRepository repository)
            {
                _repository = repository;
            }

            public async Task<List<Department>> GetAllDepartmentsAsync()
            {
                var departments = await _repository.GetAllAsync();
            /*return departments.Select(d => new Department
            {
                Id = d.Id,
                Name = d.Name,
                Description = d.Description
            }).ToList();*/
            return departments;
        }

        public async Task<Department?> GetDepartmentByIdAsync(Guid id)
            {
                var dept = await _repository.GetByIdAsync(id);
                if (dept == null) return null;

            /*   return new DepartmentDto
               {
                   Id = dept.Id,
                   Name = dept.Name,
                   Description = dept.Description
               };*/
            return dept;
            }

            public async Task<Department> CreateDepartmentAsync(DepartmentDto dto)
            {
                var dept = new Department
                {
                    Name = dto.Name,
                    Description = dto.Description
                };

                var created = await _repository.AddAsync(dept);
            /* return new DepartmentDto
             {
                 Id = created.Id,
                 Name = created.Name,
                 Description = created.Description
             };*/
            return created;
            }

            public async Task<bool> UpdateDepartmentAsync(Guid id, DepartmentDto dto)
            {
                var existing = await _repository.GetByIdAsync(id);
                if (existing == null) return false;

                existing.Name = dto.Name;
                existing.Description = dto.Description;
                await _repository.UpdateAsync(existing);
                return true;
            }

            public async Task<bool> DeleteDepartmentAsync(Guid id)
            {
                var existing = await _repository.GetByIdAsync(id);
                if (existing == null) return false;

                await _repository.DeleteAsync(id);
                return true;
            }

        public Task<bool> UpdateDepartmentAsync(Guid id, Department dto)
        {
            throw new NotImplementedException();
        }

        /*public Task<Department> CreateDepartmentAsync(Department dto)
        {
            throw new NotImplementedException();
        }*/
    }
}
