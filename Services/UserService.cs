using Inventory_Mgmt_System.Dtos;
using Inventory_Mgmt_System.Models;
using Inventory_Mgmt_System.Repositories.Interfaces;
using Inventory_Mgmt_System.Services.Interfaces;
using Inventory_Mgmt_System.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol;

namespace Inventory_Mgmt_System.Services
{
    public class UserService: IUserService
    {
        private readonly  IUserRepository _userRepository;
        private readonly IActivityServices _activityServices;
        public UserService(IUserRepository userRepository, IActivityServices activityServices)
        {

        _userRepository = userRepository;
        _activityServices = activityServices;
        }
        
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
        public async Task<User> DeleteUserById(Guid id)
        {
            var user = await _userRepository.DeleteUserById(id);
            return user;
        }

        public async Task<User> GetUserById(Guid id)
        {
            var user = await _userRepository.GetUserById(id);
            return user;
        }



    }
}
