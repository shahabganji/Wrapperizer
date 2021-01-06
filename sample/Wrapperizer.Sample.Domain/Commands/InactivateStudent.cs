using System;
using Wrapperizer.Domain.Abstraction.Cqrs;

namespace Wrapperizer.Sample.Domain.Commands
{
    public class InactivateStudent : ICommand
    {
        public Guid StudentId { get; set; }
    }
}
