using Mango.MessageBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mango.Services.ShoppingCardAPI.RabbitMQ
{
   public interface ICartMessagePublisher
    {
        public void SendMessage(BaseMessage baseMessage, string exchangeName);
    }
}
