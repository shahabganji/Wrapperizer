using System.Threading.Tasks;

namespace Wrapperizer.Abstraction.Cqrs
{
    public interface ICommandQueryManager
    {
        Task Send(ICommand command);
        Task<TResponse> Send<TResponse>(ICommand<TResponse> command);
        Task<TResponse> Send<TResponse>(IQuery<TResponse> query);
    }
}
