using MediatR;

namespace Wrapperizer.Core.Abstraction
{
    public interface ICommand<out TResult> : IRequest<TResult>
    {
    }

    public interface ICommand : IRequest
    {
    }

    public interface IQuery<out TResult> : IRequest<TResult>
    {
    }
}
