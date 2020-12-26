using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Wrapperizer.Sample.Domain.Commands;
using Wrapperizer.Sample.Domain.Repositories;

namespace Wrapperizer.Sample.Application.Handlers.Commands
{
    public sealed class RegisterStudentHandler : IRequestHandler<RegisterStudent, Guid>
    {
        private readonly IStudentRepository _repository;

        public RegisterStudentHandler(IStudentRepository repository)
        {
            _repository = repository;
        }

        public async Task<Guid> Handle(RegisterStudent command, CancellationToken cancellationToken)
        {
            var studentId = await _repository.RegisterStudent(command.FirstName, command.LastName, command.NationalCode,
                command.Birthdate, cancellationToken).ConfigureAwait(false);

            return studentId;
        }
    }
}
