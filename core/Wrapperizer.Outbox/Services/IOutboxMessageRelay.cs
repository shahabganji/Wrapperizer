using System.Threading.Tasks;

namespace Wrapperizer.Outbox.Services
{
    public interface IOutboxMessageRelay
    {
        Task PublishEventsThroughEventBusAsync();
    }
}
