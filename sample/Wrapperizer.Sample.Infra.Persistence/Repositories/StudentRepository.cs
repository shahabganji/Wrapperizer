using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Wrapperizer.Abstraction.Repositories;
using Wrapperizer.Sample.Domain.Repositories;
using Wrapperizer.Sample.Domain.Specifications;
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

        public async Task<string> GetStudentFullName(Guid studentId)
        {
            var x = await _dbContext.Students.SingleOrDefaultAsync(
                    s => s.Id == studentId)
                .ConfigureAwait(false);

            if (x != null)
                await _dbContext.Entry(x).Reference(y => y.RegistrationStatus).LoadAsync(CancellationToken.None);

            return x?.LastName;
        }

        public async Task<Guid> RegisterStudent(string firstname, string lastname, string nationalCode,
            DateTimeOffset birthdate, CancellationToken cancellationToken)
        {
            var student = new Student(firstname, lastname, nationalCode, birthdate);
            await _dbContext.Students.AddAsync(student, cancellationToken).ConfigureAwait(false);

            await this.UnitOfWork.CommitAsync(cancellationToken).ConfigureAwait(false);

            return student.Id;
        }

        public async Task ConfirmRegistration(Guid studentId, CancellationToken cancellationToken)
        {
            var student = await _dbContext.Students.SingleOrDefaultAsync(
                    s => s.Id == studentId && RegistrationStatus.Requested == s.RegistrationStatus,
                    cancellationToken)
                .ConfigureAwait(false);

            if (student == null) return;

            student.ConfirmRegistration();
            await this.UnitOfWork.CommitAsync(cancellationToken).ConfigureAwait(false);
        }

        public async Task Inactivate(Guid studentId, CancellationToken cancellationToken)
        {
            var student = await _dbContext.Students.FindAsync(studentId).ConfigureAwait(false);

            if (student == null) throw new InvalidOperationException("No such student");

            _dbContext.Students.Remove(student);
            await this.UnitOfWork.CommitAsync(cancellationToken);
        }

        public async Task<IEnumerable<Student>> FilterPendingStudents()
        {
            var pendingSpec = new PendingStudentSpecification().ToExpression();
            var students = await _dbContext.Students.Where(pendingSpec).ToListAsync();

            return students;
        }
        
        public async Task<IEnumerable<Student>> FilterPendingStudentsWithDoubleZero()
        {
            var composite = new MyCompositeSpecification();
            var students = await _dbContext.Students.Where(composite).ToListAsync();

            return students;
        }
    }
}
