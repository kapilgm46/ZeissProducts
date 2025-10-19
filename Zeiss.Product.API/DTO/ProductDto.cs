using System.ComponentModel.DataAnnotations;

namespace Zeiss.Product.API.DTO
{
    public class ProductRequestDto
    {
        [Required]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "Name must be between 1 and 100 characters.")]
        public string Name { get; set; }

        [Required]
        [Range(1, 100000, ErrorMessage = "Quantity must be between 1 and 100000.")]
        public int Quantity { get; set; }

        [Required]
        [Range(typeof(decimal), "1", "9999999.99", ErrorMessage = "Price must be between 1 and 9999999.99")]
        public decimal Price { get; set; }

        [StringLength(500, MinimumLength = 5, ErrorMessage = "Description must be between 5 and 500 characters.")]
        public string? Description { get; set; }
    }

    public class ProductDto : ProductRequestDto
    {
        public int ProductId { get; set; }
    }
}
