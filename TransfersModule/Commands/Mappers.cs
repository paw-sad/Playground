using System;
using System.Linq;
using TransfersModule.Persistence;

namespace TransfersModule.Commands
{
    internal static class PlayerContractMapper
    {
        public static PlayersContract Map(Contract.Shared.PlayersContract contract) =>
            new PlayersContract
            {
                EmploymentContractStart = contract.EmploymentContractStart,
                EmploymentContractEnd = contract.EmploymentContractEnd,
                Salary = MapSalary(contract.Salary)
            };  
        
        public static Contract.Shared.PlayersContract Map(PlayersContract contract) =>
            new Contract.Shared.PlayersContract
            {
                EmploymentContractStart = contract.EmploymentContractStart,
                EmploymentContractEnd = contract.EmploymentContractEnd,
                Salary = MapSalary(contract.Salary)
            };

        private static ISalary MapSalary(Contract.Shared.ISalary contractSalary)
        {
            switch (contractSalary)
            {
                case Contract.Shared.NoSalary noSalary:
                    return new NoSalary();
                case Contract.Shared.PlayersSalaryIrregular irregularSalary:
                    return Map(irregularSalary);
                case Contract.Shared.PlayersSalaryRegular regularSalary:
                    return Map(regularSalary);
                default:
                    throw new ArgumentException("unknown salary type");
            }
        }

        private static Contract.Shared.ISalary MapSalary(ISalary contractSalary)
        {
            switch (contractSalary)
            {
                case NoSalary noSalary:
                    return new Contract.Shared.NoSalary();
                case PlayersSalaryIrregular irregularSalary:
                    return Map(irregularSalary);
                case PlayersSalaryRegular regularSalary:
                    return Map(regularSalary);
                default:
                    throw new ArgumentException("unknown salary type");
            }
        }

        private static PlayersSalaryIrregular Map(Contract.Shared.PlayersSalaryIrregular irregularSalary) =>
            new PlayersSalaryIrregular
            {
                SalaryPeriods = irregularSalary.SalaryPeriods.Select(Map).ToList()
            };       
        
        private static Contract.Shared.PlayersSalaryIrregular Map(PlayersSalaryIrregular irregularSalary) =>
            new Contract.Shared.PlayersSalaryIrregular
            {
                SalaryPeriods = irregularSalary.SalaryPeriods.Select(Map).ToList()
            };

        private static IrregularSalaryPeriod Map(Contract.Shared.IrregularSalaryPeriod irregularSalary) =>
            new IrregularSalaryPeriod
            {
                PeriodStart = irregularSalary.PeriodStart,
                PeriodEnd = irregularSalary.PeriodEnd,
                SalaryAmount = irregularSalary.SalaryAmount,
                CurrencyCode = irregularSalary.CurrencyCode
            };      
        
        private static Contract.Shared.IrregularSalaryPeriod Map(IrregularSalaryPeriod irregularSalary) =>
            new Contract.Shared.IrregularSalaryPeriod
            {
                PeriodStart = irregularSalary.PeriodStart,
                PeriodEnd = irregularSalary.PeriodEnd,
                SalaryAmount = irregularSalary.SalaryAmount,
                CurrencyCode = irregularSalary.CurrencyCode
            };

        private static PlayersSalaryRegular Map(Contract.Shared.PlayersSalaryRegular irregularSalary) =>
            new PlayersSalaryRegular
            {
                SalaryAmount = irregularSalary.SalaryAmount,
                CurrencyCode = irregularSalary.CurrencyCode,
                SalaryInterval = (SalaryInterval)irregularSalary.SalaryInterval
            };

        private static Contract.Shared.PlayersSalaryRegular Map(PlayersSalaryRegular irregularSalary) =>
            new Contract.Shared.PlayersSalaryRegular
            {
                SalaryAmount = irregularSalary.SalaryAmount,
                CurrencyCode = irregularSalary.CurrencyCode,
                SalaryInterval = (Contract.Shared.SalaryInterval)irregularSalary.SalaryInterval
            };
    }
}