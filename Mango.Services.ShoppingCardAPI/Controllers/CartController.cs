using Mango.MessageBus;
using Mango.Services.ShoppingCardAPI.Message;
using Mango.Services.ShoppingCardAPI.Models.Dto;
using Mango.Services.ShoppingCardAPI.RabbitMQ;
using Mango.Services.ShoppingCardAPI.Repository;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mango.Services.ShoppingCardAPI.Controllers
{
    [ApiController]
    [Route("api/cart")]
    public class HttpGet : Controller
    {
        private readonly ICartRepository _cartRepository;
        protected ResponseDto _responseDto;
        protected IMessageBus _messageBus;
        private readonly ICouponRepository _couponRepository;
        private readonly ICartMessagePublisher _cartMessageSender;

        public HttpGet(ICartRepository cartRepository, IMessageBus messageBus, ICouponRepository couponRepository, ICartMessagePublisher cartMessageSender)
        {
            _cartMessageSender = cartMessageSender;
            _cartRepository = cartRepository;
            _responseDto = new ResponseDto();
            _messageBus = messageBus;
            _couponRepository = couponRepository;
        }

        //public IActionResult Index()
        //{
        //    return View();
        //}

        [HttpGet("GetCart/{userId}")]
        public async Task<object> GetCart(string userId)
        {
            try
            {
                CartDto cartDto = await _cartRepository.GetCartByUserId(userId);
                _responseDto.Result = cartDto;
                _responseDto.IsSuccess = true;
            }
            catch (Exception ex)
            {
                _responseDto.IsSuccess = false;
                _responseDto.ErrorMessage = new List<string> { ex.ToString() };
            }
            return _responseDto;
        }

        [HttpPost("AddCart")]
        public async Task<object> AddCart(CartDto cartDto)
        {
            try
            {
                _responseDto.Result = await _cartRepository.CreateUpdateCart(cartDto);
                _responseDto.IsSuccess = true;
            }
            catch (Exception ex)
            {
                _responseDto.IsSuccess = false;
                _responseDto.ErrorMessage = new List<string> { ex.Message.ToString() };
            }
            return _responseDto;
        }

        [HttpPost("UpdateCart")]
        public async Task<object> UpdateCart(CartDto cartDto)
        {
            try
            {
                _responseDto.Result = await _cartRepository.CreateUpdateCart(cartDto);
                _responseDto.IsSuccess = true;
            }
            catch (Exception ex)
            {
                _responseDto.IsSuccess = false;
                _responseDto.ErrorMessage = new List<string> { ex.Message.ToString() };
            }
            return _responseDto;
        }


        [HttpPost("Checkout")]
        public async Task<object> Checkout(CheckOutHeaderDto checkOutHeader)
        {
            try
            {
                var cartDto = await _cartRepository.GetCartByUserId(checkOutHeader.UserId);
                if (cartDto == null)
                {
                    return BadRequest();
                }

                if (!string.IsNullOrEmpty(checkOutHeader.CouponCode))
                {
                    var coupon = await _couponRepository.GetCoupon(checkOutHeader.CouponCode);
                    if (checkOutHeader.DiscountTotal != coupon.Discount)
                    {
                        _responseDto.IsSuccess = false;
                        _responseDto.ErrorMessage = new List<string> { "Coupon price has changed, please confirm" };
                        _responseDto.DisplayMessage = "Coupon pirce has chnged, please confirm";
                        return _responseDto;
                    }
                }

                checkOutHeader.CartDetails = cartDto.CartDetails;

                //logic to add message to prcess order.
                //Burda messageBus'a Queuename'de Topic'de gondere bilerik.
              //  await _messageBus.PublishMessage(checkOutHeader, "checkoutqueue");

                _cartMessageSender.SendMessage(checkOutHeader, "mango-cart-message");

                await _cartRepository.ClearCart(checkOutHeader.UserId);


                _responseDto.IsSuccess = true;
            }
            catch (Exception ex)
            {
                _responseDto.IsSuccess = false;
                _responseDto.ErrorMessage = new List<string> { ex.Message };
            }
            return _responseDto;
        }

        [HttpPost("RemoveCart")]
        public async Task<object> RemoveCart([FromBody] int cartId)
        {
            try
            {
                _responseDto.Result = await _cartRepository.RemoveFromCart(cartId);
                _responseDto.IsSuccess = true;
            }
            catch (Exception ex)
            {
                _responseDto.IsSuccess = false;
                _responseDto.ErrorMessage = new List<string> { ex.Message };
            }
            return _responseDto;
        }

        [HttpPost("ApplyCoupon")]
        public async Task<object> ApplyCoupon([FromBody] CartDto cartDto)
        {
            try
            {
                _responseDto.IsSuccess = await _cartRepository.ApplyCouponAsync(cartDto.CartHeader.UserId, cartDto.CartHeader.CouponCode);
            }
            catch (Exception ex)
            {
                _responseDto.IsSuccess = false;
                _responseDto.ErrorMessage = new List<string> { ex.Message };
            }
            return _responseDto;
        }

        [HttpPost("RemoveCoupon")]
        public async Task<object> RemoveCoupon([FromBody]string userId)
        {
            try
            {
                _responseDto.IsSuccess = await _cartRepository.RemoveCouponAsync(userId);
            }
            catch (Exception ex)
            {
                _responseDto.ErrorMessage = new List<string> { ex.Message };
                _responseDto.IsSuccess = false;
            }
            return _responseDto;
        }
            
    }
}
