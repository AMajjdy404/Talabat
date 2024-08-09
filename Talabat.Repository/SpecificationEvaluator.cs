using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;
using Talabat.Core.Specifications;

namespace Talabat.Repository
{
    // _dbContext.Products.Where(P => P.Id == id).Include(P => P.ProductType).Include(P => P.ProductBrand);
    public static class SpecificationEvaluator<T> where T : ProductBase
    {
        public static IQueryable<T> GenerateQuery(IQueryable<T> InputQuery,ISpecification<T> spec)
        {
            var Query = InputQuery;
            if(spec.Criteria is not null)
            {
                Query = Query.Where(spec.Criteria); // _dbContext.Products.Where(P => P.Id == id)
            }
            if(spec.OrderBy is not null) // P => P.Price
            {
                Query = Query.OrderBy(spec.OrderBy); // // _dbContext.Products.Where(P => P.Id == id).OrderBy(P => P.Price)
            }
            if(spec.OrderByDesc is not null)
            {
                Query = Query.OrderByDescending(spec.OrderByDesc);
            }
            if(spec.IsPaginationEnabled)
            {
                Query = Query.Skip(spec.Skip).Take(spec.Take);
            }

            Query = spec.Includes.Aggregate(Query, (CurrentQuery, IncludeExp) => CurrentQuery.Include(IncludeExp));

            // _dbContext.Products.Where(P => P.Id == id).Include(P => P.ProductType);
            // _dbContext.Products.Where(P => P.Id == id).Include(P => P.ProductType).Include(P => P.ProductBrand);

            return Query;
        }
    }
}
