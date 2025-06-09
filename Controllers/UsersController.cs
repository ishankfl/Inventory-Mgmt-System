using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Inventory_Mgmt_System.Models;
using Inventory_Mgmt_System.Services;

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
        public async Task<ActionResult<User>> PostUser(User user)
        {
            var createdUser = await _userService.AddUserService(user);
            return CreatedAtAction(nameof(GetUser), new { id = createdUser.Id }, createdUser);
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
        public async Task<ActionResult<List<User>>> GetAllUserss(){

            var userList = await _userService.GetAllUser();
            return Ok(userList);

        }


}
}
