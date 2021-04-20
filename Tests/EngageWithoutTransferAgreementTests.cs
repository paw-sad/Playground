using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Playground;
using Playground.Contract;
using Playground.Persistence;
using Shouldly;

namespace Tests
{
    [TestClass]
    public class EngageWithoutTransferAgreementTests
    {
        [TestMethod]
        public void TestTransferWithNoPayments()
        {
            // given new request with payments equal 0
            var request = new EngageWithoutTransferAgreementRequest
            {
                PaymentsAmount = 0,
                ReleasingClubId = 1,
                EngagingClubId = 2,
                PlayerId = 1,
                TransferDate = new DateTime(2020, 01, 01)
            };
            
            // when calling EngageWithoutTransferAgreement
            var response = ProgramAPI.EngageWithoutTransferAgreement(request);

            // then i should get new completed transfer
            var transfer = ProgramAPI.GetTransferById(new GetTransferByIdRequest
            {
                Id = response.TransferId
            });

            transfer.State.ShouldBe(TransferState.Completed);
        }
    }
}
