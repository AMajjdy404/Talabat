using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core;
using Talabat.Core.Entities;
using Talabat.Core.IRepository;
using Talabat.Repository.Data;

namespace Talabat.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly StoreContext dbContext;
        private Hashtable repositories;

        public UnitOfWork(StoreContext dbContext)
        {
            this.dbContext = dbContext;
            repositories = new Hashtable();
        }
        public async Task<int> CompleteAsync()
        => await dbContext.SaveChangesAsync();
        

        public async ValueTask DisposeAsync()
        => await dbContext.DisposeAsync();

        public IGenericRepository<TEntity> Repository<TEntity>() where TEntity : ProductBase
        {
            var type = typeof(TEntity).Name; // Product, Order, ...
            if (!repositories.ContainsKey(type))
            {
                var Repository = new GenericRepository<TEntity>(dbContext);
                repositories.Add(type, Repository);
            }
            return (IGenericRepository<TEntity>)repositories[type];
        }
    }
}
