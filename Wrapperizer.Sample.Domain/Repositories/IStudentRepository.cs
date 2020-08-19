using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Wrapperizer.Abstraction.Repositories;
using Wrapperizer.Sample.Domain.StudentAggregateRoot;

namespace Wrapperizer.Sample.Domain.Repositories
{
    public interface IStudentRepository : IRepository<Student>
    {
        Task<string> GetStudentFullName(Guid studentId);
        
        Task<Guid> RegisterStudent(string firstname, string lastname, string nationalCode, DateTimeOffset birthdate,
            CancellationToken cancellationToken);

        Task ConfirmRegistration(Guid studentId, CancellationToken cancellationToken);
        Task Inactivate(Guid studentId, CancellationToken cancellationToken);

        Task<IEnumerable<Student>> FilterPendingStudents();

    }
}
