using Microsoft.AspNetCore.Mvc;
using TiendaInspire.Catalog.Services;
using static TiendaInspire.Catalog.Dtos.ProductDTO;

namespace TiendaInspire.Catalog.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class ProductsController(ProductService productService) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductListResponse>>> GetAll(
            [FromQuery] int? categoryId = null,
            [FromQuery] bool? isActive = null,
            [FromQuery] string? search = null)
        {
            var result = await productService.GetAllAsync(categoryId, search);
            return Ok(result.Data);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ProductResponse>> GetById(int id)
        {
            var result = await productService.GetByIdAsync(id);

            if (!result.Succeeded)
                return NotFound();

            return Ok(result.Data);
        }

        [HttpPost]
        public async Task<ActionResult<ProductResponse>> Create(CreateProductRequest request)
        {
            var result = await productService.CreateAsync(request);

            if (!result.Succeeded)
                return BadRequest(string.Join(", ", result.Errors));

            return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result.Data);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<ProductResponse>> Update(int id, UpdateProductRequest request)
        {
            var result = await productService.UpdateAsync(id, request);

            if (!result.Succeeded)
            {
                if (result.Errors.Any(e => e.Contains("not found")))
                    return NotFound();

                return BadRequest(string.Join(", ", result.Errors));
            }

            return Ok(result.Data);
        }

        [HttpPatch("{id:int}/stock")]
        public async Task<IActionResult> UpdateStock(int id, UpdateStockRequest request)
        {
            var result = await productService.UpdateStockAsync(id, request.Quantity);

            if (!result.Succeeded)
                return NotFound();

            return NoContent();
        }


        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await productService.DeleteAsync(id);

            if (!result.Succeeded)
                return NotFound();

            return NoContent();
        }
    }
}
