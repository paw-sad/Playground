using System;

namespace TransfersService.Domain
{
    public class PlayersContract
    {
        public DateTime EmploymentContractStart { get; set; }
        public DateTime EmploymentContractEnd { get; set; }
        public decimal Salary { get; set; }
    }
}