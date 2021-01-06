using System;
using Wrapperizer.Domain.Abstraction.Cqrs;

namespace Wrapperizer.Sample.Domain.Queries
{
    public sealed class GetStudentInfo : IQuery<string>
    {
        public Guid StudentId { get; set; }
    }
}
