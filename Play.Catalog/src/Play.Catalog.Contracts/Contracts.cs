using System;

namespace Play.Catalog.Contracts
{
   public record CatalogItemCreated(Guid ItemId, string Name, string Description);//not everyt. about cat item, really stuff that our consumers are interested in. Inv. needs to know; itemId, name, description
   public record CatalogItemUpdated(Guid ItemId, string Name, string Description);
   public record CatalogItemDeleted(Guid ItemId);
   
} 