using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Mango.Web.Models.Dto;
using Mango.Web.Services.IServices;

namespace Mango.Web.Services
{
    public class CartService :BaseService, ICartService
    {

        private IHttpClientFactory _httpClientFactory;

        public CartService(IHttpClientFactory httpClientFactory) : base(httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<T> AddCartAsync<T>(CartDto cartDto, string token = null) =>
             await this.SendAsync<T>(new Models.ApiRequest {
                AccessToken = token,
                ApiType = SD.ApiType.POST,
                Url = SD.ShoppingCartAPIBase + "/api/cart/AddCart",
                Data = cartDto
            }) ;

        public async Task<T> GetCartByUserIdAsync<T>(string userId, string token = null)
       =>
          await this.SendAsync<T>(new Models.ApiRequest
          {
              ApiType = SD.ApiType.GET,
              AccessToken = token,
              Url = SD.ShoppingCartAPIBase + "/api/cart/GetCart/" + userId,
              Data = null
          });

        public async Task<T> RemoveFromCartAsync<T>(int CartDetailId, string token = null) =>
         await this.SendAsync<T>(new Models.ApiRequest
         {
             ApiType = SD.ApiType.POST,
             AccessToken = token,
             Data = CartDetailId,
             Url = SD.ShoppingCartAPIBase + "/api/cart/RemoveCart"
         });

        public Task<T> Checkout<T>(CartHeaderDto cartHeaderDto, string token = null)
        {
            return this.SendAsync<T>(new Models.ApiRequest
            {
                AccessToken = token,
                ApiType = SD.ApiType.POST,
                Data = cartHeaderDto,
                Url = SD.ShoppingCartAPIBase + "/api/cart/Checkout"
            });
        }

        public async Task<T> ApplyCouponAsync<T>(CartDto cardDto, string token = null)=>
            await this.SendAsync<T>(new Models.ApiRequest { 
                AccessToken = token,
                ApiType = SD.ApiType.POST,
                Url = SD.ShoppingCartAPIBase + "/api/cart/ApplyCoupon",
                Data = cardDto
            });

        public async Task<T> UpdateCartAsyn<T>(CartDto cartDto, string token = null) =>
           await this.SendAsync<T>(new Models.ApiRequest
           {
               ApiType = SD.ApiType.POST,
               AccessToken = token,
               Data = cartDto,
               Url = SD.ShoppingCartAPIBase + "/api/cart/UpdateCart"
           });


        public async Task<T> RemoveCouponAsync<T>(string userId, string token = null) =>
             await this.SendAsync<T>(new Models.ApiRequest
            {
                AccessToken = token,
                ApiType = SD.ApiType.POST,
                Data = userId,
                Url = SD.ShoppingCartAPIBase + "/api/cart/RemoveCoupon"
            });
       
        
    }
}
