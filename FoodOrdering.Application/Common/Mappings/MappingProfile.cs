//using AutoMapper;
//using FoodOrdering.Application.Features.Auth.DTOs;
//using FoodOrdering.Application.Features.Categories.DTOs;
//using FoodOrdering.Application.Features.Orders.DTOs;
//using FoodOrdering.Application.Features.Payments.DTOs;
//using FoodOrdering.Application.Features.Products.DTOs;
//using FoodOrdering.Domain.Entities;


//namespace FoodOrdering.Application.Common.Mappings
//{
//   public class MappingProfile : Profile
//        {
//            public MappingProfile()
//            {
//                CreateMap<ApplicationUser, UserDto>()
//                    .ForMember(d => d.FullName, o => o.MapFrom(s => $"{s.FirstName} {s.LastName}"));

//                CreateMap<Category, CategoryDto>();

//                CreateMap<Product, ProductDto>()
//                    .ForMember(d => d.CategoryNameEn, o => o.MapFrom(s => s.Category != null ? s.Category.NameEn : string.Empty))
//                    .ForMember(d => d.CategoryNameAr, o => o.MapFrom(s => s.Category != null ? s.Category.NameAr : string.Empty));

//                CreateMap<Order, OrderDto>()
//                    .ForMember(d => d.UserName, o => o.MapFrom(s => s.User != null ? $"{s.User.FirstName} {s.User.LastName}" : string.Empty))
//                    .ForMember(d => d.StatusName, o => o.MapFrom(s => s.Status.ToString()))
//                    .ForMember(d => d.Payment, o => o.MapFrom(s => s.Payment));

//                CreateMap<OrderItem, OrderItemDto>()
//                    .ForMember(d => d.ProductNameEn, o => o.MapFrom(s => s.Product != null ? s.Product.NameEn : string.Empty))
//                    .ForMember(d => d.ProductNameAr, o => o.MapFrom(s => s.Product != null ? s.Product.NameAr : string.Empty))
//                    .ForMember(d => d.TotalPrice, o => o.MapFrom(s => s.TotalPrice));

//                CreateMap<Payment, OrderPaymentDto>()
//                    .ForMember(d => d.MethodName, o => o.MapFrom(s => s.Method.ToString()))
//                    .ForMember(d => d.StatusName, o => o.MapFrom(s => s.Status.ToString()));

//                CreateMap<Payment, PaymentDto>()
//                    .ForMember(d => d.MethodName, o => o.MapFrom(s => s.Method.ToString()))
//                    .ForMember(d => d.StatusName, o => o.MapFrom(s => s.Status.ToString()));
//            }
//        }
//    }
using AutoMapper;
using FoodOrdering.Application.Features.Auth.DTOs;
using FoodOrdering.Application.Features.Categories.DTOs;
using FoodOrdering.Application.Features.Contacts.DTOs;
using FoodOrdering.Application.Features.Orders.DTOs;
using FoodOrdering.Application.Features.Payments.DTOs;
using FoodOrdering.Application.Features.Products.DTOs;
using FoodOrdering.Application.Features.Reviews.DTOs;
using FoodOrdering.Domain.Entities;

namespace FoodOrdering.Application.Common.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<ApplicationUser, UserDto>()
                .ForMember(d => d.FullName, o => o.MapFrom(s => $"{s.FirstName} {s.LastName}"));

            // Category.ImageUrl  : string  →  CategoryDto.ImageUrl : string  ✓
            // (was failing because CategoryDto.ImageUrl was typed as IFormFile)
            CreateMap<Category, CategoryDto>();
            CreateMap<CategoryDto, Category>().ForMember(x => x.ImageUrl, opt => opt.Ignore());
            //CreateMap<Upda, Category>().ForMember(x => x.ImageUrl, opt => opt.Ignore());
            CreateMap<Product, ProductDto>()
                .ForMember(d => d.CategoryNameEn,
                    o => o.MapFrom(s => s.Category != null ? s.Category.NameEn : string.Empty))
                .ForMember(d => d.CategoryNameAr,
                    o => o.MapFrom(s => s.Category != null ? s.Category.NameAr : string.Empty)).ForMember(d => d.AverageRating,
                o => o.MapFrom(s =>
                    s.Reviews.Any(r => r.IsApproved)
                        ? Math.Round(s.Reviews.Where(r => r.IsApproved).Average(r => (double)r.Rating), 2)
                        : 0.0))
            .ForMember(d => d.ReviewCount,
                o => o.MapFrom(s => s.Reviews.Count(r => r.IsApproved)));


            CreateMap<Product, ReturnProductDto>()
                .ForMember(d => d.CategoryNameEn,
                    o => o.MapFrom(s => s.Category != null ? s.Category.NameEn : string.Empty))
                .ForMember(d => d.CategoryNameAr,
                    o => o.MapFrom(s => s.Category != null ? s.Category.NameAr : string.Empty))
                 .ForMember(d => d.AverageRating,
                    o => o.MapFrom(s =>
                          s.Reviews.Any(r => r.IsApproved)
                        ? Math.Round(s.Reviews.Where(r => r.IsApproved).Average(r => (double)r.Rating), 2)
                        : 0.0))
                 .ForMember(d => d.ReviewCount,
                o => o.MapFrom(s => s.Reviews.Count(r => r.IsApproved)));
             
            CreateMap<Order, OrderDto>()
                .ForMember(d => d.UserName, o => o.MapFrom(s => s.User != null ? $"{s.User.FirstName} {s.User.LastName}" : string.Empty))
                .ForMember(d => d.StatusName, o => o.MapFrom(s => s.Status.ToString()))
                .ForMember(d => d.Payment, o => o.MapFrom(s => s.Payment));

            CreateMap<OrderItem, OrderItemDto>()
                .ForMember(d => d.ProductNameEn, o => o.MapFrom(s => s.Product != null ? s.Product.NameEn : string.Empty))
                .ForMember(d => d.ProductNameAr, o => o.MapFrom(s => s.Product != null ? s.Product.NameAr : string.Empty))
                .ForMember(d => d.TotalPrice, o => o.MapFrom(s => s.TotalPrice));

            CreateMap<Payment, OrderPaymentDto>()
                .ForMember(d => d.MethodName, o => o.MapFrom(s => s.Method.ToString()))
                .ForMember(d => d.StatusName, o => o.MapFrom(s => s.Status.ToString()));

            CreateMap<Payment, PaymentDto>()
                .ForMember(d => d.MethodName, o => o.MapFrom(s => s.Method.ToString()))
                .ForMember(d => d.StatusName, o => o.MapFrom(s => s.Status.ToString()));

            CreateMap<Review, ReviewDto>()
       .ForMember(d => d.UserFullName,
           o => o.MapFrom(s => s.User != null
               ? $"{s.User.FirstName} {s.User.LastName}"
               : string.Empty))
       .ForMember(d => d.ProductNameEn,
           o => o.MapFrom(s => s.Product != null ? s.Product.NameEn : string.Empty))
       .ForMember(d => d.ProductNameAr,
           o => o.MapFrom(s => s.Product != null ? s.Product.NameAr : string.Empty));

            // ── Contact ───────────────────────────────────────────────
            CreateMap<Contact, ContactDto>();
        }
    }
}
