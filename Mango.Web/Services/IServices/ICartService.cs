using Mango.Web.Models.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mango.Web.Services.IServices
{
    public interface ICartService
    {
         Task<T> GetCartByUserIdAsync<T>(string userId, string token = null);
        Task<T> AddCartAsync<T>(CartDto cartDto, string token = null);
        Task<T> UpdateCartAsyn<T>(CartDto cartDto, string token = null);
        Task<T> RemoveFromCartAsync<T>(int CartDetailId, string token = null);
        Task<T> ApplyCouponAsync<T>(CartDto cardDto, string token = null);
        Task<T> RemoveCouponAsync<T>(string userId, string token = null);
        Task<T> Checkout<T>(CartHeaderDto cartHeaderDto, string token = null);
    }
}
