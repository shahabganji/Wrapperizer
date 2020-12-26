using System;
using System.ComponentModel.DataAnnotations;
using Wrapperizer.Domain.Abstraction.Cqrs;

namespace Wrapperizer.Sample.Domain.Commands
{
    public sealed class RegisterStudent : ICommand<Guid>
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string NationalCode { get; set; }
        [Required]
        public DateTimeOffset Birthdate { get; set; }

        // public RegisterStudent()
        // {
        //     
        // }
        //
        // public RegisterStudent(string firstName, string lastName, string nationalCode, DateTimeOffset birthdate)
        // {
        //     FirstName = firstName;
        //     LastName = lastName;
        //     NationalCode = nationalCode;
        //     Birthdate = birthdate;
        // }
    }
}
