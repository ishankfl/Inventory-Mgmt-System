using Inventory_Mgmt_System.Models;

namespace Inventory_Mgmt_System.Repositories
{
    public interface IUserRepository
    {
        Task<User> AddUserRepo(User user);
        Task<List<User>> GetAllUser();
        Task<User> CheckEmailAndPassword(string email, string password);

        Task<User> GetUserByEmailAsync(string Email);

    }
}
