using System;
using System.Threading;
using System.Threading.Tasks;
using Wrapperizer.Abstraction.Repositories;
using Wrapperizer.Sample.Domain.StudentAggregateRoot;

namespace Wrapperizer.Sample.Domain.Repositories
{
    public interface IStudentRepository : IRepository<Student>
    {
        Task<Guid> RegisterStudent(string firstname, string lastname, string nationalCode, DateTimeOffset birthdate,
            CancellationToken cancellationToken);

        Task ConfirmRegistration(Guid studentId, CancellationToken cancellationToken);
    }
}
