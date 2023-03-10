using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MongoDB.Driver;


namespace Play.Common.MongoDB
{

    public class MongoRepository<T>  : IRepository<T> where T: IEntity
    {
        
        private readonly IMongoCollection<T> dbCollection;
        private readonly FilterDefinitionBuilder<T> filterBuilder = Builders<T>.Filter;//build the filters to query for items in the mongo db

        public MongoRepository(IMongoDatabase database, string collectionName)
        {
            // var MongoClient = new MongoClient("mongodb://localhost:27017");  //connection string to connect mongodb
            // var database = MongoClient.GetDatabase("Catalog");
            
            dbCollection = database.GetCollection<T>(collectionName);
        }


        public async Task<IReadOnlyCollection<T>> GetAllAsync()
        {

            return await dbCollection.Find(filterBuilder.Empty).ToListAsync();
        }

        public async Task<IReadOnlyCollection<T>> GetAllAsync(Expression<Func<T, bool>> filter)
        {
            return await dbCollection.Find(filter).ToListAsync();
        }


        public async Task<T> GetAsync(Guid id)
        {
            FilterDefinition<T> filter = filterBuilder.Eq(entity => entity.Id, id);
            return await dbCollection.Find(filter).FirstOrDefaultAsync();

        }

        public async Task<T> GetAsync(Expression<Func<T, bool>> filter)
        {
            
            return await dbCollection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task CreateAsync(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }
            await dbCollection.InsertOneAsync(entity);
        }

        public async Task UpdateAsync(T entity)
        {

            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }
            FilterDefinition<T> filter = filterBuilder.Eq(existingentity => existingentity.Id, entity.Id);
            await dbCollection.ReplaceOneAsync(filter, entity);

        }

        public async Task RemoveAsync(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            FilterDefinition<T> filter = filterBuilder.Eq(deletedEntity => deletedEntity.Id, entity.Id);
            await dbCollection.DeleteOneAsync(filter);

        }

        

        
    }
}