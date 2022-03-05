using Mango.Services.ShoppingCardAPI.Models.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mango.Services.ShoppingCardAPI.Repository
{
   public interface ICouponRepository
    {
        Task<CouponDto> GetCoupon(string couponName);
    }
}
