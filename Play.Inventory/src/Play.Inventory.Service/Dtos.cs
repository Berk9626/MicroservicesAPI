using System;

namespace Play.Inventory.Service.Dtos
{
    //first DTO going to use grant items to a user, other going to use return the series
    public record GrantItemsDto(Guid UserId, Guid CatalogItemId, int Quantity); //which is the item the catalog item that's going to be assigned = catalog item id, how much item goin to be
    //assigned to the user = int Quantity

    public record InventoryItemDto(Guid CatalogItemId,string Name,string Description, int Quantity, DateTimeOffset AcquiredDate);// gonna return items that are assigned to a user

    public record CatalogItemDto(Guid Id, string Name, string Description);//bunun için client yarattık. Bu bizim Catalog için yolladığımız request
    
}