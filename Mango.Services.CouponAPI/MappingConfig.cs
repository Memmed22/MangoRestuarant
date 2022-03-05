using AutoMapper;
using Mango.Services.CouponAPI.Model;
using Mango.Services.CouponAPI.Model.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mango.Services.CouponAPI
{
    public class MappingConfig
    {
        public static MapperConfiguration RegisterMaps() =>
            new MapperConfiguration(config => {
                config.CreateMap<CouponDto, Coupon>().ReverseMap();
            });
    }
}
