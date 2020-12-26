using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Funx;
using Microsoft.EntityFrameworkCore;
using Wrapperizer.Domain.Abstraction.Domain;
using Wrapperizer.Domain.Abstraction.Repositories;
using Wrapperizer.Domain.Abstraction.Specifications;

namespace Wrapperizer.Extensions.Repositories.EfCore
{
    public abstract class EfCoreCrudRepository<T> : ICrudRepository<T>
        where T : class, IAggregateRoot
    {
        private readonly DbSet<T> _dbSet;
        private readonly DbContext _dbContext;

        public EfCoreCrudRepository(DbContext dbContext)
        {
            _dbContext = dbContext;
            _dbSet = _dbContext.Set<T>();
        }

        private IQueryable<T> GetAllIncludes(params Expression<Func<T, object>>[] includeProperties)
        {
            var queryable =  _dbSet.AsNoTracking();

            return includeProperties.Aggregate(queryable,
                (current, includeProperty) => current.Include(includeProperty));
        }

        Task<Option<T>> ICrudRepository<T>.FindByKey(object key, params Expression<Func<T, object>>[] includeProperties)
        {
            return FindByKey(key, includeProperties);
        }
        public Task<IReadOnlyCollection<T>> FindBy(Expression<Func<T, bool>> predicate, params Expression<Func<T,object>>[] includeProperties)
        {
            var queryable = this.GetAllIncludes(includeProperties);
            
            return Task.FromResult<IReadOnlyCollection<T>>(
                queryable
                    .AsNoTracking()
                    .Where(predicate)
                    .ToImmutableList()
            );
        }
        public Task<IReadOnlyCollection<T>> FindBy(Specification<T> specification, params Expression<Func<T,object>>[] includeProperties)
            => this.FindBy(specification.ToExpression() , includeProperties);

        public async Task<Option<T>> FindByKey(object key, params Expression<Func<T,object>>[] includeProperties)
        {
            // the following lambda expression is meant to be build
            // entity => entity.Id == key
            var item = Expression.Parameter(typeof(T), "entity");
            var value = Expression.Constant(key);
            var property = Expression.Property(item, $"{nameof(Entity<T>.Id)}");
            var equal = Expression.Equal(property, value);
            var lambda = Expression.Lambda<Func<T, bool>>(equal, item);

            var queryable = this.GetAllIncludes(includeProperties);

            return await queryable
                .SingleOrDefaultAsync(lambda)
                .ConfigureAwait(false);
        } 

        public async Task<bool> Add(T entity)
        {
            await _dbSet.AddAsync(entity);
            return await _dbContext.SaveChangesAsync()
                .ConfigureAwait(false) >= 0;
        }

        public async Task<bool> Delete(T entity)
        {
            _dbSet.Remove(entity);
            return await _dbContext.SaveChangesAsync()
                .ConfigureAwait(false) >= 0;
        }

        public async Task<bool> Update(T entity)
        {
            _dbSet.Update(entity);
            return await _dbContext.SaveChangesAsync()
                       .ConfigureAwait(false) >= 0;
        }
    }
}
