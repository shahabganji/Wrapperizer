using System.Threading.Tasks;
using Wrapperizer.Abstraction.Domain;

namespace Wrapperizer.Outbox.Services
{
    public interface IOutboxMessageRelay
    {
        Task PublishEventsThroughEventBusAsync();
    }
}
