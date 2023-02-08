using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using Play.Common.Settings;

namespace Play.Common.MongoDB
{
    public static class Extensions
    {
        public static IServiceCollection AddMongo(this IServiceCollection services)
        {
            BsonSerializer.RegisterSerializer(new GuidSerializer(BsonType.String)); //düzgün yazılan stringe dönüşmüş mongodb 
            BsonSerializer.RegisterSerializer(new DateTimeOffsetSerializer(BsonType.String));

            
              

            services.AddSingleton(serviceProvider =>
            {
                var configuration = serviceProvider.GetService<IConfiguration>();
                var serviceSettings = configuration.GetSection(nameof(ServiceSettings)).Get<ServiceSettings>(); //configurationmongodb
                var mongoDbSettings = configuration.GetSection(nameof(MongoDbSettings)).Get<MongoDbSettings>();
                var mongoClient = new MongoClient(mongoDbSettings.ConnectionString);//we need connect to mongo db
                return mongoClient.GetDatabase(serviceSettings.ServiceName);//WE can instance of the database object that we care about
                                                                            //we are constructing and registering it with the service container
            });//constructmongodbclient, construct: inşaa etmek
            return services;

        }

        //register the repository itself
        public static IServiceCollection AddMongoRepository<T>(this IServiceCollection services, string collectionName) where T: IEntity
        {  //we have collection name in order to create a repository

          services.AddSingleton<IRepository<T>>(serviceProvider => 
            {
                var database = serviceProvider.GetService<IMongoDatabase>();
                return new MongoRepository<T>(database, collectionName);

            });//register the Itemrepository withdependency

            return services;
        } 

    }
    
}