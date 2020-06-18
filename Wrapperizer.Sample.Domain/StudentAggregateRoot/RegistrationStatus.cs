using System;
using Funx.Either;
using Wrapperizer.Abstraction.Domain;

namespace Wrapperizer.Sample.Domain.StudentAggregateRoot
{
    public class RegistrationStatus : Enumeration , IEquatable<RegistrationStatus>
    {
        public static readonly RegistrationStatus Requested = new RegistrationStatus(1, nameof(Requested));
        public static readonly RegistrationStatus Confirmed = new RegistrationStatus(2, nameof(Confirmed));

        public RegistrationStatus(int id, string name) : base(id, name)
        {
        }

        public bool Equals(RegistrationStatus other)
        {
            if (other == null) return false;

            return this.Id == other.Id;
        }

        public static bool operator ==(RegistrationStatus right, RegistrationStatus left)
            => right != null && right.Equals(left);

        public static bool operator !=(RegistrationStatus right, RegistrationStatus left) => !(right == left);
    }
}
