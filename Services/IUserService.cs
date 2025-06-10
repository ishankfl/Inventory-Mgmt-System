using Inventory_Mgmt_System.Models;

namespace Inventory_Mgmt_System.Services
{
    public interface IUserService
    {
        Task<User> AddUserService(User user);
        Task<List<User>> GetAllUser();
        Task<User> CheckEmailAndPassword(string email, string password);

        Task<User> GetUserByEmailAsync(string Email);
    }
}
