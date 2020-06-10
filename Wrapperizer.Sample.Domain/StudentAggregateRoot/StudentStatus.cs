using Wrapperizer.Abstraction.Domain;

namespace Wrapperizer.Sample.Domain.StudentAggregateRoot
{
    public class StudentStatus : Enumeration
    {
        public static readonly StudentStatus Registered = new StudentStatus(1, nameof(Registered));
        public static readonly StudentStatus
            Confirmed = new StudentStatus(2, nameof(Confirmed));

        public StudentStatus(int id, string name) : base(id, name)
        {
        }
        
    }
}
