using Mango.Services.OrderAPI.Messaging;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mango.Services.OrderAPI.Extension
{
    public static class ApplicationBuilderExtension
    {
        public static IAzureServiceBusConsumer  serviceBusConsumer {get;set;}

        public static IApplicationBuilder UseAzureServiceBusConsumer(this IApplicationBuilder app)
        {
            serviceBusConsumer = (IAzureServiceBusConsumer)app.ApplicationServices.GetService(typeof(IAzureServiceBusConsumer));
            var hostApplicationLife = (IHostApplicationLifetime)app.ApplicationServices.GetService(typeof(IHostApplicationLifetime));

            hostApplicationLife.ApplicationStarted.Register(OnStart);
            hostApplicationLife.ApplicationStopped.Register(OnStop);

            return app;
        }

        public static void OnStart()
        {
            serviceBusConsumer.Start();
        }

        public static void OnStop()
        {
            serviceBusConsumer.Stop();
        }

    }
}
