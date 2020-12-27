using MediatR;
using Wrapperizer.Domain.Abstractions;

namespace Wrapperizer.Cqrs.Abstractions.Commands
{
    /// <inheritdoc />
    public interface ICommand<TResult> : IRequest<Result<TResult>>
    {
    }
    
    /// <inheritdoc />
    public interface ICommand : IRequest<Result>
    {
    }
}
