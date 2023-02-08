using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Play.Common.MassTransit;
using Play.Common.MongoDB;
using Play.Inventory.Service.Clients;
using Play.Inventory.Service.Entities;
using Polly;
using Polly.Timeout;

namespace Play.Inventory.Service
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMongo().AddMongoRepository<InventoryItem>("inventoryitems")
            .AddMongoRepository<CatalogItem>("catalogitems")
            .AddMassTransitwithRabbitMQ();



            AddCatalogClient(services);

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Play.Inventory.Service", Version = "v1" });
            });
        }

       

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Play.Inventory.Service v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

         private static void AddCatalogClient(IServiceCollection services)
        {
            Random jitterer = new Random();

            services.AddHttpClient<CatalogClient>(client =>
            {//base servisini tanımlıyoruz. Yoksa gideceği yeri bilemez.
                client.BaseAddress = new Uri("https://localhost:5001"); //handy way to register catalog client 
            })
            .AddTransientHttpErrorPolicy(builder => builder.Or<TimeoutRejectedException>().WaitAndRetryAsync( //timeout yüzünden faillersek 
                5,//how many times we want to retry in the presence of transient error failures
                retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))//(sleep duration provider)=function, how much time to wait between each retry, ilk san.2 sonra 4 sonra 6
                                  + TimeSpan.FromMilliseconds(jitterer.Next(0, 1000)),

                onRetry: (outcome, timespan, retryAttempt) =>
                { //what happend the between different calls

                    var serviceProvider = services.BuildServiceProvider();
                    serviceProvider.GetService<ILogger<CatalogClient>>()? //bu servisin örneğini alabiliyorsak
                    .LogWarning($"Delaying for {timespan.TotalSeconds} seconds, then making retry {retryAttempt}"); //specify that message


                }
            ))

            .AddTransientHttpErrorPolicy(builder => builder.Or<TimeoutRejectedException>().CircuitBreakerAsync(

                3, //how many faied request we are going to allow throught to circuit breaker before the circuit has to open
                 TimeSpan.FromSeconds(15),  //duration of the time span of the other break so this is how much time we ill keep the circuit open
                 onBreak: (outcome, timespan) => //WHATS GOİNG ON BEHİND THE scenes
                 {// we want to add a log message when secret breaker is opening

                     var serviceProvider = services.BuildServiceProvider();
                     serviceProvider.GetService<ILogger<CatalogClient>>()? //bu servisin örneğini alabiliyorsak
                     .LogWarning($"Opening the circuit for {timespan.TotalSeconds} seconds"); //specify that message
                 },
                 onReset: () =>
                 {
                     var serviceProvider = services.BuildServiceProvider();
                     serviceProvider.GetService<ILogger<CatalogClient>>()? //bu servisin örneğini alabiliyorsak
                     .LogWarning($"Closing the circuit..."); //specify that message


                 }
            ))

            .AddPolicyHandler(Policy.TimeoutAsync<HttpResponseMessage>(1));//this is type of response I will get when you invoke the external external client so.
                                                                           //o saniye ne kadar bekleyeceğimizi söylüyor external api çağırdığımızda hata olmasından önce.do
        }
    }
}
