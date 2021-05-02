using System.Collections.Generic;

namespace TransfersModule.Persistence
{
    internal class PlayersSalaryIrregular : ISalary
    {
        public IEnumerable<IrregularSalaryPeriod> SalaryPeriods { get; set; }
    }
}