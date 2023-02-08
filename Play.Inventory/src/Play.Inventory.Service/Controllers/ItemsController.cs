using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Play.Common;
using Play.Inventory.Service.Clients;
using Play.Inventory.Service.Dtos;
using Play.Inventory.Service.Entities;

namespace Play.Inventory.Service.Controllers
{
    [ApiController]
    [Route("items")]
    public class ItemsController: ControllerBase 
    {
        private readonly IRepository<InventoryItem> _inventoryItemsRepository;
        private readonly IRepository<CatalogItem> _catalogitemsRepository;
        // private readonly CatalogClient _catalogClient;
        public ItemsController(IRepository<InventoryItem> inventoryItemsRepository, IRepository<CatalogItem> catalogitemsRepository)
        {
            _inventoryItemsRepository = inventoryItemsRepository;
            _catalogitemsRepository = catalogitemsRepository;
            
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<InventoryItemDto>>> GetAsync(Guid userId) //we want to get all the items in a user's inventory
        { // we need go ahead and get all the items based on the user 

        if (userId == Guid.Empty)
        {
         return BadRequest();  
        }

        // var catalogItems = await _catalogClient.GetCatalogItemsAsync(); //we get catalog items over there. Catalo microservisindeki tüm catalog itemlerı verdi
        var inventoryItemEntities = await _inventoryItemsRepository.GetAllAsync(x=>x.UserId == userId);
        var itemIds = inventoryItemEntities.Select(item => item.CatalogItemId);
        var catalogItemEntities = await _catalogitemsRepository.GetAllAsync(item => itemIds.Contains(item.Id));

        var inventoryItemDtos = inventoryItemEntities.Select(inventoryItem=>
        {
            var catalogItem = catalogItemEntities.Single(x=>x.Id == inventoryItem.CatalogItemId);
            return inventoryItem.AsDto(catalogItem.Name, catalogItem.Description);

        });

        

        return Ok(inventoryItemDtos);  

        }

        //we are goint to assign one item to the inventory. Post operation.
        [HttpPost]
        public async Task<ActionResult> PostAsync(GrantItemsDto grantItemsDto)
        {//if we have that inventoryItem already assigned in inventory because it could already have it so in which case we just need to increase the quantity or 
        //we dont have it we just have to create it for the first time in inventory

        var inventoryItem = await _inventoryItemsRepository.GetAsync(x=>x.UserId == grantItemsDto.UserId && x.CatalogItemId == grantItemsDto.CatalogItemId );
        //eğer burdaysa o itemı bulduk.

        if (inventoryItem == null)
        {
            inventoryItem = new InventoryItem
            {
                CatalogItemId = grantItemsDto.CatalogItemId,
                UserId = grantItemsDto.UserId,
                Quantity = grantItemsDto.Quantity,
                AcquiredDate = DateTimeOffset.UtcNow
                
            };
            await _inventoryItemsRepository.CreateAsync(inventoryItem);    
        }

        else // we find the item
        {
            inventoryItem.Quantity += grantItemsDto.Quantity;
            await _inventoryItemsRepository.UpdateAsync(inventoryItem) ;   
        }

        return Ok();
        
        }

        

    }
    
}