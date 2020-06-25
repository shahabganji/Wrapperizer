using System;
using Wrapperizer.Abstraction.Cqrs;

namespace Wrapperizer.Sample.Domain.Commands
{
    public class InactivateStudent : ICommand
    {
        public Guid StudentId { get; set; }
    }
}
