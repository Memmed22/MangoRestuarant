using Mango.Services.PaymentAPI.Messaging;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mango.Services.PaymentAPI.Extension
{
    public static class ApplicationBuilderExtension
    {
        public static IAzureServiceBusConsumer serviceBusConsumer { get; set; }

        public static IApplicationBuilder UserAzureServiceBusConsumer(this IApplicationBuilder app)
        {
            serviceBusConsumer = (IAzureServiceBusConsumer) app.ApplicationServices.GetService(typeof(IAzureServiceBusConsumer));
            var hostApplicatonLife = (IHostApplicationLifetime)app.ApplicationServices.GetService(typeof(IHostApplicationLifetime));

            hostApplicatonLife.ApplicationStarted.Register(OnStart);
            hostApplicatonLife.ApplicationStopped.Register(OnStop);

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
