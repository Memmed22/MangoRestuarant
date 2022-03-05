using Azure.Messaging.ServiceBus;
using Mango.Services.Email.Messages;
using Mango.Services.Email.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mango.Services.Email.Messaging
{
    public class AzureServiceBusConsumer : IAzureServiceBusConsumer
    {


        private readonly string serviceBusConnectionString;
        private readonly string emailSubscription;
        private readonly string orderUpdatePaymentResultTopic;

        private readonly IConfiguration _config;
        private readonly EmailRepository _emailRepo;
        private readonly ServiceBusProcessor _busProcessor;

        public AzureServiceBusConsumer(IConfiguration config, EmailRepository emailRepo)
        {
            _config = config;
            _emailRepo = emailRepo;
            serviceBusConnectionString = _config.GetValue<string>("ServiceBusConnectionString");
            emailSubscription = _config.GetValue<string>("EmailSubscription");
            orderUpdatePaymentResultTopic = _config.GetValue<string>("OrderUpdatePaymentResultTopic");

            var client = new ServiceBusClient(serviceBusConnectionString);
            _busProcessor = client.CreateProcessor(orderUpdatePaymentResultTopic, emailSubscription);
        }

      

        public async Task Start()
        {
            _busProcessor.ProcessMessageAsync += OnOrderPaymentUpdateReceived;
            _busProcessor.ProcessErrorAsync += ErrorHandler;
            await _busProcessor.StartProcessingAsync();
        }

        public async Task Stop()
        {
            await _busProcessor.StopProcessingAsync();
            await _busProcessor.DisposeAsync();
        }

        private async Task OnOrderPaymentUpdateReceived(ProcessMessageEventArgs args)
        {
            var message = args.Message;
            var body = Encoding.UTF8.GetString(message.Body);

            PaymentUpdateResultMessage objMessage = JsonConvert.DeserializeObject<PaymentUpdateResultMessage>(body.ToString());

            try
            {
                await _emailRepo.SendAndLogEmail(objMessage);
                await args.CompleteMessageAsync(args.Message);

            }
            catch (Exception)
            {

                throw;
            }


        }

        Task ErrorHandler(ProcessErrorEventArgs args)
        {
            Console.WriteLine(args.Exception.Message);
            return Task.CompletedTask;
        }

        //public Task Invoke(HttpContext context, IEmailRepository emailRepo)
        //{
        //    _emailRepo = emailRepo;
        //    throw new NotImplementedException();
        //}
    }

}
