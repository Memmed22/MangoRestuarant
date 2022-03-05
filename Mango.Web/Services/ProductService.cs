﻿using Mango.Web.Models.Dto;
using Mango.Web.Services.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Mango.Web.Services
{
    public class ProductService : BaseService, IProductService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public ProductService(IHttpClientFactory httpClientFactory): base(httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<T> CreateProductAsync<T>(ProductDto productDto, string token)
        {
            return await this.SendAsync<T>(new Models.ApiRequest()
            {
                ApiType = SD.ApiType.POST,
                Data = productDto,
                Url = SD.ProductAPIBase + "/api/ProductAPI",
                AccessToken = token
            }) ;
        }

        public async Task<T> DeleteProductAsync<T>(int id, string token)
        {
            return await this.SendAsync<T>(new Models.ApiRequest()
            {
                ApiType = SD.ApiType.DELETE,
                Url = SD.ProductAPIBase + "/api/ProductAPI/" + id,
                AccessToken = token
            }) ;
        }

        public async Task<T> GetAllProductsAsync<T>(string token)
        {
            return await this.SendAsync<T>(new Models.ApiRequest
            {
                ApiType = SD.ApiType.GET,
                Url = SD.ProductAPIBase + "/api/ProductAPI",
                AccessToken =token

            });
        }

        public async Task<T> GetProductByIdAsync<T>(int id, string token)
        {
            return await SendAsync<T>(new Models.ApiRequest()
            {
                ApiType = SD.ApiType.GET,
                Url = SD.ProductAPIBase + "/api/ProductAPI/" + id,
                AccessToken = token
            }) ;
        }

        public async Task<T> UpdateProductAsync<T>(ProductDto productDto, string token)
        {
            return await SendAsync<T>(new Models.ApiRequest() { 
                ApiType = SD.ApiType.PUT,
                Data = productDto,
                Url = SD.ProductAPIBase +  "/api/ProductAPI",
                AccessToken = token
            });
        }
    }
}
