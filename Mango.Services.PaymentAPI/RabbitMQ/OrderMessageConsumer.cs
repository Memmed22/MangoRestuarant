using Mango.Services.PaymentAPI.Messages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using PaymentProcessor;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Mango.Services.PaymentAPI.RabbitMQ
{
    public class OrderMessageConsumer : BackgroundService
    {
        private readonly string _hostName;
        private readonly string _username;
        private readonly string _password;
        private readonly string _orderMessageExchangeName;
        private readonly string _orderMessageQueueName;
        private readonly string _orderMessageRouteKey;
        private IConnection _connection;
        private readonly IConfiguration _config;
        private readonly IModel _channel;
        private readonly IProcessPayment _processPayment;
        private readonly IPaymentMessagePublisher _paymentPublisher;



        public OrderMessageConsumer(IConfiguration config, IProcessPayment processPayment, IPaymentMessagePublisher paymentPublisher)
        {
            _config = config;
            _processPayment = processPayment;
            _paymentPublisher = paymentPublisher;
            _hostName = _config.GetValue<string>("RabbitMQConnection:Hostname");
            _username = _config.GetValue<string>("RabbitMQConnection:Username");
            _password = _config.GetValue<string>("RabbitMQConnection:Password");
            _orderMessageExchangeName = _config.GetValue<string>("OrderMessageExchangeName");
            _orderMessageRouteKey = _config.GetValue<string>("OrderMessageRoutingKey");

            var factory = new ConnectionFactory()
            {
                HostName = _hostName,
                UserName = _username,
                Password = _password
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.ExchangeDeclare(_orderMessageExchangeName, ExchangeType.Direct);
            _orderMessageQueueName = _channel.QueueDeclare().QueueName;
            _channel.QueueBind(queue: _orderMessageQueueName, exchange: _orderMessageExchangeName, routingKey: _orderMessageRouteKey);

        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (sender, e) =>
            {
                var message = Encoding.UTF8.GetString(e.Body.ToArray());
                PaymentRequestMessage paymentRequestMessage = JsonConvert.DeserializeObject<PaymentRequestMessage>(message);
                HandleMessage(paymentRequestMessage).GetAwaiter().GetResult();

                _channel.BasicAck(e.DeliveryTag, false);
            };

            _channel.BasicConsume(_orderMessageQueueName, false, consumer);
            return Task.CompletedTask;
        }

        private async Task HandleMessage(PaymentRequestMessage paymentRequestMessage)
        {
            var paymentResult = _processPayment.PaymnetProcess();

            PaymentUpdateResultMessage paymentRequest = new PaymentUpdateResultMessage()
            {
                Email = paymentRequestMessage.Email,
                MessageCreated = DateTime.Now,
                OrderId = paymentRequestMessage.OrderId,
                Status = paymentResult
            };

            try
            {
                _paymentPublisher.PublishMessage(paymentRequest);
            }
            catch (Exception ex)
            {

                throw;
            }

        }

    }
}
