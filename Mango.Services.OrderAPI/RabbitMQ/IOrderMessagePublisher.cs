using Mango.Services.OrderAPI.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mango.Services.OrderAPI.RabbitMQ
{
   public interface IOrderMessagePublisher
    {
        public void PublishMessage(PaymnetRequestMessage message);
    }
}
