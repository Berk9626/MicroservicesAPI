using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Play.Catalog.Contracts;
using Play.Catalog.Service.Dtos;
using Play.Catalog.Service.Entities;
using Play.Common;

namespace Play.Catalog.Service.Controllers
{
    [ApiController]
    [Route("items")]
    public class ItemsController : ControllerBase
    {
        // private static readonly List<ItemDto> items = new()
        // {
        //     new ItemDto(Guid.NewGuid(), "Potion", "Restores a small of HP", 5, DateTimeOffset.UtcNow),
        //     new ItemDto(Guid.NewGuid(), "Antidote", "Cures Poison", 7, DateTimeOffset.UtcNow),
        //     new ItemDto(Guid.NewGuid(), "Bronze sword", "Deals a small amount of damage", 20, DateTimeOffset.UtcNow),
        // };
        private readonly IRepository<Item> _itemRepository;
        // private static int requestCounter = 0; //neden static = it doesnt reset after every request 
        private readonly IPublishEndpoint _publishEndpoint;
        public ItemsController(IRepository<Item> itemRepository, IPublishEndpoint publishEndpoint)
        {
            _itemRepository = itemRepository;
            _publishEndpoint = publishEndpoint;
        }
      

        [HttpGet]
        public async Task <ActionResult<IEnumerable<ItemDto>>> GetAsync()
        {
            // requestCounter++;
            // Console.WriteLine($"Request {requestCounter}: Starting...");

            // if (requestCounter <=2 ) 
            // {
            //     Console.WriteLine($"Request {requestCounter}: Delaying...");
            //     await Task.Delay(TimeSpan.FromSeconds(10));
                
            // }

            //  if (requestCounter <=4 )
            // {
            //     Console.WriteLine($"Request {requestCounter}: 500 (Internal Server Error)...");
            //     return StatusCode(500);
                
            // }

            var items = (await _itemRepository.GetAllAsync())
            .Select(item=> item.AsDto()); //extension 

            // Console.WriteLine($"Request {requestCounter}: 200 (OK)...");
            
            return Ok(items);

        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ItemDto>> GetByIdAsync(Guid id)
        {
            var item = await _itemRepository.GetAsync(id);
            
            if(item == null)
            {
                return NotFound();
            }
            return item.AsDto();
        }

        [HttpPost]
        public async Task<ActionResult<ItemDto>> PostAsync(CreateItemDto createItemDto)
        {
            var item = new Item{
                Name = createItemDto.Name,
                Description = createItemDto.Description,
                Price = createItemDto.Price,
                CreatedDate = DateTimeOffset.UtcNow
            };
         
          await _itemRepository.CreateAsync(item);

          //We gonna do publish message has been that the item has benn created
          await _publishEndpoint.Publish(new CatalogItemCreated(item.Id, item.Name, item.Description));



          return CreatedAtAction(nameof(GetByIdAsync), new {id = item.Id}, item);
              
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutAsync(Guid id, UpdateItemDto updateItemDto) //Sadece işimi yapacağım dönecek bir şey yok
        {
            var existingItem = await _itemRepository.GetAsync(id);

            if(existingItem == null){
                return NotFound();
            }
         
         
         existingItem.Name = updateItemDto.Name;
         existingItem.Description = updateItemDto.Description;
         existingItem.Price = updateItemDto.Price;

         await _itemRepository.UpdateAsync(existingItem);

          await _publishEndpoint.Publish(new CatalogItemUpdated (existingItem.Id, existingItem.Name, existingItem.Description));

          
           return NoContent();
     
        }
        

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
             var item = await _itemRepository.GetAsync(id);

            if(item == null){
                return NotFound();
            }

            await _itemRepository.RemoveAsync(item);

            await _publishEndpoint.Publish(new CatalogItemDeleted(id));
            return NoContent();
            


             


        }


    }


}