using System;
using System.Collections.Generic;
using System.Reflection;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Wrapperizer.Cqrs.Abstractions.Queries;
using Wrapperizer.Extensions.DependencyInjection.Abstractions;

namespace Wrapperizer.Extensions.DependencyInjection.Cqrs.Abstractions
{
    public static class CqrsContextBuilderServiceCollectionExtensions
    {
        // private static Assembly[] GetListOfEntryAssemblyWithReferences()
        // {
        //     var listOfAssemblies = new List<Assembly>();
        //     var mainAsm = Assembly.GetEntryAssembly();
        //     listOfAssemblies.Add(mainAsm);
        //
        //     if (mainAsm is { }) 
        //         listOfAssemblies.AddRange(mainAsm.GetReferencedAssemblies().Select(Assembly.Load));
        //
        //     return listOfAssemblies.ToArray();
        // }

        public static WrapperizerContext AddHandlers(this WrapperizerContext wrapperizerContext,
            Action<CqrsContext> configure = null,
            ServiceLifetime serviceLifetime = ServiceLifetime.Transient, params Assembly[] assemblies
            )
        {
            if(wrapperizerContext is null ) throw new ArgumentNullException(nameof(wrapperizerContext));
            
            if (assemblies == null || assemblies.Length == 0)
            {
                assemblies = AppDomain.CurrentDomain.GetAssemblies();
            }

            var cqrsContext = new CqrsContext(wrapperizerContext.Services, serviceLifetime);

            configure?.Invoke(cqrsContext);

            var ass = new List<Assembly> {typeof(IMediator).Assembly, typeof(IQuery<>).Assembly};
            ass.AddRange(assemblies);

            wrapperizerContext.Services.AddMediatR(ass, configuration => configuration = cqrsContext.ServiceLifetime switch
            {
                ServiceLifetime.Singleton => configuration.AsSingleton(),
                ServiceLifetime.Scoped => configuration.AsScoped(),
                _ => configuration.AsTransient()
            });

            return wrapperizerContext;
        }
    }
}
