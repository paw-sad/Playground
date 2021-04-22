using System;
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
        public void TestTransferWithNoPayments()
        {
            var request = new EngageWithoutTransferAgreementRequest
            {
                PaymentsAmount = 0,
                ReleasingClubId = 1,
                EngagingClubId = 2,
                PlayerId = 1,
                TransferDate = new DateTime(2020, 01, 01)
            };

            // when engaging player without transfer agreement and no payments
            var response = Api.EngageWithoutTransferAgreement(request);

            // then i should get new completed transfer
            var transfer = Api.GetTransferById(new GetTransferByIdRequest
            {
                Id = response.TransferId
            });

            transfer.State.ShouldBe(TransferState.Completed);
        }
    }
}
