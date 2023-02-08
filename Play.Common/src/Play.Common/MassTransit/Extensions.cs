using System;
using System.Reflection;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Play.Common.Settings;


namespace Play.Common.MassTransit
{
    public static class Extensions
    {
        public static IServiceCollection AddMassTransitwithRabbitMQ(this IServiceCollection services)
        {
            services.AddMassTransit(configure =>
            {
                configure.AddConsumers(Assembly.GetEntryAssembly()); //diğer servisler de kullanabilsin diye



                configure.UsingRabbitMq((context, configurator) =>
                 {
                     var configuration = context.GetService<IConfiguration>();
                     var serviceSettings = configuration.GetSection(nameof(ServiceSettings)).Get<ServiceSettings>();
                     var rabbitMQSettings = configuration.GetSection(nameof(RabbitMQSettings)).Get<RabbitMQSettings>();//we can figure out whats the host that we are going to use 
                     configurator.Host(rabbitMQSettings.Host); //rabbitmq nerde yaşıyorsa
                     configurator.ConfigureEndpoints(context, new KebabCaseEndpointNameFormatter(serviceSettings.ServiceName, false));//gonna help us define or modify a little bit how queues are created in the rabbitMQ
                     configurator.UseMessageRetry(retryConfigurator =>
                     {
                         retryConfigurator.Interval(3, TimeSpan.FromSeconds(5));

                     });

                 });

            });

            return services;

        }

    }
}