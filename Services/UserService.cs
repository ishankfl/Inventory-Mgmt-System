using Inventory_Mgmt_System.Dtos;
using Inventory_Mgmt_System.Models;
using Inventory_Mgmt_System.Repositories.Interfaces;
using Inventory_Mgmt_System.Services.Interfaces;
using Inventory_Mgmt_System.Utils;

namespace Inventory_Mgmt_System.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IActivityServices _activityServices;

        public UserService(IUserRepository userRepository, IActivityServices activityServices)
        {
            _userRepository = userRepository;
            _activityServices = activityServices;
        }

        public async Task<(User? User, string? ErrorMessage)> RegisterUserAsync(RegisterUserDTO request, Guid performedByUserId)
        {
            var existingUser = await _userRepository.GetUserByEmailAsync(request.Email);
            if (existingUser != null)
            {
                return (null, "User with this email already exists");
            }

            PasswordHasher.CreatePasswordHash(request.Password, out byte[] hash, out byte[] salt);

            var user = new User
            {
                Id = Guid.NewGuid(),
                FullName = request.FullName,
                Email = request.Email,
                PasswordHash = Convert.ToBase64String(hash),
                PasswordSalt = Convert.ToBase64String(salt),
                Role = request.Role == 0 ? UserRole.Admin : UserRole.Staff,
            };

            var createdUser = await _userRepository.AddUserRepo(user);

            if (createdUser != null)
            {
                var activity = new ActivityDTO
                {
                    Action = $"New user added with name {user.FullName}",
                    Status = "success",
                    Type = ActivityType.UserAdded,
                    UserId = performedByUserId
                };
                await _activityServices.AddNewActivity(activity);
            }

            return (createdUser, null);
        }

        public async Task<(string? Token, string? ErrorMessage)> LoginUserAsync(LoginDto request)
        {
            var user = await _userRepository.GetUserByEmailAsync(request.Email);
            if (user == null)
            {
                return (null, "Invalid email or password.");
            }

            bool isValid = PasswordHasher.VerifyPasswordHash(request.password, user.PasswordHash, user.PasswordSalt);
            if (!isValid)
            {
                return (null, "Invalid email or password.");
            }

            var token = JwtUtils.GenerateJwtToken(user);

            var activity = new ActivityDTO
            {
                Action = $"User {user.Email} logged in successfully",
                Status = "warning",
                Type = ActivityType.UserLoggedIn,
                UserId = user.Id
            };
            await _activityServices.AddNewActivity(activity);

            return (token, null);
        }

        public async Task<List<User>> GetAllUser()
        {
            return await _userRepository.GetAllUser();
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await _userRepository.GetUserByEmailAsync(email);
        }

        public async Task<User> DeleteUserById(Guid userId, Guid performedByUserId)
        {
            var user = await _userRepository.DeleteUserById(userId);

            if (user != null)
            {
                var activity = new ActivityDTO
                {
                    Action = $"User with Email {user.Email} was deleted",
                    Status = "danger",
                    Type = ActivityType.UserDeleted,
                    UserId = performedByUserId
                };
                await _activityServices.AddNewActivity(activity);
            }

            return user;
        }

        public async Task<User> GetUserById(Guid id)
        {
            return await _userRepository.GetUserById(id);
        }
    }
}
