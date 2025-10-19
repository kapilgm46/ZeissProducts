using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;
using Zeiss.Product.API.Controllers;
using Zeiss.Product.API.DTO;
using Zeiss.Product.API.Helpers;
using Zeiss.Product.DAL.Models;
using Zeiss.Product.Services.ProductService;

namespace Zeiss.Product.Test
{
    [TestFixture]
    public class ProductsControllerTests
    {
        private Mock<IProductService> _serviceMock;
        private ProductsController _controller;
        private IMapper _mapper;

        [SetUp]
        public void Setup()
        {
            _serviceMock = new Mock<IProductService>();
            var config = new AutoMapper.MapperConfiguration(cfg => {
                cfg.AddProfile(new MappingProfile());
            });

            _mapper = config.CreateMapper();
            _controller = new ProductsController(_serviceMock.Object, _mapper);
        }

        [Test]
        public async Task GetAll_ReturnsOkResult()
        {
            _serviceMock.Setup(r => r.GetAllProductsAsync()).ReturnsAsync(new List<ProductModel> { new ProductModel { ProductId = 1, Name = "Test", Description = "Desc", Quantity = 10, Price = 99.99M } });

            var result = await _controller.GetAllProducts();

            Assert.IsInstanceOf<OkObjectResult>(result.Result);
        }

        [Test]
        public async Task GetById_ReturnsNotFound_WhenMissing()
        {
            _serviceMock.Setup(r => r.GetProductByIdAsync(It.IsAny<int>())).ReturnsAsync((ProductModel?)null);

            var result = await _controller.GetProductById(1);

            Assert.IsInstanceOf<NotFoundResult>(result.Result);
        }

        [Test]
        public async Task Create_ReturnsCreatedAtAction()
        {
            var dto = new ProductRequestDto { Name = "New", Description = "Desc", Quantity = 5, Price = 50M };

            _serviceMock.Setup(r => r.AddProductAsync(It.IsAny<ProductModel>())).Returns(Task.CompletedTask);

            var result = await _controller.CreateProduct(dto);

            Assert.IsInstanceOf<CreatedAtActionResult>(result.Result);
        }

        [Test]
        public async Task Delete_ReturnsNoContent()
        {
            var product = new ProductModel { ProductId = 1, Name = "ToDelete", Description = "Desc", Quantity = 1, Price = 10M };
            _serviceMock.Setup(r => r.GetProductByIdAsync(1)).ReturnsAsync(product);
            _serviceMock.Setup(r => r.DeleteProductAsync(1)).Returns(Task.CompletedTask).Verifiable();

            var result = await _controller.DeleteProduct(1);

            Assert.IsInstanceOf<NoContentResult>(result);
            _serviceMock.Verify(r => r.DeleteProductAsync(1), Times.Once);
        }

        // New tests for Increment/Decrement product quantity

        [Test]
        public async Task IncrementProductQuantity_CallsService_AndReturnsOk()
        {
            // Arrange
            var productId = 2;
            var quantity = 5;
            _serviceMock.Setup(s => s.IncrementProductStockAsync(productId, quantity)).Returns(Task.CompletedTask).Verifiable();

            // Act
            var result = await _controller.IncrementProductQuantity(productId, quantity);

            // Assert
            Assert.IsInstanceOf<OkResult>(result);
            _serviceMock.Verify(s => s.IncrementProductStockAsync(productId, quantity), Times.Once);
        }

        [Test]
        public async Task DecrementProductQuantity_CallsService_AndReturnsOk()
        {
            // Arrange
            var productId = 3;
            var quantity = 2;
            _serviceMock.Setup(s => s.DecrementProductStockAsync(productId, quantity)).Returns(Task.CompletedTask).Verifiable();

            // Act
            var result = await _controller.DecrementProductQuantity(productId, quantity);

            // Assert
            Assert.IsInstanceOf<OkResult>(result);
            _serviceMock.Verify(s => s.DecrementProductStockAsync(productId, quantity), Times.Once);
        }
    }
}