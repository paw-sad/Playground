namespace TransfersModule.Contract.Shared
{
    public class PlayersSalaryRegular: ISalary
    {
        public SalaryInterval SalaryInterval { get; set; }
        public decimal SalaryAmount { get; set; }
        public string CurrencyCode { get; set; }
    }
}