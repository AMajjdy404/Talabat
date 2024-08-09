using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;
using Talabat.Core.Specifications;

namespace Talabat.Core.IRepository
{
    public interface IGenericRepository<T> where T : ProductBase
    {
        #region Without Specification
        // Get All
        Task<IReadOnlyList<T>> GetAllAsync();

        // Get By Id
        Task<T> GeyByIdAsync(int id);
        #endregion

        Task<IReadOnlyList<T>> GetAllWithSpecAsync(ISpecification<T> spec);

        Task<T> GetEntitylWithSpecAsync(ISpecification<T> spec);

        Task<int> GetCountWithSpecAsync(ISpecification<T> spec);

        Task AddAsync(T item);

        void Delete(T item);
        void Update(T item);

    }
}
