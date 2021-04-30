using System;
using System.Collections.Generic;
using MediatR;

namespace TransfersModule.Contract
{
    public class EngageWithoutTransferAgreementContract
    {
        public class Request: IRequest<Response>
        {
            public int EngagingClubId { get; set; }
            public int ReleasingClubId { get; set; }
            public int PlayerId { get; set; }
            public DateTime TransferDate { get; set; }
            public decimal PaymentsAmount { get; set; }
        }


    //public class Request : IRequest<Response>
//    {
//        public int EngagingClubId { get; set; }
//        public int ReleasingClubId { get; set; }
//        public int PlayerId { get; set; }
//        public PlayersContract PlayersContract { get; set; }
//    }

        public class PlayersContract
        {
            public DateTime EmploymentContractStart { get; set; }
            public DateTime EmploymentContractEnd { get; set; }
            public PlayersSalaryRegular PlayersSalaryRegular { get; set; }
            public PlayersSalaryIrregular PlayersSalaryIrregular { get; set; }
        }

        public class PlayersSalaryRegular
        {
            public SalaryInterval SalaryInterval { get; set; }
            public decimal SalaryAmount { get; set; }
            public string CurrencyCode { get; set; }
        }

        public class PlayersSalaryIrregular
        {
            public IEnumerable<IrregularSalaryPeriod> SalaryPeriods { get; set; }
        }

        public class IrregularSalaryPeriod
        {
            public DateTime PeriodStart { get; set; }
            public DateTime PeriodEnd { get; set; }
            public decimal SalaryAmount { get; set; }
            public string CurrencyCode { get; set; }
        }

        public enum SalaryInterval
        {
            Weekly,
            Monthly,
            Yearly
        }

        public class Response
        {
            public Guid TransferId { get; set; }
        }
    }
}