using System.Threading.Tasks;
using MediatR;
using Wrapperizer.Domain.Abstraction.Cqrs;
using Wrapperizer.Domain.Abstraction.Domain;

namespace Wrapperizer.Extensions.Cqrs.Managers
{
    internal sealed class DomainEventCommandQueryManager : IDomainEventManager , ICommandQueryManager
    {
        private readonly IMediator _mediator;

        public DomainEventCommandQueryManager(IMediator mediator)
        {
            _mediator = mediator;
        }

        public Task Publish(IDomainEvent @event) 
            => _mediator.Publish(@event);

        public async Task Publish(params IDomainEvent[] events)
        {
            foreach (var @event in events)
            {
                await this.Publish(@event).ConfigureAwait(false);
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
