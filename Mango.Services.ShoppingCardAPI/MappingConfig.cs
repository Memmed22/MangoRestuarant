using AutoMapper;
using Mango.Services.ShoppingCardAPI.Models;
using Mango.Services.ShoppingCardAPI.Models.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mango.Services.ShoppingCardAPI
{
    public class MappingConfig
    {
        public static MapperConfiguration RegisterMap() => new MapperConfiguration(configure => {
            configure.CreateMap<Product, ProductDto>().ReverseMap();
            configure.CreateMap<CartHeader, CartHeaderDto>().ReverseMap();
            configure.CreateMap<CartDetails, CartDetailsDto>().ReverseMap(); ;
            configure.CreateMap<Cart, CartDto>().ReverseMap();
            });
            
        
    }
}
