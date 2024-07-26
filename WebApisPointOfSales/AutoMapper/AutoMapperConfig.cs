using AutoMapper;
using PointOfSales.Entities;
using WebApisPointOfSales.Dto.ProductsDtos;
using WebApisPointOfSales.Dto.UserDtos;
using WebApisPointOfSales.Dto;
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        //Mapp for Register User DTO
        CreateMap<RegisterUserDto, Users>();

        //LOgin user DTO
        CreateMap<LoginUserDto, Users>()
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.Password, opt => opt.MapFrom(src => src.Password));


        //Dtos For Product
        CreateMap<Product, ProductDto>();
        CreateMap<CreateProductDto, Product>();
        CreateMap<UpdateProductDto, Product>();


        //Mapping For Purchase Item
        CreateMap<WebApisPointOfSales.Dto.PurchaseItemDto, PurchaseItem>()
                .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.ProductId))
                .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity));
        //Mapping for Purchase Item
        CreateMap<WebApisPointOfSales.Dto.SaleItemDTO, SaleItem>()
                .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.ProductId))
                .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity));
        // Map RegisterUserDto to Admin if UserRole is "Admin"
        CreateMap<RegisterUserDto, Admin>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.Password, opt => opt.MapFrom(src => src.Password))
            .ForMember(dest => dest.UserRole, opt => opt.MapFrom(src => "Admin"));

        // Map RegisterUserDto to Cashier if UserRole is "Cashier"
        CreateMap<RegisterUserDto, Cashier>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.Password, opt => opt.MapFrom(src => src.Password))
            .ForMember(dest => dest.UserRole, opt => opt.MapFrom(src => "Cashier"));

        
    }
}
