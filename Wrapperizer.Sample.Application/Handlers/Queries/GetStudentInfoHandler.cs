using System.Threading;
using System.Threading.Tasks;
using Wrapperizer.Abstraction.Cqrs;
using Wrapperizer.Abstraction.Repositories;
using Wrapperizer.Sample.Domain.Queries;
using Wrapperizer.Sample.Domain.Repositories;
using Wrapperizer.Sample.Domain.StudentAggregateRoot;

namespace Wrapperizer.Sample.Application.Handlers.Queries
{
    public sealed class GetStudentInfoHandler : IQueryHandler<GetStudentInfo, string>
    {
        private readonly IStudentRepository _repository;

        // todo: this should be changed, only for test of DI 
        public GetStudentInfoHandler(IRepository<Student> repository)
        {
            _repository = repository as IStudentRepository;
        }

        public Task<string> Handle(GetStudentInfo request, CancellationToken cancellationToken)
            => _repository.GetStudentFullName(request.StudentId);
    }
}
