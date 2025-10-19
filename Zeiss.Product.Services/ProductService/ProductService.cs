using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zeiss.Product.DAL.Data;
using Zeiss.Product.DAL.Models;

namespace Zeiss.Product.Services.ProductService
{
    /// <summary>
    /// Provides CRUD and stock-management operations for products persisted in <see cref="ProductDbContext"/>.
    /// </summary>
    public class ProductService : IProductService
    {
        private readonly ProductDbContext _dbContext;
        private readonly ILogger<ProductService> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductService"/> class.
        /// </summary>
        /// <param name="productDbContext">Entity Framework <see cref="ProductDbContext"/> used to access product data.</param>
        /// <param name="logger">Logger used to record operational and error information.</param>
        public ProductService(ProductDbContext productDbContext, ILogger<ProductService> logger) 
        {
            _dbContext = productDbContext;
            _logger = logger;
        }

        /// <summary>
        /// Retrieves all products from the database.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains an <see cref="IEnumerable{ProductModel}"/> of all products.</returns>
        /// <exception cref="Exception">Thrown when an unexpected error occurs while querying the database. See inner details in logged exception.</exception>
        public async Task<IEnumerable<ProductModel>> GetAllProductsAsync()
        {
            try
            {
                return await _dbContext.Products.ToListAsync();
            }
            catch (Exception ex)

            {
                _logger.LogError(ex, "Unhandled Exception");
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Retrieves a single product by its identifier.
        /// </summary>
        /// <param name="productId">The product identifier.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains the <see cref="ProductModel"/> if found; otherwise <c>null</c>.
        /// </returns>
        /// <exception cref="Exception">Thrown when an unexpected error occurs while querying the database. See logged exception for details.</exception>
        public async Task<ProductModel?> GetProductByIdAsync(int productId)
        {
            try 
            {
                return await _dbContext.Products.FindAsync(productId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled Exception");
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Adds a new product to the database. A new product id is generated via <see cref="GenerateNextProductIdAsync"/>
        /// </summary>
        /// <param name="product">The product to add. The <see cref="ProductModel.ProductId"/> will be set by this method.</param>
        /// <returns>A task that represents the asynchronous add operation.</returns>
        /// <exception cref="DbUpdateException">Thrown when the save operation fails at the database layer.</exception>
        public async Task AddProductAsync(ProductModel product)
        {
            product.ProductId = await GenerateNextProductIdAsync();
            using var transaction = await _dbContext.Database.BeginTransactionAsync();

            try
            {
                await _dbContext.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT Products ON");
                _dbContext.Products.Add(product);
                await _dbContext.SaveChangesAsync();
                await _dbContext.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT Products OFF");
                await transaction.CommitAsync();
            }
            catch (DbUpdateException ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Database Update Failed");
                throw new DbUpdateException(ex.Message);
            }
        }

        /// <summary>
        /// Updates an existing product in the database.
        /// </summary>
        /// <param name="product">The product with updated values. The <see cref="ProductModel.ProductId"/> identifies the record to update.</param>
        /// <returns>A task that represents the asynchronous update operation.</returns>
        /// <exception cref="DbUpdateException">Thrown when the save operation fails at the database layer.</exception>
        public async Task UpdateProductAsync(int id,ProductModel product)
        {
            try
            {
                var existing = await _dbContext.Products.FindAsync(id);
                if (existing != null)
                {
                    product.ProductId = existing.ProductId;
                    _dbContext.Entry(existing).CurrentValues.SetValues(product);
                    await _dbContext.SaveChangesAsync();
                }
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database Update Failed");
                throw new DbUpdateException(ex.Message);
            }
        }

        /// <summary>
        /// Deletes a product by its identifier if it exists.
        /// </summary>
        /// <param name="productId">The identifier of the product to remove.</param>
        /// <returns>A task that represents the asynchronous delete operation.</returns>
        /// <exception cref="DbUpdateException">Thrown when the save operation fails at the database layer.</exception>
        public async Task DeleteProductAsync(int productId)
        {
            try
            {
                var product = await _dbContext.Products.FindAsync(productId);
                if (product != null) 
                {
                    _dbContext.Products.Remove(product);
                    await _dbContext.SaveChangesAsync();
                }
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database Update Failed");
                throw new DbUpdateException(ex.Message);
            }
        }

        /// <summary>
        /// Increments the stock quantity for a product.
        /// </summary>
        /// <param name="productId">The product identifier.</param>
        /// <param name="quantity">The quantity to add to the product's stock (must be positive).</param>
        /// <returns>A task that represents the asynchronous increment operation.</returns>
        /// <exception cref="DbUpdateException">Thrown when the save operation fails at the database layer.</exception>
        public async Task IncrementProductStockAsync(int productId, int quantity)
        {
            try
            {
                var product = await _dbContext.Products.FindAsync(productId);
                if (product != null)
                {
                    product.Quantity += quantity;
                    _dbContext.Products.Update(product);
                    await _dbContext.SaveChangesAsync();
                }
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database Update Failed");
                throw new DbUpdateException(ex.Message);
            }
        }

        /// <summary>
        /// Decrements the stock quantity for a product.
        /// </summary>
        /// <param name="productId">The product identifier.</param>
        /// <param name="quantity">The quantity to subtract from the product's stock (must be positive).</param>
        /// <returns>A task that represents the asynchronous decrement operation.</returns>
        /// <exception cref="DbUpdateException">Thrown when the save operation fails at the database layer.</exception>
        public async Task DecrementProductStockAsync(int productId, int quantity)
        {
            try
            {
                var product = await _dbContext.Products.FindAsync(productId);
                if (product != null)
                {
                    product.Quantity -= quantity;
                    _dbContext.Products.Update(product);
                    await _dbContext.SaveChangesAsync();
                }
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database Update Failed");
                throw new DbUpdateException(ex.Message);
            }
        }

        /// <summary>
        /// Generates and persists the next available product identifier in a transactional manner.
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains the newly generated product identifier.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the product id tracker is not initialized or the maximum allowed product id range has been reached.
        /// </exception>
        /// <exception cref="DbUpdateException">Thrown when persisting the tracker update fails at the database layer.</exception>
        public async Task<int> GenerateNextProductIdAsync()
        {
            try
            {
                using var transaction = await _dbContext.Database.BeginTransactionAsync();
                var tracker = await _dbContext.ProductIdTrackers.FirstOrDefaultAsync(t => t.Id == 1);
                if (tracker == null)
                    throw new InvalidOperationException("Product Id Tracker is not Initialized");
                if (tracker.LastId >= 999999)
                    throw new InvalidOperationException("Product Range Reached Maximum Allowed Count");
                tracker.LastId += 1;

                int newProductId = tracker.LastId;

                _dbContext.ProductIdTrackers.Update(tracker);
                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();
                return newProductId;

            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database Update Failed");
                throw new DbUpdateException(ex.Message);
            }
        }
    }
}
