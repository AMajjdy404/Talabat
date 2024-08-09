using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;

namespace Talabat.Core.Specifications
{
    public interface ISpecification<T> where T : ProductBase
    {

        // Signature for Where(P => P.Id == id) => Where Condition
        public Expression<Func<T, bool>> Criteria { get; set; }

        // Signature for Include(P => P.ProductType).Include(P => P.ProductBrand) => List Of Includes
        public List<Expression<Func<T, object>>> Includes { get; set; }

        // Signature for OrderBy(P=>P.Name)
        public Expression<Func<T,object>> OrderBy { get; set; }

        // Signature for OrderByDesc(P=>P.Name)
        public Expression<Func<T, object>> OrderByDesc { get; set; }

        // Skip
        public int Skip { get; set; }

        // Take
        public int Take { get; set; }

        // IsPaginationEnabled
        public bool IsPaginationEnabled { get; set; }



    }
}
