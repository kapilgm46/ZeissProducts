using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zeiss.Product.DAL.Models;

namespace Zeiss.Product.Services.ProductService
{
    public interface IProductService
    {
        Task<IEnumerable<ProductModel>> GetAllProductsAsync();
        Task<ProductModel?> GetProductByIdAsync(int productId);
        Task AddProductAsync(ProductModel product);
        Task UpdateProductAsync(int id, ProductModel product);
        Task DeleteProductAsync(int productId);
        Task IncrementProductStockAsync(int productId, int quantity);
        Task DecrementProductStockAsync(int productId, int quantity);

    }
}
