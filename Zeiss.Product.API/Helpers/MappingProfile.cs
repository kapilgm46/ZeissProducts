using AutoMapper;
using Zeiss.Product.API.DTO;
using Zeiss.Product.DAL.Models;

namespace Zeiss.Product.API.Helpers
{
    public class MappingProfile:Profile
    {
        public MappingProfile() 
        {
            CreateMap<ProductModel,ProductDto>().ReverseMap();
            CreateMap<ProductRequestDto, ProductModel>().ReverseMap();
        }
    }
}
