using System;

namespace TransfersModule.Persistence
{
    internal class IrregularSalaryPeriod : IEquatable<IrregularSalaryPeriod>
    {
        public DateTime PeriodStart { get; set; }
        public DateTime PeriodEnd { get; set; }
        public decimal SalaryAmount { get; set; }
        public string CurrencyCode { get; set; }

        public bool Equals(IrregularSalaryPeriod other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return PeriodStart.Equals(other.PeriodStart) && PeriodEnd.Equals(other.PeriodEnd) && SalaryAmount == other.SalaryAmount && CurrencyCode == other.CurrencyCode;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((IrregularSalaryPeriod) obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(PeriodStart, PeriodEnd, SalaryAmount, CurrencyCode);
        }
    }
}