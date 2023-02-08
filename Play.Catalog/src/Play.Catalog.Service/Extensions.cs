using Play.Catalog.Service.Dtos;
using Play.Catalog.Service.Entities;

namespace Play.Catalog.Service
{
    public static class Exttension
    {
        public static ItemDto AsDto(this Item item)
        {
            return new ItemDto(item.Id, item.Name, item.Description, item.Price, item.CreatedDate);
            

        }

    }
    
}