using Mango.Web.Services.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Mango.Web.Services
{
    public class CouponSerice : BaseService,ICouponService
    {
        private readonly IHttpClientFactory _clientFactory;
        public CouponSerice(IHttpClientFactory clientFactory) : base(clientFactory)
        {
            _clientFactory = clientFactory;
        }
        public async Task<T> GetCoupon<T>(string couponCode, string token = null)
        {
            return await this.SendAsync<T>(new Models.ApiRequest
            {
                AccessToken = token,
                ApiType = SD.ApiType.GET,
                Url = SD.CouponAPIBase + "/api/couponAPI/"+couponCode,
                Data = couponCode
            }) ;
        }
    }
}
