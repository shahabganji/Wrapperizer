using System.Threading;
using System.Threading.Tasks;
using Wrapperizer.Abstraction.Cqrs;
using Wrapperizer.Sample.Domain.Queries;
using Wrapperizer.Sample.Domain.Repositories;

namespace Wrapperizer.Sample.Application.Handlers.Queries
{
    public sealed class GetStudentInfoHandler : IQueryHandler<GetStudentInfo, string>
    {
        private readonly IStudentRepository _repository;

        public GetStudentInfoHandler(IStudentRepository repository)
        {
            _repository = repository;
        }

        public Task<string> Handle(GetStudentInfo request, CancellationToken cancellationToken)
            => _repository.GetStudentFullName(request.StudentId);
    }
}
