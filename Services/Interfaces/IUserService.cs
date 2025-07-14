using Inventory_Mgmt_System.Dtos;
using Inventory_Mgmt_System.Models;

namespace Inventory_Mgmt_System.Services.Interfaces
{
    public interface IUserService
    {

            Task<(User? User, string? ErrorMessage)> RegisterUserAsync(RegisterUserDTO request, Guid performedByUserId);
            Task<(string? Token, string? ErrorMessage)> LoginUserAsync(LoginDto request);

            Task<List<User>> GetAllUser();
            Task<User> GetUserByEmailAsync(string email);
            Task<User> DeleteUserById(Guid userId, Guid performedByUserId);
            Task<User> GetUserById(Guid id);
        

    }
}
