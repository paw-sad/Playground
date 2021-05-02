using System;

namespace TransfersModule.Persistence
{
    internal class PlayersContract
    {
        public DateTime EmploymentContractStart { get; set; }
        public DateTime EmploymentContractEnd { get; set; }
        public ISalary Salary { get; set; }
    }
}