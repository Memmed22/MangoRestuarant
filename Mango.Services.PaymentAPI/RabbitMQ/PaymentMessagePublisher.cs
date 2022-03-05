using Mango.Services.PaymentAPI.Messages;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mango.Services.PaymentAPI.RabbitMQ
{
    public class PaymentMessagePublisher : IPaymentMessagePublisher
    {
        private readonly string _hostName;
        private readonly string _username;
        private readonly string _password;
        private readonly string _paymentMessageExchangeName;
        private readonly string _paymenMessageRouteKey;
        private IConnection _connection;
        private readonly IConfiguration _config;
        private readonly IModel _channel;

        public PaymentMessagePublisher(IConfiguration config)
        {
            _config = config;

            _hostName = _config.GetValue<string>("RabbitMQConnection:Hostname");
            _username = _config.GetValue<string>("RabbitMQConnection:Username");
            _password = _config.GetValue<string>("RabbitMQConnection:Password");
            _paymentMessageExchangeName = _config.GetValue<string>("PaymentMessageExchangename");
            _paymenMessageRouteKey = _config.GetValue<string>("PaymentMessageRoutingKey");

            var factory = new ConnectionFactory() { 
                HostName = _hostName,
                UserName = _username,
                Password = _password
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.ExchangeDeclare(_paymentMessageExchangeName, ExchangeType.Direct);
            
        }

        public void PublishMessage(PaymentUpdateResultMessage message)
        {
            var json = JsonConvert.SerializeObject(message);
            var body = Encoding.UTF8.GetBytes(json);

            _channel.BasicPublish(_paymentMessageExchangeName, _paymenMessageRouteKey, null, body);

        }
    }
}
