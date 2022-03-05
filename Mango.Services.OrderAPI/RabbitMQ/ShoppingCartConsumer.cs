using Mango.Services.OrderAPI.Messaging;
using Mango.Services.OrderAPI.Models;
using Mango.Services.OrderAPI.Repository;
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

namespace Mango.Services.OrderAPI.RabbitMQ
{
    public class ShoppingCartConsumer : BackgroundService
    {
        private readonly string _hostName;
        private readonly string _username;
        private readonly string _password;
        private readonly string _shoppingCartExchangeName;
        private readonly string _shoppingCartQueueName;
        private readonly string _shoppingCartRouteKey;
        private IConnection _connection;
        private readonly IConfiguration _config;
        private readonly IModel _channel;
        private readonly OrderRepository _orderRepo;
        private readonly OrderMessagePublisher _messagePublisher;


        public ShoppingCartConsumer(IConfiguration config, OrderRepository orderRepo, OrderMessagePublisher messagePublisher)
        {
            _orderRepo = orderRepo;
            _config = config;
            _messagePublisher = messagePublisher;
            _hostName = _config.GetValue<string>("RabbitMQConnection:Hostname");
            _username = _config.GetValue<string>("RabbitMQConnection:Username");
            _password = _config.GetValue<string>("RabbitMQConnection:Password");
            _shoppingCartExchangeName = _config.GetValue<string>("ShoppingCartExchangeName");
            _shoppingCartRouteKey = _config.GetValue<string>("ShoppingCartRouteKey");

            var factory = new ConnectionFactory {
                HostName = _hostName,
                UserName = _username,
                Password = _password
            };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.ExchangeDeclare(_shoppingCartExchangeName, ExchangeType.Direct);

            _shoppingCartQueueName = _channel.QueueDeclare().QueueName;
            //_shoppingCartQueueName = "mango-cart-queue";
            //_channel.QueueDeclare(_shoppingCartQueueName, false, false, false, null);
            _channel.QueueBind(_shoppingCartQueueName, _shoppingCartExchangeName, routingKey: _shoppingCartRouteKey);
        }


        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (sender, e) => {
                var message = Encoding.UTF8.GetString(e.Body.ToArray());
                CheckOutHeaderDto headerDto = JsonConvert.DeserializeObject<CheckOutHeaderDto>(message);
                HandleMessage(headerDto).GetAwaiter().GetResult();

                //lets to know to deleiver every message, and that message will be discarded.
                _channel.BasicAck(e.DeliveryTag, false);
            };

            _channel.BasicConsume(_shoppingCartQueueName, false, consumer);
           return  Task.CompletedTask;
        }

        private async Task HandleMessage(CheckOutHeaderDto checkOutHeaderDto)
        {
            OrderHeader orderHeader = new OrderHeader
            {
                UserId = checkOutHeaderDto.UserId,
                FirstName = checkOutHeaderDto.FirstName,
                LastName = checkOutHeaderDto.LastName,
                OrderDetails = new List<OrderDetails>(),
                CardNumber = checkOutHeaderDto.CardNumber,
                CouponCode = checkOutHeaderDto.CouponCode,
                CVV = checkOutHeaderDto.CVV,
                DiscountTotal = checkOutHeaderDto.DiscountTotal,
                Email = checkOutHeaderDto.Email,
                ExpiryMonthYear = checkOutHeaderDto.ExpiryMonthYear,
                OrderTime = DateTime.Now,
                OrderTotal = checkOutHeaderDto.OrderTotal,
                PaymentStatus = false,
                Phone = checkOutHeaderDto.Phone,
                PickUpDateTime = checkOutHeaderDto.PickUpDateTime
            };

            foreach (var item in checkOutHeaderDto.CartDetails)
            {
                OrderDetails orderDetails = new OrderDetails
                {
                    ProductId = item.ProductId,
                    ProductName = item.Product.Name,
                    Price = item.Product.Price,
                    Count = item.Count
                };
                orderHeader.CartTotalItems +=item.Count;
                orderHeader.OrderDetails.Add(orderDetails);
            }

            //EF core will add OrderDetails automatically
            await _orderRepo.AddOrder(orderHeader);


            PaymnetRequestMessage paymentRequestMessage = new PaymnetRequestMessage
            {
                CardNumber = orderHeader.CardNumber,
                CVV = orderHeader.CVV,
                Email = orderHeader.Email,
                ExpiryDate = orderHeader.ExpiryMonthYear,
                Name = $"{orderHeader.FirstName} { orderHeader.LastName}",
                OrderId= orderHeader.Id,
                OrderTotal = orderHeader.OrderTotal
            };

            try
            {
                _messagePublisher.PublishMessage(paymentRequestMessage);
            }
            catch (Exception ex)
            {

                throw;
            }
        }
    }
}
