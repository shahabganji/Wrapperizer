using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Wrapperizer.Sample.Domain.Commands;
using Wrapperizer.Sample.Domain.Repositories;

namespace Wrapperizer.Sample.Application.Handlers.Commands
{
    public sealed class InactivateStudentHandler : IRequestHandler<InactivateStudent, Unit>
    {
        private readonly IStudentRepository _repository;

        public InactivateStudentHandler(IStudentRepository repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(InactivateStudent command, CancellationToken cancellationToken)
        {
            await _repository.Inactivate(command.StudentId, cancellationToken);
            return Unit.Value;
        }

    }
}
