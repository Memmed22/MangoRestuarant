using Mango.Services.ShoppingCardAPI.Models.Dto;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Mango.Services.ShoppingCardAPI.Repository
{
    public class CouponRepository : ICouponRepository
    {

        private readonly HttpClient _client;
        public CouponRepository(HttpClient client)
        {
            _client = client;
        }
        public async Task<CouponDto> GetCoupon(string couponName)
        {
            var response = await _client.GetAsync($"api/couponAPI/{couponName}");
            var apiContent = await response.Content.ReadAsStringAsync();
            var resp = JsonConvert.DeserializeObject<ResponseDto>(apiContent);
            if (resp.IsSuccess)
            {
                return JsonConvert.DeserializeObject<CouponDto>(resp.Result.ToString());
            }

            return new CouponDto();

        }
    }
}
