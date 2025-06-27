using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Inventory_Mgmt_System.Models;
using Inventory_Mgmt_System.Dtos;
using Inventory_Mgmt_System.Utils;
using Microsoft.AspNetCore.Authorization;
using Inventory_Mgmt_System.Services.Interfaces;

namespace Inventory_Mgmt_System.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IActivityServices _activityServices;

        public UsersController(IUserService userService, IActivityServices activityServices)
        {
            _userService = userService;
            _activityServices = activityServices;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<User>> PostUser(RegisterUserDTO request)
        {
            try
            {
                PasswordHasher.CreatePasswordHash(request.Password, out byte[] hash, out byte[] salt);

                var isExit = await _userService.GetUserByEmailAsync(request.Email);
                if (isExit != null)
                {
                    return StatusCode(409, new { error = "User with this email already exists" });
                }

                var user = new User
                {
                    Id = Guid.NewGuid(),
                    FullName = request.FullName,
                    Email = request.Email,
                    PasswordHash = Convert.ToBase64String(hash),
                    PasswordSalt = Convert.ToBase64String(salt),
                    Role = request.Role == 0 ? UserRole.Admin : UserRole.Staff,
                };

                var createdUser = await _userService.AddUserService(user);
                if (createdUser != null)
                {
                    var userIdClaim = User?.FindFirst("id")?.Value;
                    if (string.IsNullOrEmpty(userIdClaim))
                    {
                        return Unauthorized("Invalid token or user ID claim not found");
                    }

                    var currentUserId = Guid.Parse(userIdClaim);

                    var activityDto = new ActivityDTO
                    {
                        Action = $"New user added with name {user.FullName}",
                        Status = "success",
                        Type = ActivityType.UserAdded,
                        UserId = currentUserId
                    };

                    await _activityServices.AddNewActivity(activityDto);

                    return CreatedAtAction(nameof(GetUser), new { id = createdUser.Id }, createdUser);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

            return BadRequest("Something went wrong");
        }

        [HttpPost("Login")]
        public async Task<ActionResult> Login(LoginDto request)
        {
            var user = await _userService.GetUserByEmailAsync(request.Email);
            if (user == null)
            {
                return Unauthorized("Invalid email or password.");
            }

            bool isPasswordValid = PasswordHasher.VerifyPasswordHash(request.password, user.PasswordHash, user.PasswordSalt);
            if (!isPasswordValid)
            {
                return Unauthorized("Invalid email or password.");
            }

            string token = JwtUtils.GenerateJwtToken(user);

            var activityDto = new ActivityDTO
            {
                Action = $"User {user.Email} logged in successfully",
                Status = "success",
                Type = ActivityType.UserLoggedIn,
                UserId = user.Id
            };

            await _activityServices.AddNewActivity(activityDto);

            // 5. Return token
            return Ok(new Dictionary<string, string?>
            {
                { "token", token }
            });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(Guid id)
        {
            return NotFound("Get by ID not implemented yet.");
        }

        [HttpGet]
        public async Task<ActionResult<List<User>>> GetAllUserss()
        {
            var userList = await _userService.GetAllUser();
            return Ok(userList);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<ActionResult<User>> DeleteUserById(string id)
        {
            var user = await _userService.DeleteUserById(Guid.Parse(id));
            if (user == null)
            {
                return NotFound($"No user found with ID {id}");
            }

            var userIdClaim = User?.FindFirst("id")?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
            {
                return Unauthorized("Invalid token or user ID claim not found");
            }

            var currentUserId = Guid.Parse(userIdClaim);

            var activityDto = new ActivityDTO
            {
                Action = $"User with Email {user.Email} was deleted",
                Status = "success",
                Type = ActivityType.UserDeleted,
                UserId = currentUserId
            };

            await _activityServices.AddNewActivity(activityDto);

            return Ok(user); 
        }
    }
}
