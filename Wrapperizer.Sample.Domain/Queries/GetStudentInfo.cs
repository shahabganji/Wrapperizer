using System;
using Wrapperizer.Abstraction.Cqrs;

namespace Wrapperizer.Sample.Domain.Queries
{
    public sealed class GetStudentInfo : IQuery<string>
    {
        public Guid StudentId { get; set; }
    }
}
