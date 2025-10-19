using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Zeiss.Product.API.DTO;
using Zeiss.Product.DAL.Models;
using Zeiss.Product.Services.ProductService;

namespace Zeiss.Product.API.Controllers
{
    /// <summary>
    /// Controller for managing product-related operations.
    /// </summary>
    [ApiController]
    [Route("api/[Controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _service;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductsController"/> class.
        /// </summary>
        /// <param name="service">Service for product operations.</param>
        /// <param name="mapper">AutoMapper instance for DTO mapping.</param>
        public ProductsController(IProductService service, IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
        }

        /// <summary>
        /// Retrieves all products.
        /// </summary>
        /// <returns>
        /// An <see cref="ActionResult{IEnumerable{ProductDto}}"/> containing a list of product DTOs.
        /// Returns HTTP 200 (OK) with the mapped DTO collection.
        /// </returns>

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetAllProducts()
        {
            var products = await _service.GetAllProductsAsync();
            return Ok(_mapper.Map<IEnumerable<ProductDto>>(products));
        }

        /// <summary>
        /// Retrieves a product by its ID.
        /// </summary>
        /// <param name="id">The product identifier.</param>
        /// <returns>
        /// An <see cref="ActionResult{ProductDto}"/> containing the product DTO if found.
        /// Returns HTTP 200 (OK) with the product, or HTTP 404 (NotFound) if the product does not exist.
        /// </returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDto>> GetProductById(int id)
        {
            var product = await _service.GetProductByIdAsync(id);
            if (product == null)
                return NotFound();

            return Ok(_mapper.Map<ProductDto>(product));
        }

        /// <summary>
        /// Creates a new product.
        /// </summary>
        /// <param name="productDto">The product DTO to create.</param>
        /// <returns>
        /// An <see cref="ActionResult{ProductDto}"/> indicating the outcome of the create operation.
        /// Returns HTTP 201 (Created) on success.
        /// </returns>
        [HttpPost]
        public async Task<ActionResult<ProductDto>> CreateProduct([FromBody] ProductRequestDto productDto)
        {
            var product = _mapper.Map<ProductModel>(productDto);
            await _service.AddProductAsync(product);

            var createdDto = _mapper.Map<ProductDto>(product);
            return CreatedAtAction(nameof(GetProductById), new { id = product.ProductId }, createdDto);
        }

        /// <summary>
        /// Updates an existing product.
        /// </summary>
        /// <param name="id">The id of the product to update.</param>
        /// <param name="productDto">The updated product DTO.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> indicating the result of the update.
        /// Returns HTTP 400 (BadRequest) when the id does not match the DTO, HTTP 404 (NotFound) when the product is missing, or HTTP 204 (NoContent) on success.
        /// </returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] ProductRequestDto productDto)
        {
            var updatedProduct = _mapper.Map<ProductModel>(productDto);
            await _service.UpdateProductAsync(id,updatedProduct);
            return Ok();
        }

        /// <summary>
        /// Increments the stock quantity of a product.
        /// </summary>
        /// <param name="id">The product identifier.</param>
        /// <param name="quantity">The amount to increment.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> indicating the outcome of the operation.
        /// Returns HTTP 200 (OK) on success.
        /// </returns>
        //[Route("add-to-stock")]
        [HttpPut("add-to-stock/{id}/{quantity}")]
        public async Task<IActionResult> IncrementProductQuantity(int id, int quantity)
        {
            if(quantity >= 100000 && quantity <= 0)
                return BadRequest("Quantity must be between 1 and 100000.");
            if(id < 100000 && id > 999999)
                return BadRequest("Product ID must be Valid.");

            await _service.IncrementProductStockAsync(id, quantity);
            return Ok();
        }

        /// <summary>
        /// Decrements the stock quantity of a product.
        /// </summary>
        /// <param name="id">The product identifier.</param>
        /// <param name="quantity">The amount to decrement.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> indicating the outcome of the operation.
        /// Returns HTTP 200 (OK) on success.
        /// </returns>
        [HttpPut("decrement-stock/{id}/{quantity}")]
        public async Task<IActionResult> DecrementProductQuantity(int id, int quantity)
        {
            if (quantity >= 100000 && quantity <= 0)
                return BadRequest("Quantity must be between 1 and 100000.");
            if (id < 100000 && id > 999999)
                return BadRequest("Product ID must be Valid.");

            await _service.DecrementProductStockAsync(id, quantity);
            return Ok();
        }

        /// <summary>
        /// Deletes a product by id.
        /// </summary>
        /// <param name="id">The product identifier to delete.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> indicating the result of the delete operation.
        /// Returns HTTP 404 (NotFound) when the product does not exist, or HTTP 204 (NoContent) on success.
        /// </returns>
        [HttpDelete]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var existingProduct = await _service.GetProductByIdAsync(id);
            if (existingProduct == null)
                return NotFound();

            await _service.DeleteProductAsync(id);
            return NoContent();
        }

    }
}
