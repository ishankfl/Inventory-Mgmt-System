using Inventory_Mgmt_System.Dtos;
using Inventory_Mgmt_System.Models;
using Inventory_Mgmt_System.Services;
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

        public ProductController(IProductService productService)
        {
            _productService = productService;
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

            return Ok(updatedProduct);
        }


        // DELETE: api/Product/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string idInString)
        {
            Guid id = Guid.Parse(idInString);
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
                return NotFound();

            await _productService.DeleteProductAsync(id);
            return NoContent();
        }
    }
}
