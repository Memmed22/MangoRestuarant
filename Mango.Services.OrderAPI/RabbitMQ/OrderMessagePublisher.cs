using Mango.Services.OrderAPI.Messaging;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mango.Services.OrderAPI.RabbitMQ
{
    public class OrderMessagePublisher : IOrderMessagePublisher
    {
        private readonly string _hostName;
        private readonly string _username;
        private readonly string _password;

        private readonly string _orderMessageExchangeName;
        private readonly string _orderRoutingKey;

        private IConnection _connection;
        private IModel _channel;
        private readonly IConfiguration _config;


        public OrderMessagePublisher(IConfiguration config)
        {
            _config = config;
            _hostName = _config.GetValue<string>("RabbitMQConnection:Hostname");
            _username = _config.GetValue<string>("RabbitMQConnection:Username");
            _password = _config.GetValue<string>("RabbitMQConnection:Password");
            _orderRoutingKey = _config.GetValue<string>("OrderMessageRoutingKey");

            _orderMessageExchangeName = _config.GetValue<string>("OrderMessageExchangeName");

            var factory = new ConnectionFactory() { 
                HostName = _hostName,
                UserName = _username,
                Password = _password
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.ExchangeDeclare(_orderMessageExchangeName, ExchangeType.Direct);
        }

        public void PublishMessage(PaymnetRequestMessage message)
        {
            var json = JsonConvert.SerializeObject(message);
            var body = Encoding.UTF8.GetBytes(json);

            _channel.BasicPublish(_orderMessageExchangeName, _orderRoutingKey, null, body);
        }
    }
}
