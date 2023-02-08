using System;
using System.ComponentModel.DataAnnotations;

namespace Play.Catalog.Service.Dtos
{
    public record ItemDto(Guid Id, string Name, string Description, decimal Price, DateTimeOffset CreatedDate); //recordlar class olarak düşünülebilir, bu dto get operasyonu için kullan.
    public record CreateItemDto([Required] string Name, string Description,[Range(0,1000)] decimal Price);//itemların yaratılması için kullanılacak
    public record UpdateItemDto([Required] string Name, string Description,[Range(0,1000)] decimal Price);//update için

    

    


    
    
}