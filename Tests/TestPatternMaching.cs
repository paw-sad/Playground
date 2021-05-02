using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TransfersModule.Contract.Shared;

namespace Tests
{
    [TestClass]
    public class TestPatternMaching
    {
        [TestMethod]
        public void Test()
        {
            var contract = new PlayersContract()
            {
                Salary = new PlayersSalaryIrregular()
            };

            switch (contract.Salary)
            {
                case PlayersSalaryRegular regularSalary:
                    Assert.Fail();
                    break;
                case IrregularSalaryPeriod irregular:
                    Assert.IsTrue(true);
                    break;
                default:
                    throw new ArgumentException("unknow salary type");
            }
        }
    }
}
