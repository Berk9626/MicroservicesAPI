using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Play.Inventory.Service.Dtos;

namespace Play.Inventory.Service.Clients
{
    public class CatalogClient //amaç 2 servis arasında http aracılığıyla request ve response alışverişi.
    {
        private readonly HttpClient _httpClient;
        public CatalogClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IReadOnlyCollection<CatalogItemDto>> GetCatalogItemsAsync()
        {
            var items = await _httpClient.GetFromJsonAsync<IReadOnlyCollection<CatalogItemDto>>("/items");//parantez içine where you put the route that you want to access in the invoked 
            //address api.Our case int he cases of catalog. The route would be under "/items"
            return items;

        }

    }
    
}