using Mango.Services.CouponAPI.Model.Dto;
using Mango.Services.CouponAPI.Repository;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mango.Services.CouponAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CouponAPIController : Controller
    {

        protected ResponseDto _responseDto;
        private readonly ICouponRepository _couponRepository;

        public CouponAPIController(ICouponRepository couponRepository)
        {
            _responseDto = new ResponseDto();
            _couponRepository = couponRepository;
        }


        [HttpGet]
        [Route("{code}")]
        public async Task<object> Get(string code)
        {
            try
            {
                _responseDto.Result = await _couponRepository.GetCouponByCodeAsync(code);
                _responseDto.IsSuccess = true;
            }
            catch (Exception ex)
            {
                _responseDto.IsSuccess = false;
                _responseDto.ErrorMessage = new List<string> { ex.Message };
            }
            return _responseDto;
        }
            
    }
}
