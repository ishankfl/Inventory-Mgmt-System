using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Inventory_Mgmt_System.Models;
using Inventory_Mgmt_System.Dtos;
using NuGet.Common;
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

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        // POST: api/Users
        [HttpPost]
        public async Task<ActionResult<User>> PostUser(RegisterUserDTO request)
        {
            PasswordHasher.CreatePasswordHash(request.Password, out byte[] hash, out byte[] salt);

            var user = new User
            {
                Id = Guid.NewGuid(),
                FullName = request.FullName,
                Email = request.Email,
                PasswordHash = Convert.ToBase64String(hash),
                PasswordSalt = Convert.ToBase64String(salt),
                Role =  request.Role == 1? UserRole.Admin : UserRole.Staff,
            };

            

            var createdUser = await _userService.AddUserService(user);
            if (createdUser != null)
            {
                return CreatedAtAction(nameof(GetUser), new { id = createdUser.Id }, createdUser);
            }

           return BadRequest("Some thing went wrong");
        }


        [HttpPost("Login")]
        public async Task<ActionResult> Login(LoginDto request)
        {
            // 1. Get user by email
            var user = await _userService.GetUserByEmailAsync(request.Email);
            if (user == null)
            {
                Console.WriteLine("User not found");

                return Unauthorized("Invalid email or password.");
            }

            // 2. Verify password using stored hash and salt
            bool isPasswordValid = PasswordHasher.VerifyPasswordHash(request.password, user.PasswordHash, user.PasswordSalt);
            if (!isPasswordValid)
            {
                return Unauthorized("Invalid email or password.");
            }

            // 3. Generate JWT token
            string token = JwtUtils.GenerateJwtToken(user);

            // 4. Return token as a dictionary (or just return it as a string if preferred)
            var dict = new Dictionary<string, string?>
    {
        { "token", token }
    };

            return Ok(dict);
        }



        // GET: api/Users/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(Guid id)
        {
            // You would need to add GetUserByIdService in IUserService and UserService.
            return NotFound("Get by ID not implemented yet.");
        }

        // For getting user list
        [HttpGet]
        public async Task<ActionResult<List<User>>> GetAllUserss()
        {

            var userList = await _userService.GetAllUser();
            return Ok(userList);

        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<User>> DeleteUserById(string id)
        {

            var userList = await _userService.DeleteUserById(Guid.Parse(id));
            return Ok(userList);

        }

    }
}
