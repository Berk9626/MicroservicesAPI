using System;
using Play.Common;

namespace Play.Inventory.Service.Entities
{
    public class CatalogItem : IEntity
    {
        public Guid Id { get ;set; } // we will only introduce the properties that make sense for inventory
        public string Name { get; set; }

        public string Description { get; set; }
       
    }
}