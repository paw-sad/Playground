using TransfersService.Persistence.Entities;

namespace TransfersService.Commands
{
    internal static class PlayerContractMapper
    {
        public static PlayersContract Map(Contract.Shared.PlayersContract contract) =>
            new PlayersContract
            {
                EmploymentContractStart = contract.EmploymentContractStart,
                EmploymentContractEnd = contract.EmploymentContractEnd,
                Salary = contract.Salary
            };

        public static Contract.Shared.PlayersContract Map(PlayersContract contract) =>
            new Contract.Shared.PlayersContract
            {
                EmploymentContractStart = contract.EmploymentContractStart,
                EmploymentContractEnd = contract.EmploymentContractEnd,
                Salary = contract.Salary
            };
    }
}