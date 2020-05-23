using MediatR;

namespace Wrapperizer.Core.Abstraction
{
    public interface ICommand : ICommand<Unit>
    {
    }
    
    public interface ICommand<out TResult> : IRequest<TResult>
    {
    }


    public interface IQuery<out TResult> : IRequest<TResult>
    {
    }
}
