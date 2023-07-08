using AutoMapper;

namespace SampleApi.Products;

sealed class ProductsMappingProfile : Profile
{
    public ProductsMappingProfile()
    {
        CreateMap<Product, ProductResponse>();
    }
}
