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
                var userIdClaim = User?.FindFirst("id")?.Value;
                if (string.IsNullOrEmpty(userIdClaim))
                    return Unauthorized("Invalid token or user ID claim not found");

                var currentUserId = Guid.Parse(userIdClaim);

                var (createdUser, errorMessage) = await _userService.RegisterUserAsync(request, currentUserId);

                if (errorMessage != null)
                    return Conflict(new { error = errorMessage });

                return CreatedAtAction(nameof(GetUser), new { id = createdUser!.Id }, createdUser);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("Login")]
        public async Task<ActionResult> Login(LoginDto request)
        {
            try
            {
                var (token, errorMessage) = await _userService.LoginUserAsync(request);

                if (errorMessage != null)
                    return Unauthorized(errorMessage);

                return Ok(new Dictionary<string, string?>
        {
            { "token", token }
        });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(Guid id)
        {
            return NotFound("Get by ID not implemented yet.");
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<List<User>>> GetAllUserss()
        {
            var userList = await _userService.GetAllUser();
            return Ok(userList);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<ActionResult<User>> DeleteUserById(string id)
        {
            var userIdClaim = User?.FindFirst("id")?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized("Invalid token or user ID claim not found");

            var currentUserId = Guid.Parse(userIdClaim);
            var deletedUser = await _userService.DeleteUserById(Guid.Parse(id), currentUserId);

            if (deletedUser == null)
                return NotFound($"No user found with ID {id}");

            return Ok(deletedUser);
        }
    }
}
