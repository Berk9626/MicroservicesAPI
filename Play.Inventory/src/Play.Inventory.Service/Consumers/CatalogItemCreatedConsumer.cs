using System.Threading.Tasks;
using MassTransit;
using Play.Catalog.Contracts;
using Play.Common;
using Play.Inventory.Service.Entities;

namespace Play.Inventory.Service.Consumers
{
    public class CatalogItemCreatedConsumer : IConsumer<CatalogItemCreated> //what is the type of message that this consumer is going to handle
    {
        private readonly IRepository<CatalogItem> _repository;
        public CatalogItemCreatedConsumer(IRepository<CatalogItem> repository)
        {
            _repository = repository;
        }
        public async Task Consume(ConsumeContext<CatalogItemCreated> context)
        {
            var message = context.Message;

            var item = await _repository.GetAsync(message.ItemId);

            if (item != null)
            {
                return; //zaten bunu databasemizinde yaratmışız neden yaratalım
            }

             item = new CatalogItem
            {
                Id = message.ItemId,
                Description = message.Description,
                Name = message.Name

            };

            await _repository.CreateAsync(item); //if we got trouble over here :(
        }
    }

}