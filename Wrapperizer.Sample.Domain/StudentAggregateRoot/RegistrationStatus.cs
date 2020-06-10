using Wrapperizer.Abstraction.Domain;

namespace Wrapperizer.Sample.Domain.StudentAggregateRoot
{
    public class RegistrationStatus : Enumeration
    {
        public static readonly RegistrationStatus Requested = new RegistrationStatus(1, nameof(Requested));
        public static readonly RegistrationStatus Confirmed = new RegistrationStatus(2, nameof(Confirmed));

        public RegistrationStatus(int id, string name) : base(id, name)
        {
        }
        
    }
}
