using MediatR;

namespace Wrapperizer.Abstraction.Cqrs
{
    public interface IQuery<out TResult> : IRequest<TResult>
    {
    }
}