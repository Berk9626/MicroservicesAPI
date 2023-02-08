using System.Threading.Tasks;
using MassTransit;
using Play.Catalog.Contracts;
using Play.Common;
using Play.Inventory.Service.Entities;

namespace Play.Inventory.Service.Consumers
{
    public class CatalogItemDeletedConsumer : IConsumer<CatalogItemDeleted> //what is the type of message that this consumer is going to handle
    {
        private readonly IRepository<CatalogItem> _repository;
        public CatalogItemDeletedConsumer(IRepository<CatalogItem> repository)
        {
            _repository = repository;
        }

        public async Task Consume(ConsumeContext<CatalogItemDeleted> context)
        {
            var message = context.Message;

            var item = await _repository.GetAsync(message.ItemId);

            if (item == null)
            {
                return;
            }

            await _repository.RemoveAsync(item);








        }
    }

}