using System.Collections.Generic;

namespace TransfersModule.Contract.Shared
{
    public class PlayersSalaryIrregular : ISalary
    {
        public IEnumerable<IrregularSalaryPeriod> SalaryPeriods { get; set; }
    }
}