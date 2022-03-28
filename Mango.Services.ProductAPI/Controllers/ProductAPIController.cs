using Mango.Services.ProductAPI.Model.Dto;
using Mango.Services.ProductAPI.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mango.Services.ProductAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductAPIController : ControllerBase
    {
        private readonly IProductRepository _productRepository;
        protected ResponseDto _responseDto;

        public ProductAPIController(IProductRepository productRepository)
        {
            _productRepository = productRepository;
            _responseDto = new ResponseDto();
        }

        [HttpGet]
        public async Task<object> Get()
        {
            try
            {
                IEnumerable<ProductDto> productDtoList = await _productRepository.GetProducts();
                _responseDto.Result = productDtoList;
                _responseDto.IsSuccess = true;
            }
            catch (Exception ex)
            { 
                _responseDto.IsSuccess = false;
                _responseDto.ErrorMessage = new List<string>()
                {
                    ex.Message
            };

            }
            return _responseDto;
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<object> Get(int id)
        {
            try
            {
               
                _responseDto.Result = await _productRepository.GetProductById(id);
            }
            catch (Exception ex)
            {
                _responseDto.ErrorMessage = new List<string>() { ex.Message };
                _responseDto.IsSuccess = false;

            }
            return _responseDto;
        }


        [HttpPost]
        public async Task<object> Post([FromBody] ProductDto productDto)
        {
            try
            {
                _responseDto.Result = await _productRepository.CreateUpdate(productDto);
            }
            catch (Exception ex)
            {
                _responseDto.ErrorMessage = new List<string>() { ex.Message };
                _responseDto.IsSuccess = false ;
            }
            return _responseDto;
        }

        [HttpPut]
        public async Task<object> Put([FromBody] ProductDto productDto)
        {
            try
            {
                _responseDto.Result = await _productRepository.CreateUpdate(productDto);
            }
            catch (Exception ex)
            {
                _responseDto.ErrorMessage = new List<string>() { ex.Message };
                _responseDto.IsSuccess = false;
            }
            return _responseDto;
        }

        [HttpDelete]
        [Route("{id}")]
        [Authorize(Roles ="Admin")]
        public async Task<object> Delete(int id)
        {
            //var user = User.Claims.ToList();
        
            try
            {
                _responseDto.Result = await _productRepository.Delete(id);
            }
            catch (Exception ex)
            {
                _responseDto.ErrorMessage = new List<string>() { ex.Message };
                _responseDto.IsSuccess = false;
            }
            return _responseDto;
        }

    }
}

