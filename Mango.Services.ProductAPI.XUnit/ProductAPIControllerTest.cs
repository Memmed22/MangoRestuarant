using Mango.Services.ProductAPI.Controllers;
using Mango.Services.ProductAPI.Model.Dto;
using Mango.Services.ProductAPI.Repository;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Mango.Services.ProductAPI.XUnit
{
    public class ProductAPIControllerTest
    {
        private readonly Mock<IProductRepository> productRepositoryMock;
        private ProductAPIController _sut;

        private ProductDto productDto1;
        private ProductDto productDto2;

        public ProductAPIControllerTest()
        {
            productRepositoryMock = new Mock<IProductRepository>();
            _sut = new(productRepositoryMock.Object);

            productDto1 = new()
            {
                Id = 1,
                CategoryName = "cat1",
                Description = "desc1",
                ImageUrl = "Url1",
                Name = "name1",
                Price = 12
            };
            productDto2 = new ProductDto()
            {
                Id = 2,
                CategoryName = "cat2",
                Description = "desc2",
                ImageUrl = "Url2",
                Name = "name2",
                Price = 13
            };
        }

        [Fact]
        public void Get_RequestWithMockedProductRepository_ReturnSuccessResponseDto()
        {
            //Arrange
            IEnumerable<ProductDto> productDtos = new List<ProductDto> { productDto1, productDto2 };

            productRepositoryMock.Setup(u => u.GetProducts()).ReturnsAsync(productDtos);

            //Act
            var result =(ResponseDto) _sut.Get().GetAwaiter().GetResult();

            //Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(productDto1.Name, (result.Result as List<ProductDto>).First().Name);
        }

        [Fact]
        public void Get_Request_VerifyGetProductsMethodInvoked()
        {
            var result = _sut.Get();
            productRepositoryMock.Verify(x => x.GetProducts(), Times.Once);
        }

        [Fact]
        public void GetById_RequestUnavailbleMockedRepoWithId_ReturnIsSuccessFalseResponeDto()
        {
            ResponseDto responseDto = new();

            productRepositoryMock.Setup(u => u.GetProductById(It.IsAny<int>()))
                .Callback((int i) => { 
                    if(i<0)
                    responseDto.IsSuccess =  false;
                })
                .ReturnsAsync(new ProductDto());

            var result = (ResponseDto) _sut.Get(-1).GetAwaiter().GetResult();

            Assert.False(responseDto.IsSuccess);
        }

        [Fact]
        public void GetById_RequestMockedRepoWithId_ReturnSuccessResponeDtoWithSpecifiedProduct()
        {
            //Arrange
            productRepositoryMock.Setup(u => u.GetProductById(It.Is<int>(t => t > 0))).
                ReturnsAsync(productDto1);

            //Act
            var result = (ResponseDto)_sut.Get(1).GetAwaiter().GetResult();

            //Assert
            Assert.True(result.IsSuccess);

        }

        [Fact]
        public void GetById_Request_VerifyGetProductsMethodInvokedOnce()
        {
            var result = _sut.Get();
            productRepositoryMock.Verify(x => x.GetProducts(), Times.Once);
        }

        [Fact]
        public void Post_NewProductDto_ReturnSuccessResponseDto() {
            productRepositoryMock.Setup(u => u.CreateUpdate(productDto1)).ReturnsAsync(productDto1);

            var result = (ResponseDto) _sut.Post(productDto1).GetAwaiter().GetResult();

            Assert.NotNull(result);
            Assert.True(result.IsSuccess);
        }


        [Fact]
        public void Put_ProductDto_ReturnSuccessResponseDto()
        {
            productRepositoryMock.Setup(u => u.CreateUpdate(It.IsAny<ProductDto>())).ReturnsAsync(productDto1);

            var result = (ResponseDto)_sut.Put(productDto1).GetAwaiter().GetResult();

            Assert.NotNull(result);
            Assert.True(result.IsSuccess);
        }


        [Fact]
        public void Delete_SendProductDto_ReturnSuccessResponseDto()
        {
            productRepositoryMock.Setup(u => u.Delete(It.IsAny<int>())).ReturnsAsync(true);

            var result = (ResponseDto)_sut.Delete(1).GetAwaiter().GetResult();

            Assert.NotNull(result);
            Assert.True((bool)result.Result);
            Assert.True(result.IsSuccess);
        }
    }
}
