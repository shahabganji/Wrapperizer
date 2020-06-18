using System;
using Wrapperizer.Abstraction.Domain;
using Wrapperizer.Sample.Domain.Common;
using Wrapperizer.Sample.Domain.Events;
using static Wrapperizer.Sample.Domain.Common.NationalCode;
using static Wrapperizer.Sample.Domain.StudentAggregateRoot.RegistrationStatus;

namespace Wrapperizer.Sample.Domain.StudentAggregateRoot
{
    public class Student : Entity<Guid>, IAggregateRoot , ICanBeSoftDeleted, ICanBeAudited
    {
        public string FirstName { get; private set; }
        public string LastName { get; private set; }

        /// <summary>
        /// Used Primitive Obsession pattern
        /// </summary>
        public NationalCode NationalCode { get; private set; }

        public DateTimeOffset DateOfBirth { get; private set; }

        private int _registrationStatus;

        public RegistrationStatus RegistrationStatus { get; set; }
        // {
        //     get => FromValue<RegistrationStatus>(_registrationStatus);
        //     private set => _registrationStatus = value.Id;
        // }

        private Student()
        {
        }

        public Student(string firstName, string lastName, string nationalCode, DateTimeOffset dateOfBirth)
            : base(Guid.NewGuid())
        {
            FirstName = firstName ?? throw new ArgumentNullException(nameof(firstName));
            this.LastName = lastName ?? throw new ArgumentNullException(nameof(lastName));

            NationalCode = From(nationalCode);

            DateOfBirth = dateOfBirth;
            _registrationStatus  = Requested.Id;

            this.AddDomainEvent(new StudentRegistered(this));
        }

        public void ConfirmRegistration()
        {
            _registrationStatus  = Confirmed.Id;
            this.AddDomainEvent(new StudentRegistrationConfirmed(this.Id));
        }
    }
}
