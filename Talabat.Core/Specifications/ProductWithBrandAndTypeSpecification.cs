using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;

namespace Talabat.Core.Specifications
{
    public class ProductWithBrandAndTypeSpecification:BaseSpecification<Product>
    {
        // CTOR used for Get All
        public ProductWithBrandAndTypeSpecification(ProductSpecParams Param)
            : base(P =>
                 (string.IsNullOrEmpty(Param.Search) || P.Name.ToLower().Contains(Param.Search))
                 &&
                 (!Param.TypeId.HasValue || P.ProductTypeId == Param.TypeId)
                 &&
                 (!Param.BrandId.HasValue || P.ProductBrandId == Param.BrandId)
                 )
        {
            Includes.Add(P => P.ProductType);
            Includes.Add(P => P.ProductBrand);
            if(!string.IsNullOrEmpty(Param.Sort))
            {
                switch(Param.Sort)
                {
                    case "PriceAsc":
                        AddOrderBy(p=>p.Price);
                        break;
                    case "PriceDesc":
                        AddOrderByDesc(p => p.Price);
                        break;
                    default:
                        AddOrderBy(n => n.Name);
                        break;
                }
            }

            // products = 20
            // page size = 5
            // page index = 3
            // skip => 10 = 5*2
            // take = 5
            ApplyPagination(Param.PageSize * (Param.PageIndex - 1), Param.PageSize);
        }

        // CTOR userd for Get By Id
        public ProductWithBrandAndTypeSpecification(int id):base(P => P.Id == id)
        {
            Includes.Add(P => P.ProductType);
            Includes.Add(P => P.ProductBrand);
        }
    }
}
