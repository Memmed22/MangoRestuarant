using Mango.Services.OrderAPI.DbContexts;
using Mango.Services.OrderAPI.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mango.Services.OrderAPI.Repository
{
    public class OrderRepository : IOrderRepository
    {
        //Just one differnet way to use DbCOntext, nothing special here
        private readonly DbContextOptions<ApplicationDbContext> _contextOptions;

        public OrderRepository(DbContextOptions<ApplicationDbContext> contextOptions)
        {
            _contextOptions = contextOptions;
        }
        public async Task<bool> AddOrder(OrderHeader orderHeader)
        {
            await using var _db = new ApplicationDbContext(_contextOptions);
            _db.OrderHeader.Add(orderHeader);
            await _db.SaveChangesAsync();
            //if there is any error possible to return "false"
            return true;
        }

        public async Task UpdatePaymentStatus(int OrderHeaderId, bool paid)
        {
            await using var _db = new ApplicationDbContext(_contextOptions);
            var OrderHeader = await _db.OrderHeader.FirstOrDefaultAsync(u => u.Id == OrderHeaderId);
            if (OrderHeader != null)
            {
                OrderHeader.PaymentStatus = paid;
                await _db.SaveChangesAsync();
            }    
        }
    }
}
