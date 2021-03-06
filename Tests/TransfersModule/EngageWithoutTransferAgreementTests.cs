using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using TransfersService;
using TransfersService.Contract;
using TransfersService.Contract.Shared;

namespace Tests.TransfersModule
{
    [TestClass]
    public class EngageWithoutTransferAgreementTests
    {
        [TestMethod]
        public async Task TestTransferWithNoPayments()
        {
            var request = new EngageWithoutTransferAgreement.Request
            {
                ReleasingClubId = 1,
                EngagingClubId = 2,
                PlayerId = 1,
                PlayersContract = new PlayersContract
                {
                    EmploymentContractStart = new DateTime(2020, 01, 01),
                    EmploymentContractEnd = new DateTime(2021, 01, 01),
                    Salary = 999
                }
            };

            var api = new TransfersApi();
            // when engaging player without transfer agreement and no payments
            var response = await api.Execute(request);

            // then i should get new draft transfer
            var transfer = await api.Execute(new GetTransferByIdContract.Request
            {
                Id = response.TransferId
            });

            transfer.State.ShouldBe(TransferState.Draft);
        }
    }
}
