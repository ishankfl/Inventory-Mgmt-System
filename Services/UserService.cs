using Inventory_Mgmt_System.Models;
using Inventory_Mgmt_System.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Inventory_Mgmt_System.Services
{
    public class UserService: IUserService
    {
        private IUserRepository _userRepository;
        public UserService(IUserRepository userRepository)
        {

        _userRepository = userRepository; }

       public async  Task<User> AddUserService(User user)
        {
            var userNew = await _userRepository.AddUserRepo(user);
            return userNew;
        }

        public async Task<List<User>> GetAllUser()
        {
            var userList = await _userRepository.GetAllUser();
            return userList;
        }
        public async Task<User> CheckEmailAndPassword(string email, string password)
        {
            var user = await _userRepository.CheckEmailAndPassword(email, password);

            return user;
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            var user = await _userRepository.GetUserByEmailAsync(email);

            return user;
        }



    }
}
