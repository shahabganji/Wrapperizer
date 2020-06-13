using System;
using System.Linq;
using Wrapperizer.Sample.Domain.Events;
using Wrapperizer.Sample.Domain.StudentAggregateRoot;
using Xunit;
using static Wrapperizer.Sample.Domain.StudentAggregateRoot.RegistrationStatus;

namespace Wrapperizer.Sample.Domain.Test.AggregateRoots
{
    public sealed class Student_Spec
    {

        [Fact]
        public void Ctor_should_create_student_in_registered_state()
        {
            var student = new Student("test" , "full test" , "1234567890" , DateTimeOffset.Now);
            
            Assert.Equal(student.RegistrationStatus , Requested);
        }
        
        [Fact]
        public void Ctor_should_add_StudentRegistered_domain_event()
        {
            var student = new Student("test" , "full test" , "1234567890" , DateTimeOffset.Now);
            
            Assert.NotEmpty(student.DomainEvents);
            Assert.IsType<StudentRegistered>(student.DomainEvents.First());
        }

        [Fact]
        public void ConfirmRegistration_should_change_state_to_Confirmed()
        {
            var student = new Student("test" , "full test" , "1234567890" , DateTimeOffset.Now);
            
            Assert.Equal(student.RegistrationStatus , Requested);
            
            student.ConfirmRegistration();
            
            Assert.Equal(student.RegistrationStatus , Confirmed);
        }
        
    }
}
