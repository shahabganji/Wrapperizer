using MediatR;

namespace Wrapperizer.Abstraction.Cqrs
{
    public interface ICommand : ICommand<Unit>
    {
    }
    
    public interface ICommand<out TResult> : IRequest<TResult>
    {
    }
}
