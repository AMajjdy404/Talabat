using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;

namespace Talabat.Core.Specifications
{
    public class ProductWithFilterationCountAsync: BaseSpecification<Product>
    {
        public ProductWithFilterationCountAsync(ProductSpecParams Param) : base(P =>
                 (string.IsNullOrEmpty(Param.Search) || P.Name.ToLower().Contains(Param.Search))
                 &&
                 (!Param.TypeId.HasValue || P.ProductTypeId == Param.TypeId)
                 &&
                 (!Param.BrandId.HasValue || P.ProductBrandId == Param.BrandId)
                 )
        {
            
        }
    }
}
