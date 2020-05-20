using System.Threading.Tasks;

namespace Wrapperizer.Core.Abstraction
{
    public interface ICommandQueryManager
    {
        Task Send(ICommand command);
        Task<TResponse> Send<TResponse>(ICommand<TResponse> command);
        Task<TResponse> Send<TResponse>(IQuery<TResponse> command);
    }
}
