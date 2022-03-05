using Mango.Services.CouponAPI.Model.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mango.Services.CouponAPI.Repository
{
   public interface ICouponRepository
    {
        Task<CouponDto> GetCouponByCodeAsync(string couponCode);
    }
}
