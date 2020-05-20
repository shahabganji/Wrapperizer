using System.Threading.Tasks;
using MediatR;
using Wrapperizer.Core.Abstraction;

namespace Wrapperizer.Core
{
    internal sealed class DomainEventCommandQueryManager : IDomainEventManager , ICommandQueryManager
    {
        private readonly IMediator _mediator;

        public DomainEventCommandQueryManager(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async  Task Dispatch(IDomainEvent @event)
        {
            await _mediator.Publish(@event)
                .ConfigureAwait(false);
        }

        public async Task Dispatch(params IDomainEvent[] events)
        {
            foreach (var @event in events)
            {
                await this.Dispatch(@event).ConfigureAwait(false);
            }
        }

        public async Task Send(ICommand command) => await _mediator.Send(command).ConfigureAwait(false);

        public async Task<TResponse> Send<TResponse>(ICommand<TResponse> command) => await _mediator.Send(command).ConfigureAwait(false);

        public async Task<TResponse> Send<TResponse>(IQuery<TResponse> command) => await _mediator.Send(command).ConfigureAwait(false);
    }
}
