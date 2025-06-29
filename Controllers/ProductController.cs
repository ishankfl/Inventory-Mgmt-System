using Inventory_Mgmt_System.Dtos;
using Inventory_Mgmt_System.Models;
using Inventory_Mgmt_System.Services;
using Inventory_Mgmt_System.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.JSInterop.Infrastructure;
using System;
using System.Threading.Tasks;

namespace Inventory_Mgmt_System.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase  // Use ControllerBase for APIs
    {
        private readonly IProductService _productService;
        private readonly IActivityServices _activityServices;
        private readonly IUserService _userService;

        public ProductController(IProductService productService, IActivityServices activityServices, IUserService userService)
        {
            this._activityServices = activityServices;
            _productService = productService;
            _userService = userService;
        }

        // GET: api/Product
        [HttpGet]
        public async Task<IActionResult> GetAllProduct()
        {
            var products = await _productService.GetAllProductsAsync();
            return Ok(products);
        }

        // GET: api/Product/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> Details(string id)
        {
            try
            {

                Guid pId = Guid.Parse(id);
                var product = await _productService.GetProductByIdAsync(pId);
                if (product == null)
                    return NotFound();

                return Ok(product);
            }
            catch (Exception ex) { 
                return BadRequest(ex.Message);
            }

        }

        // POST: api/Product
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AddProductDto productDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var isExist = await _productService.GetProductByNameAsync(productDto.Name);
                if (isExist !=null)
                {
                    return StatusCode(409, $"Product with this name already exist:");

                }

                // Manually map DTO to Product
                var product = new Product
                {
                    Id = Guid.NewGuid(),
                    Name = productDto.Name,
                    Description = productDto.Description,
                    Quantity = productDto.Quantity,
                    Price = productDto.Price,
                    CategoryId = productDto.CategoryId,
                    UserId = productDto.UserId,
                    CreatedAt = DateTime.UtcNow
                };
             
                var createdProduct = await _productService.CreateProductAsync(product);

                var userIdClaim = User?.FindFirst("id")?.Value;
                if (string.IsNullOrEmpty(userIdClaim))
                {
                    return Unauthorized("Invalid token or user ID claim not found");
                }

                var currentUserId = Guid.Parse(userIdClaim);
             var getUserById = await _userService.GetUserById(Guid.Parse(currentUserId.ToString()));
                var activityDto = new ActivityDTO
                {
                    Action = $"User with Email {getUserById.Email} added new product : {product.Name}",
                    Status = "success",
                    Type = ActivityType.ProductAdded,
                    UserId = getUserById.Id
                };

                await _activityServices.AddNewActivity(activityDto);
                return Ok( createdProduct);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> Edit(string id, [FromBody] AddProductDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            
            Product product = new Product
            {
                Id = Guid.Parse(id),  // 
                Name = dto.Name,
                Description = dto.Description,
                Quantity = dto.Quantity,
                Price = dto.Price,
                CategoryId = dto.CategoryId,
                UserId = dto.UserId
            };

            var updatedProduct = await _productService.UpdateProductAsync(product);
            if (updatedProduct == null)
                return NotFound();
            var userIdClaim = User?.FindFirst("id")?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
            {
                return Unauthorized("Invalid token or user ID claim not found");
            }

            var currentUserId = Guid.Parse(userIdClaim);
            var getUserById = await _userService.GetUserById(Guid.Parse(currentUserId.ToString()));

            var activityDto = new ActivityDTO
            {
                Action = $"User with Email {getUserById.Email} updated the product: {updatedProduct.Name}",
                Status = "warning",
                Type = ActivityType.ProductUpdated,
                UserId = updatedProduct.UserId
            };

            await _activityServices.AddNewActivity(activityDto);

            return Ok(updatedProduct);
        }


        // DELETE: api/Product/{id}
        [HttpDelete("{idInString}")]
        public async Task<IActionResult> Delete(string idInString)
        {
            Guid id = Guid.Parse(idInString);
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
                return NotFound();

            await _productService.DeleteProductAsync(id);
            var userIdClaim = User?.FindFirst("id")?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
            {
                return Unauthorized("Invalid token or user ID claim not found");
            }

            var currentUserId = Guid.Parse(userIdClaim);

            var activityDto = new ActivityDTO
            {
                Action = $"User with Email {product.User.Email} deleted the product: {product.Name}",
                Status = "danger",
                Type = ActivityType.ProductDeleted,
                UserId = product.UserId
            };

            await _activityServices.AddNewActivity(activityDto);
            return NoContent();
        }
    }
}
