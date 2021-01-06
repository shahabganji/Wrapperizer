using Wrapperizer.Domain.Abstraction.Domain;

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

    public class Employee
    {
        public EmployeeType Type { get; set; }

        public int CalculateBonus()
        {
            return this.Type.Bonus;
        }
    }

    public class EmployeeType : Enumeration
    {
        public static readonly EmployeeType Manager = new ManagerType();
        public static readonly EmployeeType SimpleMiserableEmployee = new SimpleMiserableEmployeeType();

        public virtual int Bonus { get; }
        public EmployeeType(int id, string name) : base(id, name)
        {
        }
        
        private class ManagerType : EmployeeType
        {
            public ManagerType() : base(1, "Manager")
            {
            }

            public override int Bonus => 1_000_000;
        }
        
        private class SimpleMiserableEmployeeType : EmployeeType
        {
            public SimpleMiserableEmployeeType() : base(1, "Employee")
            {
            }

            public override int Bonus => 1_000;
        }
        
    }
    
}
