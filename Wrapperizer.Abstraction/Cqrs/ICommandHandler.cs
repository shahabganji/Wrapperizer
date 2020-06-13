using MediatR;

namespace Wrapperizer.Abstraction.Cqrs
{
    public interface ICommandHandler<T> : IRequestHandler<T> where T : IRequest<Unit>
    {
    }

    public interface ICommandHandler<TCommand, TResult> : IRequestHandler<TCommand, TResult>
        where TCommand : ICommand<TResult>
    {
    }

    public interface IQueryHandler<TQuery, TResult> : IRequestHandler<TQuery, TResult>
        where TQuery : IQuery<TResult>
    {
    }
}
