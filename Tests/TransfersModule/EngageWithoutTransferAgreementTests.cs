using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using TransfersModule;
using TransfersModule.Contract;
using TransfersModule.Persistence;

namespace Tests.TransfersModule
{
    [TestClass]
    public class EngageWithoutTransferAgreementTests
    {
        [TestMethod]
        public async Task TestTransferWithNoPayments()
        {
            var request = new EngageWithoutTransferAgreementContract.Request
            {
                ReleasingClubId = 1,
                EngagingClubId = 2,
                PlayerId = 1
            };

            var api = new TransfersApi();
            // when engaging player without transfer agreement and no payments
            var response = await api.Execute(request);

            // then i should get new completed transfer
            var transfer = await api.Execute(new GetTransferByIdContract.Request
            {
                Id = response.TransferId
            });

            transfer.State.ShouldBe(TransferState.Completed);
        }
    }
}
