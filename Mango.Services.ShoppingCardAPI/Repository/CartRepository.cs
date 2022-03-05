using AutoMapper;
using Mango.Services.ShoppingCardAPI.DbContexts;
using Mango.Services.ShoppingCardAPI.Models;
using Mango.Services.ShoppingCardAPI.Models.Dto;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mango.Services.ShoppingCardAPI.Repository
{
    public class CartRepository : ICartRepository
    {
        private readonly ApplicationDbContext _db;
        private IMapper _mapper;

        public CartRepository(ApplicationDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task<bool> ApplyCouponAsync(string userId, string couponCode)
        {
            var cardDb = await _db.CartHeader.FirstOrDefaultAsync(u => u.UserId == userId);
            cardDb.CouponCode = couponCode;
            _db.CartHeader.Update(cardDb);
            await _db.SaveChangesAsync();
            return true;
        }
        public async Task<bool> RemoveCouponAsync(string userId)
        {
            var cardDb = await _db.CartHeader.FirstOrDefaultAsync(u => u.UserId == userId);
            cardDb.CouponCode = null;
            _db.CartHeader.Update(cardDb);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ClearCart(string userId)
        {
            var CartHeaderInDb = await _db.CartHeader.FirstOrDefaultAsync(u => u.UserId == userId);
            if (CartHeaderInDb != null)
            {
                _db.CartDetail.RemoveRange(_db.CartDetail.Where(u => u.CartHeaderId == CartHeaderInDb.Id));
                await _db.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<CartDto> CreateUpdateCart(CartDto cartDto)
        {
            try
            {
                Cart cart = _mapper.Map<Cart>(cartDto);

                //check if product exist in database, if not create it.
                var productInDb = await _db.Product.AsNoTracking().FirstOrDefaultAsync(u => u.Id == cartDto.CartDetails.FirstOrDefault().ProductId); //cardDto always has one CardDetails

                if (productInDb == null)
                {
                    _db.Product.Add(cart.CartDetails.FirstOrDefault().Product);
                    await _db.SaveChangesAsync();
                }

                var CartHeaderFromDb = await _db.CartHeader.AsNoTracking().FirstOrDefaultAsync(u=> u.UserId==cart.CartHeader.UserId);

                if (CartHeaderFromDb == null)
                {
                    //Check if header is null
                    //create header and detils
                    _db.CartHeader.Add(cart.CartHeader);
                    await _db.SaveChangesAsync();
                    cart.CartDetails.FirstOrDefault().CartHeaderId = cart.CartHeader.Id;
                    cart.CartDetails.FirstOrDefault().Product = null;
                    _db.CartDetail.Add(cart.CartDetails.FirstOrDefault());
                    await _db.SaveChangesAsync();
                }
                else
                {
                    //if header is not null
                    var cartDetailsDb = await _db.CartDetail.AsNoTracking().FirstOrDefaultAsync(u => u.ProductId == cart.CartDetails.FirstOrDefault().ProductId &&
                                                                                    u.CartHeaderId == CartHeaderFromDb.Id);
                    if (cartDetailsDb == null)
                    {
                        cart.CartDetails.FirstOrDefault().Product = null;
                        cart.CartDetails.FirstOrDefault().CartHeaderId = CartHeaderFromDb.Id;
                        _db.CartDetail.Add(cart.CartDetails.FirstOrDefault());
                        await _db.SaveChangesAsync();
                    }
                    else
                    {
                        //check if details has same product
                        //if has then update the count

                        cart.CartDetails.FirstOrDefault().Count += cartDetailsDb.Count;
                        cart.CartDetails.FirstOrDefault().Product = null;
                        cart.CartDetails.FirstOrDefault().Id = cartDetailsDb.Id;
                        cart.CartDetails.FirstOrDefault().CartHeaderId = cartDetailsDb.CartHeaderId;
                        //cart.CartHeader.Id = CartHeaderFromDb.Id;
                        _db.CartDetail.Update(cart.CartDetails.FirstOrDefault());
                        await _db.SaveChangesAsync();
                    }

                }

                return _mapper.Map<CartDto>(cart);
            }
            catch (Exception ex)
            {
                throw; 
            }


        }

        public async Task<CartDto> GetCartByUserId(string userId)
        {
            Cart cart = new Cart
            {
                CartHeader = await _db.CartHeader.FirstOrDefaultAsync(u => u.UserId == userId)
            };
            cart.CartDetails = _db.CartDetail.Where(u => u.CartHeaderId == cart.CartHeader.Id).Include(u => u.Product);

            return _mapper.Map<CartDto>(cart);
        }

       
        public async Task<bool> RemoveFromCart(int CartDetailsId)
        {
            try
            {
                CartDetails cartDetails = await _db.CartDetail.FirstOrDefaultAsync(u => u.Id == CartDetailsId);
                if (cartDetails == null)
                    return false;

                var headersCount = _db.CartDetail.Where(u => u.CartHeaderId == cartDetails.CartHeaderId).Count();
                _db.CartDetail.Remove(cartDetails);
                if (headersCount == 1)
                {
                    CartHeader cartHeaderToRemove = await _db.CartHeader.FirstOrDefaultAsync(u => u.Id == cartDetails.CartHeaderId);
                    _db.CartHeader.Remove(cartHeaderToRemove);
                }
                await _db.SaveChangesAsync();
                return true;

            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
