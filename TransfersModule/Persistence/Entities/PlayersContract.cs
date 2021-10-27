using System;

namespace TransfersService.Persistence.Entities
{
    public class PlayersContract
    {
        public DateTime EmploymentContractStart { get; set; }
        public DateTime EmploymentContractEnd { get; set; }
        public decimal Salary { get; set; }
    }
}