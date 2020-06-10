using System;
using System.Threading;
using Wrapperizer.Abstraction.Repositories;
using Wrapperizer.Sample.Domain.StudentAggregateRoot;

namespace Wrapperizer.Sample.Domain.Repositories
{
    public interface IStudentRepository : IRepository<Student>
    {
        void RegisterStudent(string firstname, string lastname, string nationalCode, DateTimeOffset birthdate);
        void ConfirmRegistration(Guid studentId);
    }
}
