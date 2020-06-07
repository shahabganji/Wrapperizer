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

        public Task Dispatch(IDomainEvent @event) 
            => _mediator.Publish(@event);

        public async Task Dispatch(params IDomainEvent[] events)
        {
            foreach (var @event in events)
            {
                await this.Dispatch(@event).ConfigureAwait(false);
            }
        }

        public Task Send(ICommand command)
            => _mediator.Send(command);

        public Task<TResponse> Send<TResponse>(ICommand<TResponse> command) 
            => _mediator.Send(command);

        public  Task<TResponse> Send<TResponse>(IQuery<TResponse> command)
            =>  _mediator.Send(command);
    }
}
