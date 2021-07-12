using System;

namespace TransfersModule.Persistence
{
    public class PlayersContract
    {
        public DateTime EmploymentContractStart { get; set; }
        public DateTime EmploymentContractEnd { get; set; }
        public ISalary Salary { get; set; }
    }
}