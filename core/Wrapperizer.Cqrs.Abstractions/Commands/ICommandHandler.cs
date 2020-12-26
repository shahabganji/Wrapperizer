using MediatR;
using Wrapperizer.Domain.Abstractions;

namespace Wrapperizer.Cqrs.Abstractions.Commands
{
    /// <inheritdoc />
    public interface ICommandHandler<in TCommand, TResult> :
        IRequestHandler<TCommand, ViewResult<TResult>> where TCommand : IRequest<ViewResult<TResult>>
    {
    }

    /// <inheritdoc />
    public interface ICommandHandler<in TCommand> : IRequestHandler<TCommand,ViewResult> 
        where TCommand : IRequest<ViewResult>
    {
    }
}
