using Inventory_Mgmt_System.Models;

namespace Inventory_Mgmt_System.Services.Interfaces
{
    public interface IUserService
    {
        Task<User> AddUserService(User user);
        Task<List<User>> GetAllUser();
        Task<User> CheckEmailAndPassword(string email, string password);

        Task<User> GetUserByEmailAsync(string Email);
        Task<User> DeleteUserById(Guid id);
        Task<User> GetUserById(Guid id);

    }
}
