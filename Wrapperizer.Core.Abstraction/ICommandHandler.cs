using MediatR;

namespace Wrapperizer.Core.Abstraction
{
    public interface ICommandHandler : IRequestHandler<ICommand>
    {
        IActionResultAdapter ActionResultAdapter { get; }
    }

    public interface ICommandHandler<in TCommand, TResult> : IRequestHandler<TCommand, TResult>
        where TCommand : ICommand<TResult>, ICommand
    {
        IActionResultAdapter ActionResultAdapter { get; }
    }

    public interface IQueryHandler<in TQuery, TResult> : IRequestHandler<TQuery, TResult>
        where TQuery : IQuery<TResult>
    {
        IActionResultAdapter ActionResultAdapter { get; }
    }
}
