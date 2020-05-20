using Microsoft.Extensions.DependencyInjection;
using Wrapperizer.Core.Abstraction;
using Wrapperizer.Extensions.DependencyInjection.Abstractions;
using Wrapperizer.Repositories.Abstraction;

namespace Wrapperizer.Repositories
{
    public static class WrapperizerRepositoryServiceCollectionExtensions{

        public static WrapperizerRepositoryServiceCollection AddRepositories(
            this IWrapperizerBuilder wrapperizerBuilder)
         => new WrapperizerRepositoryServiceCollection(wrapperizerBuilder.ServiceCollection);
        
        public static WrapperizerRepositoryServiceCollection WithCrudRepositories
            (this WrapperizerRepositoryServiceCollection wrapperizerServiceCollection)
        {
            wrapperizerServiceCollection.ServiceCollection.AddScoped(
                typeof(ICrudRepository<>), typeof(EfCoreCrudRepository<>));
            return wrapperizerServiceCollection;
        }

        public static void WithUnitOfWork<T>(
            this WrapperizerRepositoryServiceCollection wrapperizerRepositoryServiceCollection)
        where T : IUnitOfWork
        {
            wrapperizerRepositoryServiceCollection.ServiceCollection
                .AddScoped(typeof(IUnitOfWork), typeof(T));
        }
        
        public static void WithTransactionalUnitOfWork<T>(
            this WrapperizerRepositoryServiceCollection wrapperizerRepositoryServiceCollection)
            where T : ITransactionalUnitOfWork
        {
            wrapperizerRepositoryServiceCollection.ServiceCollection
                .AddScoped(typeof(ITransactionalUnitOfWork), typeof(T));
        }
    }
}
