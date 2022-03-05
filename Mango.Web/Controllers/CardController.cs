using Mango.Web.Models.Dto;
using Mango.Web.Services.IServices;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mango.Web.Controllers
{
    public class CardController : Controller
    {
        private readonly ICartService _cartService;
        private readonly ICouponService _couponService;
             
        public CardController(ICartService cartService, ICouponService couponService)
        {
            _cartService = cartService;
            _couponService = couponService;
        }

        [Authorize]
        public async Task<IActionResult> Remove(int id)
        {
            string token = await HttpContext.GetTokenAsync("access_token");

            var response = await _cartService.RemoveFromCartAsync<ResponseDto>(id, token);
            if (response != null && response.IsSuccess)
                return RedirectToAction(nameof(Index));
            return View();

        }
        public async Task<IActionResult> Index()
        {
            return View(await LoadCardByUserId());
        }

        [HttpPost]
        [ActionName("ApplyCoupon")]
        public async Task<IActionResult> ApplyCoupon(CartDto cardDto)
        {
            string userId = User.Claims.Where(u => u.Type == "sub")?.FirstOrDefault()?.Value;
            string token = await HttpContext.GetTokenAsync("access_token");
            var response = await _cartService.ApplyCouponAsync<ResponseDto>(cardDto, token);
            if (response != null &&  response.IsSuccess)
            {
                return RedirectToAction(nameof(Index));
            }
            return View() ;
        }

        [HttpPost]
        [ActionName("RemoveCoupon")]
        public async Task<IActionResult> RemoveCoupon(CartDto cartDto)
        {
            string token = await HttpContext.GetTokenAsync("access_token");
            var response = await _cartService.RemoveCouponAsync<ResponseDto>(cartDto.CartHeader.UserId, token);
            if ((bool)(response?.IsSuccess))
            {
                return RedirectToAction(nameof(Index));
            }
            return View();
        }

      
        public async Task<IActionResult> Checkout()
        {
            return View("Checkout",await LoadCardByUserId());
        }

        [HttpPost]
        public async Task<IActionResult> Checkout(CartDto cartDto)
        {
            try
            {
                string token = await HttpContext.GetTokenAsync("access_token");
                var response =await _cartService.Checkout<ResponseDto>(cartDto.CartHeader, token);

                if (!response.IsSuccess)
                {
                    TempData["Error"] = response.ErrorMessage[0];
                    return RedirectToAction(nameof(Checkout));
                }

                return RedirectToAction(nameof(Confirmation));
            }
            catch (Exception)
            {
                return View();
            }
        }

        public IActionResult Confirmation() => View();
        

        public async Task<CartDto> LoadCardByUserId()
        {
            string userId = User.Claims.Where(u => u.Type == "sub")?.FirstOrDefault()?.Value;
            string token = await HttpContext.GetTokenAsync("access_token");

            var response = await _cartService.GetCartByUserIdAsync<ResponseDto>(userId, token);

            CartDto cartDto = new();
            if (response != null && response.IsSuccess) 
                cartDto = JsonConvert.DeserializeObject<CartDto>(Convert.ToString(response.Result));

            if (cartDto.CartHeader != null)
            {
                if (!string.IsNullOrEmpty(cartDto.CartHeader.CouponCode))
                {
                    var couponResponse = await _couponService.GetCoupon<ResponseDto>(cartDto.CartHeader.CouponCode, token);
                    if (couponResponse != null && couponResponse.IsSuccess)
                    {
                        var couponObj = JsonConvert.DeserializeObject<CouponDto>(couponResponse.Result.ToString());
                        cartDto.CartHeader.DiscountTotal = couponObj.Discount;
                    }
                }
                foreach (var item in cartDto.CartDetails)
                {
                    cartDto.CartHeader.OrderTotal += (item.Count * item.Product.Price);
                }
                cartDto.CartHeader.OrderTotal -= cartDto.CartHeader.DiscountTotal;
            }
            return cartDto;

        }

    }
}
