using MediatR;

namespace Wrapperizer.Core.Abstraction
{
    public interface ICommandHandler : IRequestHandler<ICommand>
    {
    }

    public interface ICommandHandler<in TCommand, TResult> : IRequestHandler<TCommand, TResult>
        where TCommand : IRequest<TResult>
    {   
    }

    public interface IQueryHandler<in TQuery, TResult> : IRequestHandler<TQuery, TResult>
        where TQuery : IRequest<TResult>
    {
    }
}
