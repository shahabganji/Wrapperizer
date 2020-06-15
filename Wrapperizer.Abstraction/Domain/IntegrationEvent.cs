using System;

namespace Wrapperizer.Abstraction.Domain
{
    public abstract class IntegrationEvent
    {
        public Guid Id { get; private set; }
        public DateTimeOffset OccuredOn { get; private set; }

        protected IntegrationEvent()
        {
            this.Id = Guid.NewGuid();
            this.OccuredOn = DateTimeOffset.Now;
        }
    }
}
