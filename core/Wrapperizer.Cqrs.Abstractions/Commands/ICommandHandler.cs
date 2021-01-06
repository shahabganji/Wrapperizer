using MediatR;
using Wrapperizer.Domain.Abstractions;

namespace Wrapperizer.Cqrs.Abstractions.Commands
{
    /// <inheritdoc />
    public interface ICommandHandler<in TCommand, TResult> :
        IRequestHandler<TCommand, Result<TResult>> where TCommand : IRequest<Result<TResult>>
    {
    }

    /// <inheritdoc />
    public interface ICommandHandler<in TCommand> : IRequestHandler<TCommand,Result> 
        where TCommand : IRequest<Result>
    {
    }
}
