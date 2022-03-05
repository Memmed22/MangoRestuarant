using Azure.Messaging.ServiceBus;
using Mango.MessageBus;
using Mango.Services.OrderAPI.Models;
using Mango.Services.OrderAPI.Repository;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mango.Services.OrderAPI.Messaging
{
    public class AzureServiceBusConsumer : IAzureServiceBusConsumer
    {
        private readonly string serviceBusConnectionString;
        private readonly string checkOutMessageTopic;
        private readonly string checkOutSubscription;
        private readonly string orderPaymentProcessTopic;
        private readonly string orderUpdatePaymentResultTopic;

        private readonly OrderRepository _orderRepository;
        private readonly IConfiguration _configuration;
        private readonly ServiceBusProcessor checkOutBusProcessor;
        private readonly ServiceBusProcessor orderUpdatePaymentProcessor;
        private readonly IMessageBus _messageBus;
        


        public AzureServiceBusConsumer(OrderRepository orderRepository, IConfiguration configuration, IMessageBus messageBus)
        {
            _orderRepository = orderRepository;
            _configuration = configuration;
            _messageBus = messageBus;

            serviceBusConnectionString = _configuration.GetValue<string>("ServiceBusConnectionString");
            checkOutMessageTopic = _configuration.GetValue<string>("CheckOutMessageTopic");
            checkOutSubscription = _configuration.GetValue<string>("CheckOutSubscription");
            orderPaymentProcessTopic = _configuration.GetValue<string>("OrderPaymentProcessTopic");
            orderUpdatePaymentResultTopic = _configuration.GetValue<string>("OrderUpdatePaymentResult");

            var client = new ServiceBusClient(serviceBusConnectionString);

            checkOutBusProcessor = client.CreateProcessor(checkOutMessageTopic, checkOutSubscription);
            orderUpdatePaymentProcessor = client.CreateProcessor(orderUpdatePaymentResultTopic, checkOutSubscription);
        }

        public async Task Start()
        {
            checkOutBusProcessor.ProcessMessageAsync += OnCheckOutMessageReceived;
            checkOutBusProcessor.ProcessErrorAsync += ErrorHandler;
            await checkOutBusProcessor.StartProcessingAsync();

            orderUpdatePaymentProcessor.ProcessMessageAsync += OnOrderPaymentUpdateReceived;
            orderUpdatePaymentProcessor.ProcessErrorAsync += ErrorHandler;
            await orderUpdatePaymentProcessor.StartProcessingAsync();
        }

        public async Task Stop()
        {
            await checkOutBusProcessor.StopProcessingAsync();
            await checkOutBusProcessor.DisposeAsync();

            await orderUpdatePaymentProcessor.StopProcessingAsync();
            await orderUpdatePaymentProcessor.DisposeAsync();
        }

        Task ErrorHandler(ProcessErrorEventArgs args)
        {
            Console.WriteLine(args.Exception.ToString());
            return Task.CompletedTask;
        }

        private async Task OnOrderPaymentUpdateReceived(ProcessMessageEventArgs args) 
        {
            var message = args.Message;
            var body = Encoding.UTF8.GetString(message.Body);

            PaymentUpdateResultMessage paymentResultMessage = JsonConvert.DeserializeObject<PaymentUpdateResultMessage>(body.ToString());

            await _orderRepository.UpdatePaymentStatus(paymentResultMessage.OrderId, paymentResultMessage.Status);
            await args.CompleteMessageAsync(args.Message);
        }
        private async Task OnCheckOutMessageReceived(ProcessMessageEventArgs args)
        {
            var message = args.Message;
            var body = Encoding.UTF8.GetString(message.Body);

            CheckOutHeaderDto checkOutHeaderDto = JsonConvert.DeserializeObject<CheckOutHeaderDto>(body);

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
                OrderDetails orderDetails = new OrderDetails()
                {
                    ProductId = item.ProductId,
                    ProductName = item.Product.Name,
                    Price = item.Product.Price,
                    Count = item.Count
                };

                orderHeader.CartTotalItems += item.Count;
                orderHeader.OrderDetails.Add(orderDetails);
            }

            //EF Core will automatically add orderDeatail to appropriate table. 
            await _orderRepository.AddOrder(orderHeader);


            PaymnetRequestMessage paymnetRequestMessage = new()
            {
                CardNumber = orderHeader.CardNumber,
                CVV = orderHeader.CVV,
                ExpiryDate = orderHeader.ExpiryMonthYear,
                Name = $"{orderHeader.FirstName} {orderHeader.LastName}",
                OrderId = orderHeader.Id,
                OrderTotal = orderHeader.OrderTotal,
                Email = orderHeader.Email
            };

            try
            {
                await _messageBus.PublishMessage(paymnetRequestMessage, orderPaymentProcessTopic);
                await args.CompleteMessageAsync(args.Message);
            }
            catch (Exception ex)
            {

                throw;
            }

        }
    }
}
