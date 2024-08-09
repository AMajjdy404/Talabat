using Microsoft.AspNetCore.Mvc;
using Talabat.API.Errors;
using Talabat.API.Helpers;
using Talabat.Core;
using Talabat.Core.IRepository;
using Talabat.Core.Services;
using Talabat.Repository;
using Talabat.Service;

namespace Talabat.API.Extensions
{
    public static class ApplicationServicesExtension
    {
        public static IServiceCollection AddApplicationService(this IServiceCollection Services)
        {
            Services.AddSingleton<IResponseCacheService, ResponseCacheService>();
            Services.AddScoped<IUnitOfWork, UnitOfWork>();
            Services.AddScoped<IPaymentService, PaymentService>();
            Services.AddScoped<IOrderService, OrderService>();
            Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            Services.AddScoped<IBasketRepository,BasketRepository>();
            //builder.Services.AddAutoMapper(p => p.AddProfile(new MappingProfiles()));
            Services.AddAutoMapper(typeof(MappingProfiles));

            #region Validation Error Handling
            Services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = (actionContext) =>
                {
                    // ModelState => Dic [Key Value Pair]
                    // Key => Name of Parameter 
                    // Value => Errors
                    var errors = actionContext.ModelState.Where(P => P.Value.Errors.Count() > 0)
                                                         .SelectMany(p => p.Value.Errors)
                                                         .Select(e => e.ErrorMessage)
                                                         .ToArray();
                    var ValidationErrorResponse = new ApiValidationErrorResponse()
                    {
                        Errors = errors
                    };
                    return new BadRequestObjectResult(ValidationErrorResponse);
                };
            });
            #endregion

            return Services;
        }
    }
}
