using Mango.Services.Email.Messages;
using Mango.Services.Email.Repository;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Mango.Services.Email.RabbitMQ
{
    public class PaymentMessageConsumer : BackgroundService
    {

        private readonly string _hostName;
        private readonly string _username;
        private readonly string _password;
        private readonly string _paymentMessageExchangeName;
        private readonly string _paymentMessageQueueName;
        private readonly string _paymentMessageRoutingKey;
        private IConnection _connection;
        private readonly IConfiguration _config;
        private readonly IModel _channel;
        private readonly EmailRepository _emailRepo;

        public PaymentMessageConsumer(IConfiguration config, EmailRepository emailRepo)
        {
            _config = config;
            _emailRepo = emailRepo;
            _hostName = _config.GetValue<string>("RabbitMQConnection:Hostname");
            _username = _config.GetValue<string>("RabbitMQConnection:Username");
            _password = _config.GetValue<string>("RabbitMQConnection:Password");
            _paymentMessageExchangeName = _config.GetValue<string>("PaymentMessageExchangeName");
            _paymentMessageRoutingKey = _config.GetValue<string>("PaymentMessageRoutingKey");

            var factory = new ConnectionFactory
            {
                HostName = _hostName,
                UserName = _username,
                Password = _password
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.ExchangeDeclare(_paymentMessageExchangeName, ExchangeType.Direct);
            _paymentMessageQueueName = _channel.QueueDeclare().QueueName;
            _channel.QueueBind(_paymentMessageQueueName, exchange: _paymentMessageExchangeName, routingKey: _paymentMessageRoutingKey);

        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (sender, e) =>
            {
                var body = Encoding.UTF8.GetString(e.Body.ToArray());
                PaymentUpdateResultMessage message = JsonConvert.DeserializeObject<PaymentUpdateResultMessage>(body);
                HandleMessage(message).GetAwaiter().GetResult();

                _channel.BasicAck(e.DeliveryTag, false);

            };
            _channel.BasicConsume(_paymentMessageQueueName, false, consumer);
            return Task.CompletedTask;
        }

        private async Task HandleMessage(PaymentUpdateResultMessage message)
        {
            try
            {
               await _emailRepo.SendAndLogEmail(message);
            }
            catch (Exception ex)
            {

                throw;
            }
        }
    }
}
