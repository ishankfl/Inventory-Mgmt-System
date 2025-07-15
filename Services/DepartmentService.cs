using Inventory_Mgmt_System.Dtos;
using Inventory_Mgmt_System.Models;
using Inventory_Mgmt_System.Repositories.Interfaces;
using Inventory_Mgmt_System.Services.Interfaces;

namespace Inventory_Mgmt_System.Services
{
    public class DepartmentService : IDepartmentService
    {
        private readonly IDepartmentRepository _repository;
        private readonly IActivityServices _activityServices;

        public DepartmentService(IDepartmentRepository repository, IActivityServices activityServices)
        {
            _repository = repository;
            _activityServices = activityServices;
        }

        public async Task<List<Department>> GetAllDepartmentsAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<Department?> GetDepartmentByIdAsync(Guid id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<Department?> GetByIdAsync(Guid id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<Department?> GetByNameAsync(string name)
        {
            return await _repository.GetByNameAsync(name);
        }

        public async Task<Department> CreateDepartmentAsync(DepartmentDto dto, Guid performedByUserId)
        {
            var existing = await _repository.GetByNameAsync(dto.Name);
            if (existing != null)
                throw new InvalidOperationException("Department name already exists");

            var dept = new Department
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                Description = dto.Description
            };

            var createdDepartment = await _repository.AddAsync(dept);

            if (createdDepartment != null)
            {
                var activity = new ActivityDTO
                {
                    Action = $"Department created: {dto.Name}",
                    Status = "success",
                    Type = ActivityType.DepartmentCreated,
                    UserId = performedByUserId
                };
                await _activityServices.AddNewActivity(activity);
            }

            return createdDepartment;
        }

        public async Task<Department?> UpdateDepartmentAsync(Guid id, DepartmentDto dto, Guid performedByUserId)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null)
                return null;

            var duplicate = await _repository.GetByNameAsync(dto.Name);
            if (duplicate != null && duplicate.Id != id)
                return null;

            string oldName = existing.Name;
            existing.Name = dto.Name;
            existing.Description = dto.Description;

            await _repository.UpdateAsync(existing);

            /*    if (updatedDepartment != null)
                {*/
            var activity = new ActivityDTO
            {
                Action = $"Department updated from '{oldName}' to '{dto.Name}'",
                Status = "info",
                Type = ActivityType.DepartmentUpdated,
                UserId = performedByUserId
            };
            await _activityServices.AddNewActivity(activity);


            return new Department {
                Description = dto.Description,

                Id = id,
                Name = dto.Name,
            };
        }

        public async Task<bool> DeleteDepartmentAsync(Guid id, Guid performedByUserId)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null)
                return false;

            await _repository.DeleteAsync(id);


            var activity = new ActivityDTO
            {
                Action = $"Department deleted: {existing.Name}",
                Status = "danger",
                Type = ActivityType.DepartmentDeleted,
                UserId = performedByUserId
            };
            await _activityServices.AddNewActivity(activity);
        

            return true;
        }
    }
}