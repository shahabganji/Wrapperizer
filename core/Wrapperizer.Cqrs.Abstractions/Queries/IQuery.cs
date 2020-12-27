using System.Collections.Generic;
using MediatR;
using Wrapperizer.Domain.Abstractions;

namespace Wrapperizer.Cqrs.Abstractions.Queries
{
    public interface IQuery<TResult> : IRequest<Result<TResult>>
    {
    }
}
