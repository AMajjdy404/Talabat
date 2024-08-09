using AutoMapper;
using Talabat.API.DTOs;
using Talabat.Core.Entities.Order_Aggreagate;

namespace Talabat.API.Helpers
{
    public class OrderPictureUrlResolver : IValueResolver<OrderItem, OrderItemDto, string>
    {
        private readonly IConfiguration configuration;

        public OrderPictureUrlResolver(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public string Resolve(OrderItem source, OrderItemDto destination, string destMember, ResolutionContext context)
        {
            if (!string.IsNullOrEmpty(source.Product.PictureUrl))
            {
                return $"{configuration["ApiBaseUrl"]}{source.Product.PictureUrl}";
            }
            return string.Empty;
        }
    }
}
