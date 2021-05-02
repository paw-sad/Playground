using System;
using System.Collections.Generic;
using System.Linq;

namespace TransfersModule.Persistence
{
    internal class PlayersSalaryIrregular : ISalary
    {
        public IEnumerable<IrregularSalaryPeriod> SalaryPeriods { get; set; }

        public bool Equals(ISalary other)
        {
            switch (other)
            {
                case PlayersSalaryIrregular irregular:
                {
                    if (SalaryPeriods.Count() != irregular.SalaryPeriods.Count())
                    {
                        return false;
                    }

                    return SalaryPeriods.All(sp => irregular.SalaryPeriods.Any(sp2 => sp2.Equals(sp)));
                }
                default:
                    return false;
            }
        }
    }
}