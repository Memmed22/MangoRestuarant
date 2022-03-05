using Mango.MessageBus;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mango.Services.ShoppingCardAPI.RabbitMQ
{
    public class CartMessagePublisher : ICartMessagePublisher
    {

        private readonly string _hostName;
        private readonly string _username;
        private readonly string _password;
        private IConnection _connection;
        private readonly IConfiguration _config;

        public CartMessagePublisher(IConfiguration config)
        {
            _config = config;
            _hostName = _config.GetValue<string>("RabbitMQConnection:Hostname");
            _username = _config.GetValue<string>("RabbitMQConnection:Username");
            _password = _config.GetValue<string>("RabbitMQConnection:Password");
        }


        public void SendMessage(BaseMessage baseMessage, string exchangeName)
        {
            var factory = new ConnectionFactory
            {
                UserName = _username,
                Password = _password,
                HostName = _hostName
            };

           _connection = factory.CreateConnection();
           using var channel = _connection.CreateModel();

           channel.ExchangeDeclare(exchangeName, ExchangeType.Direct);
          // channel.QueueDeclare(queue: "mango-cart-queue", false, false, false, null);
          // channel.QueueBind("mango-cart-queue", exchangeName, "shopping.cart.new");

            var json = JsonConvert.SerializeObject(baseMessage);
            var body = Encoding.UTF8.GetBytes(json);

            channel.BasicPublish(exchangeName, "shopping.cart.new", null, body);
        }
    }
}
