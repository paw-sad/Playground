using System;

namespace TransfersModule.Contract.Shared
{
    public class PlayersContract
    {
        public DateTime EmploymentContractStart { get; set; }
        public DateTime EmploymentContractEnd { get; set; }
        public ISalary Salary { get; set; }
    }
}