using Mango.Services.ShoppingCardAPI.Models.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mango.Services.ShoppingCardAPI.Repository
{
    public interface ICartRepository
    {
        Task<CartDto> GetCartByUserId(string userId);
        Task<CartDto> CreateUpdateCart(CartDto cartDto);
        Task<bool> RemoveFromCart(int CartDetailsId);

        Task<bool> ApplyCouponAsync(string userId, string couponCode);
        Task<bool> RemoveCouponAsync(string userId);
        Task<bool> ClearCart(string userId);
    }
}
