using AutoMapper;
using Mango.Services.ProductAPI.DbContexts;
using Mango.Services.ProductAPI.Model;
using Mango.Services.ProductAPI.Model.Dto;
using Mango.Services.ProductAPI.Repository;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Xunit;

namespace Mango.Services.ProductAPI.XUnit
{
    public class ProductRepositoryTest
    {
        DbContextOptions<ApplicationDbContext> options;
        private static IMapper _mapper;
        private ProductRepository _sut;

        private Product product1;
        private Product product2;

        ApplicationDbContext _context;
        public ProductRepositoryTest()
        {
            if (_mapper == null)
            {
                IMapper mapper = MappingConfig.RegisterMaps().CreateMapper();
                _mapper = mapper;
            }

            options = new DbContextOptionsBuilder<ApplicationDbContext>().
                UseInMemoryDatabase<ApplicationDbContext>(databaseName:"temp_MangoAPI").Options;

            _context = new(options);

            product1 = new Product { 
                Id = 1,
                CategoryName = "cat1",
                Description = "desc1",
                ImageUrl = "Url1",
                Name = "name1",
                Price = 12
            };

            product2 = new Product
            {
                Id = 2,
                CategoryName = "cat2",
                Description = "desc2",
                ImageUrl = "Url2",
                Name = "name2",
                Price = 13
            };

            _context.Database.EnsureDeleted();
            _context.Product.Add(product1);
            _context.Product.Add(product2);
            _context.SaveChanges();

            _sut = new ProductRepository(_context, _mapper);
        }

        [Fact]
        public void GetProducts_SaveTwoMockRecord_ReturnAllSavedRecord()
        {
            //Act
            using (var context = new ApplicationDbContext(options))
            {
                context.Database.EnsureDeleted();
                context.Product.Add(product1);
                context.Product.Add(product2);
                context.SaveChanges();
            }

            List<ProductDto> resultProductDto = null;
            using (var context = new ApplicationDbContext(options))
            {
                var repo = new ProductRepository(context, _mapper);
                resultProductDto =  repo.GetProducts().GetAwaiter().GetResult().ToList();
            }
            //Assert
            Assert.Equal(product1.Name, resultProductDto.First().Name);
            Assert.Equal(product1.Price, resultProductDto.First().Price);
            Assert.Equal(product2.Id, resultProductDto.FirstOrDefault(u => u.Id == product2.Id).Id);
        }

       [Fact]
       public void GetProducts_WithNoDataInDb_ReturnNotNullEmptyObject()
        {
            //Act
            List<ProductDto> resultProductDto = null;
            using (var context = new ApplicationDbContext(options))
            {
                context.Database.EnsureDeleted();
                var repo = new ProductRepository(context, _mapper);
                resultProductDto = repo.GetProducts().GetAwaiter().GetResult().ToList();
            }

            //Assert
            Assert.NotNull(resultProductDto);
            Assert.Empty(resultProductDto);
        }

        [Fact]
        public void GetProductById_SendInexistentlId_ReturnAppropriateProdcut()
        {
            //Act
            using (var context = new ApplicationDbContext(options))
            {
                context.Database.EnsureDeleted();
                context.Product.Add(product1);
                context.Product.Add(product2);
                context.SaveChanges();
            }

            ProductDto resultProductDto = null;
            using (var context = new ApplicationDbContext(options))
            {
                var repo = new ProductRepository(context, _mapper);
                resultProductDto = repo.GetProductById(product1.Id).GetAwaiter().GetResult();
            }

            //Assert
            Assert.Equal(product1.Id, resultProductDto.Id);
            Assert.Equal(product1.Name, resultProductDto.Name);
        }

        [Fact]
        public void GetProductById_SendInexistentId_ReturnNull()
        {
           
            //Act
            ProductDto resultProductDto = null;
            using (var context = new ApplicationDbContext(options))
            {
                context.Database.EnsureDeleted();
                var repo = new ProductRepository(context, _mapper);
                resultProductDto = repo.GetProductById(1001).GetAwaiter().GetResult();
            }

            //Assert
            Assert.Null(resultProductDto);
        }

        [Fact]
        public void Delete_SendAvailbleProductId_ReturnTrueForDelete()
        {
            var repo = new ProductRepository(_context, _mapper);
            var result = repo.Delete(product1.Id).GetAwaiter().GetResult();

            Assert.True(result);
        }

        [Fact]
        public void Delete_SendInexistentProductId_ReturnFalseForDelete()
        {
            var repo = new ProductRepository(_context, _mapper);
            var result = repo.Delete(It.IsAny<int>()).GetAwaiter().GetResult();

            Assert.False(result);
        }

        [Fact]
        public void CreateUpdate_SendInexistentProductToCreateInDb_ReturnCreatedProduct()
        {
            ProductDto productDto = new() { 
                CategoryName = "cat3",
                Description = "desc3",
                ImageUrl = "Url3",
                Name = "Name3",
                Price = 33
            };

            var result = _sut.CreateUpdate(productDto).GetAwaiter().GetResult();

            Assert.NotNull(result);
            Assert.Equal(3, result.Id);
            Assert.Equal("Name3", result.Name);
        }

        [Fact]
        public void CreateUpdate_SendAvailbleProductForUpadte_ReturnUpdatedProduct()
        {
            ProductDto productDto = new()
            {
                Id = 1,
                CategoryName = "cat1",
                Description = "desc1",
                ImageUrl = "Url1",
                Name = "name3",
                Price = 12
            };

            ProductDto resultProductDto = null;
            using (var context = new ApplicationDbContext(options))
            {
                context.Database.EnsureDeleted();
                context.Product.Add(product1);
                context.Product.Add(product2);
                context.SaveChanges();

              
            }

            using (var context = new ApplicationDbContext(options))
            {
                var repo = new ProductRepository(context, _mapper);
                resultProductDto = repo.CreateUpdate(productDto).GetAwaiter().GetResult();
            }

            Assert.Equal(productDto.Name, resultProductDto.Name);
        }

    }

    public class CompareProductList : IEqualityComparer<Product>
    {
        //public new bool Equals(object x, object y)
        //{
        //    Product product = (Product)x;
        //    Product productDto = (Product)y;

        //    if(product.Id == productDto.Id)
        //        return true;
        //    return false;          
        //}

        //public int GetHashCode(object obj)
        //{
        //  return  obj.GetHashCode();
        //}
        public bool Equals(Product x, Product y)
        {
            throw new NotImplementedException();
        }

        public int GetHashCode([DisallowNull] Product obj)
        {
            throw new NotImplementedException();
        }
    }
}
