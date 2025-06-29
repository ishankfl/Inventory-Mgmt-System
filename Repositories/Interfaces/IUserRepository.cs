using Inventory_Mgmt_System.Models;

namespace Inventory_Mgmt_System.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<User> AddUserRepo(User user);
        Task<List<User>> GetAllUser();
        Task<User> CheckEmailAndPassword(string email, string password);

        Task<User> GetUserByEmailAsync(string Email);
        Task<User> DeleteUserById(Guid id);


        Task<int> TotalNumberOfUser();
        Task<User> GetUserById(Guid id);


    }
}
