using System;

namespace TransfersModule.Persistence
{
    internal class IrregularSalaryPeriod
    {
        public DateTime PeriodStart { get; set; }
        public DateTime PeriodEnd { get; set; }
        public decimal SalaryAmount { get; set; }
        public string CurrencyCode { get; set; }
    }
}