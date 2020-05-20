using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Funx;
using Wrapperizer.Core.Abstraction.Specifications;

namespace Wrapperizer.Core.Abstraction
{
    public interface ICrudRepository<T> where T : class, IAggregateRoot
    {
        Task<Option<T>> FindByKey(object key,params Expression<Func<T, object>>[] includeProperties);
        Task<IReadOnlyCollection<T>> FindBy(Specification<T> specification,
            params Expression<Func<T, object>>[] includeProperties);
        Task<IReadOnlyCollection<T>> FindBy(Expression<Func<T, bool>> predicate,
            params Expression<Func<T, object>>[] includeProperties);

        Task<bool> Add(T entity);
        Task<bool> Delete(T entity);
        Task<bool> Update(T entity);
    }
}
