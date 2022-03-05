using Mango.Services.OrderAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mango.Services.OrderAPI.Repository
{
   public interface IOrderRepository
    {
        public Task<bool> AddOrder(OrderHeader orderHeader);

        public Task UpdatePaymentStatus(int OrderHeaderId, bool paid);
    }
}
