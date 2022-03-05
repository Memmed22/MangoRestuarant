using Azure.Messaging.ServiceBus;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mango.MessageBus
{
   public class AzureServiceBusMessageBus : IMessageBus
    {
        private string connectionString = "Endpoint=sb://mangorestuarant.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=c278FU5xtXZu5JEM9F42KBjanjkrwSbaukya2hCugaA=";
        private ServiceBusClient client;
        private ServiceBusSender sender;
        
        public async Task PublishMessage(BaseMessage baseMessage, string topicName)
        {
            try
            {

                client = new ServiceBusClient(connectionString);
                
                    sender = client.CreateSender(topicName);

                    var jsonMessage = JsonConvert.SerializeObject(baseMessage);

                    ServiceBusMessage serviceBusMessage = new ServiceBusMessage(Encoding.UTF8.GetBytes(jsonMessage))
                    {
                        CorrelationId = Guid.NewGuid().ToString()
                    };

                    await sender.SendMessageAsync(serviceBusMessage);
                
            }
            catch (Exception ex)
            {

                throw;
            }
            finally {
                await client.DisposeAsync();
                await sender.DisposeAsync();
            }


            
        }
    }
}
