using System;

namespace TransfersModule.Contract.Shared
{
    public class PlayersContract: IEquatable<PlayersContract>
    {
        public DateTime EmploymentContractStart { get; set; }
        public DateTime EmploymentContractEnd { get; set; }
        public ISalary Salary { get; set; }

        public bool Equals(PlayersContract other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return EmploymentContractStart.Equals(other.EmploymentContractStart) && EmploymentContractEnd.Equals(other.EmploymentContractEnd) && Equals(Salary, other.Salary);
        }
    }
}