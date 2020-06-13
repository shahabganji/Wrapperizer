using System;
using System.Threading;
using System.Threading.Tasks;
using Wrapperizer.Abstraction.Repositories;
using Wrapperizer.Extensions.Repositories.EfCore.Abstraction;
using Wrapperizer.Sample.Domain.Repositories;
using Wrapperizer.Sample.Domain.StudentAggregateRoot;

namespace Wrapperizer.Sample.Infra.Persistence.Repositories
{
    public sealed class StudentRepository : IStudentRepository
    {
        private readonly UniversityDbContext _dbContext;
        public IUnitOfWork UnitOfWork { get; }

        public StudentRepository(IUnitOfWork unitOfWork, UniversityDbContext dbContext)
        {
            _dbContext = dbContext;
            UnitOfWork = unitOfWork;
        }

        public async Task<Guid> RegisterStudent(string firstname, string lastname, string nationalCode,
            DateTimeOffset birthdate, CancellationToken cancellationToken)
        {
            var student = new Student(firstname, lastname, nationalCode, birthdate);
            await _dbContext.Students.AddAsync(student, cancellationToken).ConfigureAwait(false);

            await this.UnitOfWork.CommitAsync(cancellationToken).ConfigureAwait(false);

            return student.Id;
        }

        public Task ConfirmRegistration(Guid studentId,CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
