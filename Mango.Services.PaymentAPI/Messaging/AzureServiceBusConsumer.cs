using Azure.Messaging.ServiceBus;
using Mango.MessageBus;
using Mango.Services.PaymentAPI.Messages;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using PaymentProcessor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mango.Services.PaymentAPI.Messaging
{
    public class AzureServiceBusConsumer : IAzureServiceBusConsumer
    {
        private readonly string serviceBusConnectionString;
        private readonly string orderPaymentProcessTopic;
        private readonly string orderPaymentProcessSubscription;
        private readonly string orderUpdatePaymentResultTopic;

        private readonly IConfiguration _configuration;
        private readonly ServiceBusProcessor _busProcessor;
        private readonly IMessageBus _messageBus;
        private readonly IProcessPayment _processPayment;

        public AzureServiceBusConsumer(IConfiguration configuration,IMessageBus messageBus,IProcessPayment processPayment)
        {
            _configuration = configuration;
            _messageBus = messageBus;
            _processPayment = processPayment;

            serviceBusConnectionString = _configuration.GetValue<string>("ServiceBusConnectionString");
            orderPaymentProcessTopic = _configuration.GetValue<string>("OrderPaymentProcessTopic");
            orderPaymentProcessSubscription = _configuration.GetValue<string>("OrderPaymentProcessSubscription");
            orderUpdatePaymentResultTopic = _configuration.GetValue<string>("OrderUpdatePaymentResultTopic");

            var client = new ServiceBusClient(connectionString: serviceBusConnectionString);

            _busProcessor = client.CreateProcessor(orderPaymentProcessTopic, orderPaymentProcessSubscription);
        }



        public async Task Start()
        {
            _busProcessor.ProcessMessageAsync += OnProcessPaymentMessageReceived;
            _busProcessor.ProcessErrorAsync += ErrorHandler;
            await _busProcessor.StartProcessingAsync();
        }

        public async Task Stop()
        {
            await _busProcessor.StopProcessingAsync();
            await _busProcessor.DisposeAsync();
        }

        Task ErrorHandler(ProcessErrorEventArgs args)
        {
            Console.WriteLine(args.Exception.Message);
            return Task.CompletedTask;
        }

        private async Task OnProcessPaymentMessageReceived(ProcessMessageEventArgs args)
        {
            var message = args.Message;
            var body = Encoding.UTF8.GetString(message.Body);

            PaymentRequestMessage requestMessage = JsonConvert.DeserializeObject<PaymentRequestMessage>(body.ToString());
            //For now return only tru, it can be implement base requirements.
            var result = _processPayment.PaymnetProcess();
            PaymentUpdateResultMessage updateResultMessage = new()
            {
                OrderId = requestMessage.OrderId,
                Status = result,
                Email = requestMessage.Email
            };


            try
            {
                await _messageBus.PublishMessage(updateResultMessage, orderUpdatePaymentResultTopic);
                await args.CompleteMessageAsync(args.Message);
            }
            catch (Exception ex)
            {

                throw;
            }
        }

    }
}
