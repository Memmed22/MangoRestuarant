using Mango.Services.PaymentAPI.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mango.Services.PaymentAPI.RabbitMQ
{
    public interface IPaymentMessagePublisher
    {
        public void PublishMessage(PaymentUpdateResultMessage message);
    }
}
