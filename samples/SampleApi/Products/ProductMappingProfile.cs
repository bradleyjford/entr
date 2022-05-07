using AutoMapper;

namespace SampleApi.Products;

class ProductMappingProfile : Profile
{
    public ProductMappingProfile()
    {
        CreateMap<Product, ProductResponse>();
    }
}
