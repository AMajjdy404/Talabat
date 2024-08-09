using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Talabat.API.DTOs;
using Talabat.API.Errors;
using Talabat.API.Helpers;
using Talabat.Core;
using Talabat.Core.Entities;
using Talabat.Core.IRepository;
using Talabat.Core.Specifications;

namespace Talabat.API.Controllers
{
   
    public class ProductsController : BaseApiController
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public ProductsController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        // GetAll Products
        [CachedAttribute(600)]
        [HttpGet]
        public async Task<ActionResult<Pagination<ProductToReturnDto>>> GetProducts([FromQuery] ProductSpecParams param)
        {
            var Spec = new ProductWithBrandAndTypeSpecification(param);
            var Products = await  unitOfWork.Repository<Product>().GetAllWithSpecAsync(Spec);
            var MappedProducts = mapper.Map<IReadOnlyList<Product>, IReadOnlyList<ProductToReturnDto>>(Products);
            ///var ReturnedObject = new Pagination<ProductToReturnDto>()
            ///{
            ///    PageSize = param.PageSize,
            ///    PageIndex = param.PageIndex,
            ///    Data = MappedProducts
            ///};
            var CountSpec = new ProductWithFilterationCountAsync(param);
            var Count = await unitOfWork.Repository<Product>().GetCountWithSpecAsync(CountSpec);
            return Ok(new Pagination<ProductToReturnDto>(param.PageSize,param.PageIndex,MappedProducts,Count));
        }

        // Get Product By Id
        [CachedAttribute(600)]
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ProductToReturnDto),200)]
        [ProducesResponseType(typeof(ApiResponse),StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProductToReturnDto>> GetProduct(int id)
        {
            var Spec = new ProductWithBrandAndTypeSpecification(id);
            var Product = await unitOfWork.Repository<Product>().GetEntitylWithSpecAsync(Spec);
            if (Product == null)
                return NotFound(new ApiResponse(404));
            var MappedProduct = mapper.Map<Product, ProductToReturnDto>(Product);
            return Ok(MappedProduct);
        }

        // Get All Types
        // BasUrl/api/Product/Types
        
        [HttpGet("Types")]
        public async Task<ActionResult<IReadOnlyList<ProductType>>> GetAllTypes()
        {
            var Types = await unitOfWork.Repository<ProductType>().GetAllAsync();
            return Ok(Types);
        }

        // Get All Brands
        // BasUrl/api/Product/Types
       
        [HttpGet("Brands")]
        public async Task<ActionResult<IReadOnlyList<ProductBrand>>> GetAllBrands()
        {
            var Brands = await unitOfWork.Repository<ProductBrand>().GetAllAsync();
            return Ok(Brands);
        }

    }
}
