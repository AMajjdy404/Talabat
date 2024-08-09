using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;
using Talabat.Core.IRepository;
using Talabat.Core.Specifications;
using Talabat.Repository.Data;

namespace Talabat.Repository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : ProductBase
    {
        private readonly StoreContext context;

        public GenericRepository(StoreContext context) 
        {
            this.context = context;
        }

        #region Without Specification
        public async Task<IReadOnlyList<T>> GetAllAsync()
        {
            return await context.Set<T>().ToListAsync();
        }

        public async Task<T> GeyByIdAsync(int id)
        => await context.Set<T>().FindAsync(id);
        #endregion


        public async Task<IReadOnlyList<T>> GetAllWithSpecAsync(ISpecification<T> spec)
        {
          return await ApplySpecification(spec).ToListAsync();
        }

        public async Task<T> GetEntitylWithSpecAsync(ISpecification<T> spec)
        {
            return await ApplySpecification(spec).FirstOrDefaultAsync();
        }

        private  IQueryable<T> ApplySpecification(ISpecification<T> spec)
        {
            return  SpecificationEvaluator<T>.GenerateQuery(context.Set<T>(), spec);
        }

        public Task<int> GetCountWithSpecAsync(ISpecification<T> spec)
        {
            return ApplySpecification(spec).CountAsync();
        }

        public async Task AddAsync(T item)
        {
           await context.Set<T>().AddAsync(item);
        }

        public void Delete(T item)
        => context.Set<T>().Remove(item);

        public void Update(T item)
        => context.Set<T>().Update(item);
    }
}
