using System;
using System.Collections.Generic;
using System.Text;

namespace TransfersModule.Persistence
{
    internal class TransferWithoutTransferAgreement
    {
        public int EngagingClubId { get; set; }
        public int ReleasingClubId { get; set; }
        public int PlayerId { get; set; }

        internal class PlayersContract
        {
            public DateTime EmploymentContractStart { get; set; }
            public DateTime EmploymentContractEnd { get; set; }
            public PlayersSalaryRegular PlayersSalaryRegular { get; set; }
            public PlayersSalaryIrregular PlayersSalaryIrregular { get; set; }
        }

        internal class PlayersSalaryRegular
        {
            public SalaryInterval SalaryInterval { get; set; }
            public decimal SalaryAmount { get; set; }
            public string CurrencyCode { get; set; }
        }

        internal class PlayersSalaryIrregular
        {
            public IEnumerable<IrregularSalaryPeriod> SalaryPeriods { get; set; }
        }

        internal class IrregularSalaryPeriod
        {
            public DateTime PeriodStart { get; set; }
            public DateTime PeriodEnd { get; set; }
            public decimal SalaryAmount { get; set; }
            public string CurrencyCode { get; set; }
        }

        internal enum SalaryInterval
        {
            Weekly,
            Monthly,
            Yearly
        }
    }
}
