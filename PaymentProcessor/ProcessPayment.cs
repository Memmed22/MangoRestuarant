using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentProcessor
{
    public class ProcessPayment : IProcessPayment
    {
        public bool PaymnetProcess()
        {
            //implement custom logic and get card details etc
            return true;
        }
    }
}
