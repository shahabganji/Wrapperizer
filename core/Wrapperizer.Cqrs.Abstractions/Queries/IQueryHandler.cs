using MediatR;
using Wrapperizer.Domain.Abstractions;

namespace Wrapperizer.Cqrs.Abstractions.Queries
{
    public interface IQueryHandler<in TQuery, TResult> : IRequestHandler<TQuery, Result<TResult>>
        where TQuery : IRequest<Result<TResult>>
    {
    }
}
