using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using Play.Catalog.Service.Entities;
using Play.Common.MassTransit;
using Play.Common.MongoDB;
using Play.Common.Settings;

namespace Play.Catalog.Service
{
    public class Startup
    {
        private  ServiceSettings serviceSettings;//for config
        



        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            serviceSettings = Configuration.GetSection(nameof(ServiceSettings)).Get<ServiceSettings>();

            services.AddMongo().AddMongoRepository<Item>("items").AddMassTransitwithRabbitMQ(); //aslında bunu extensions kısmında generic hale getirdik. Bu kısımda kullandık sadece.
            //Extensions.cs kısmındaki AddMongoRepository methodunun içine girerek, tüm işlemleri halletti. Database'e Item yolladığını belirttik.

            //AddMassTransitwithRabbitMQ() sonradan ekledik.
            

            // services.AddMassTransit(x =>
            // {
            //     x.UsingRabbitMq((context, configurator) =>
            //      {
            //          var rabbitMQSettings = Configuration.GetSection(nameof(RabbitMQSettings)).Get<RabbitMQSettings>();//we can figure out whats the host that we are going to use 
            //          configurator.Host(rabbitMQSettings.Host); //rabbitmq nerde yaşıyorsa
            //          configurator.ConfigureEndpoints(context, new KebabCaseEndpointNameFormatter(serviceSettings.ServiceName, false));//gonna help us define or modify a little bit how queues are created in the rabbitMQ

            //      });

            // });
            //  services.AddMassTransitHostedService();//Starting RabbitMQ bus so that messages can be published to the different exchanges and queues in RabbitMQ

            services.AddControllers(options =>
            {
                options.SuppressAsyncSuffixInActionNames = false;
            });
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Play.Catalog.Service", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Play.Catalog.Service v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
