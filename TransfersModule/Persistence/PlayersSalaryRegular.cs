namespace TransfersModule.Persistence
{
    internal class PlayersSalaryRegular: ISalary
    {
        public SalaryInterval SalaryInterval { get; set; }
        public decimal SalaryAmount { get; set; }
        public string CurrencyCode { get; set; }

        public bool Equals(ISalary other)
        {
            if (other is PlayersSalaryRegular regular)
            {
                return SalaryInterval == regular.SalaryInterval && SalaryAmount == regular.SalaryAmount && CurrencyCode == regular.CurrencyCode;
            }

            return false;
        }
    }
}