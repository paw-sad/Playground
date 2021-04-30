using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Driver;
using TransfersModule;
using TransfersModule.Contract;
using TransfersModule.Persistence;
using Shouldly;

namespace Tests.TransfersModule
{
    [TestClass]
    public class EngageReleasePlayerTests
    {
        private readonly TransfersApi _api = new TransfersApi();

        [TestInitialize]
        public async Task Setup()
        {
            var db = new MongoClient("mongodb://localhost:27017");
            await db.GetDatabase("transfers-module").DropCollectionAsync("transfers");
            await db.GetDatabase("transfers-module").DropCollectionAsync("transfer-instructions");
        }

        [TestMethod]
        public async Task CanEngageWithTransferAgreement()
        {
            // given new request with payments equal 0
            var request = new EngageWithTransferAgreementContract.Request
            {
                ReleasingClubId = 1,
                EngagingClubId = 2,
                PlayerId = 1,
                PaymentsAmount = 0,
                TransferDate = new DateTime(2020, 01, 01)
            };

            // when calling engage
            var response = await _api.Execute(request);

            // then i should get new engaging transfer instruction
            var transferInstruction = await _api.Execute(new GetTransferInstructionByIdContract.Request
            {
                Id = response.TransferInstructionId.Value
            });

            transferInstruction.Type.ShouldBe(TransferInstructionType.Engaging);
        }

        [TestMethod]
        public async Task CanReleaseWithTransferAgreement()
        {
            // given new request with payments equal 0
            var request = new ReleasePlayerContract.Request
            {
                ReleasingClubId = 1,
                EngagingClubId = 2,
                PlayerId = 1,
                PaymentsAmount = 0,
                TransferDate = new DateTime(2020, 01, 01)
            };

            // when calling engage
            var response = await _api.Execute(request);

            // then i should get new engaging transfer instruction
            var transferInstruction = await _api.Execute(new GetTransferInstructionByIdContract.Request
            {
                Id = response.TransferInstructionId.Value
            });

            transferInstruction.Type.ShouldBe(TransferInstructionType.Releasing);
        }

        [TestMethod]
        public async Task CanPairReleasingInstructionAndCreateTransfer()
        {
            var engageRequest = new EngageWithTransferAgreementContract.Request
            {
                ReleasingClubId = 1,
                EngagingClubId = 2,
                PlayerId = 1,
                PaymentsAmount = 0,
                TransferDate = new DateTime(2020, 01, 01)
            };

            await _api.Execute(engageRequest);
            var releaseRequest = new ReleasePlayerContract.Request
            {
                ReleasingClubId = 1,
                EngagingClubId = 2,
                PlayerId = 1,
                PaymentsAmount = 0,
                TransferDate = new DateTime(2020, 01, 01)
            };
            var releaseResponse = await _api.Execute(releaseRequest);
            releaseResponse.TransferId.ShouldNotBeNull();
        }

        [TestMethod]
        public async Task CanPairEngagingInstructionAndCreateTransfer()
        {
            var releaseRequest = new ReleasePlayerContract.Request
            {
                ReleasingClubId = 1,
                EngagingClubId = 2,
                PlayerId = 1,
                PaymentsAmount = 0,
                TransferDate = new DateTime(2020, 01, 01)
            };
            var releaseResponse = await _api.Execute(releaseRequest);

            // given new request with payments equal 0
            var engageRequest = new EngageWithTransferAgreementContract.Request
            {
                ReleasingClubId = 1,
                EngagingClubId = 2,
                PlayerId = 1,
                PaymentsAmount = 0,
                TransferDate = new DateTime(2020, 01, 01)
            };

            var engageResponse = await _api.Execute(engageRequest);
            engageResponse.TransferId.ShouldNotBeNull();

            engageResponse.TransferInstructionId.ShouldBeNull();
        }

        [TestMethod]
        public async Task DontPairTwoEngagingInstructions()
        {
            var engageRequest = new EngageWithTransferAgreementContract.Request
            {
                ReleasingClubId = 1,
                EngagingClubId = 2,
                PlayerId = 1,
                PaymentsAmount = 0,
                TransferDate = new DateTime(2020, 01, 01)
            };

            var engageResponse1 = await _api.Execute(engageRequest);

            var engageInstruction1 = await _api.Execute(new GetTransferInstructionByIdContract.Request
            {
                Id = engageResponse1.TransferInstructionId.Value
            });

            var engageResponse2 = await _api.Execute(engageRequest);

            var engageInstruction2 = await _api.Execute(new GetTransferInstructionByIdContract.Request
            {
                Id = engageResponse2.TransferInstructionId.Value
            });
            engageInstruction1.ShouldNotBeNull();
            engageInstruction2.ShouldNotBeNull();
            engageResponse1.TransferId.ShouldBeNull();
            engageResponse2.TransferId.ShouldBeNull();
        }

        [TestMethod]
        public async Task WhenMoreEngagingInstructionsAreAvailableShouldPairWithOldest()
        {
            var engageRequest = new EngageWithTransferAgreementContract.Request
            {
                ReleasingClubId = 1,
                EngagingClubId = 2,
                PlayerId = 1,
                PaymentsAmount = 0,
                TransferDate = new DateTime(2020, 01, 01)
            };

            var engageResponse1 = await _api.Execute(engageRequest);

            var engageResponse2 = await _api.Execute(engageRequest);

            var releaseRequest = new ReleasePlayerContract.Request
            {
                ReleasingClubId = 1,
                EngagingClubId = 2,
                PlayerId = 1,
                PaymentsAmount = 0,
                TransferDate = new DateTime(2020, 01, 01)
            };

            var releaseResponse = await _api.Execute(releaseRequest);

            var engageInstruction1 = await _api.Execute(new GetTransferInstructionByIdContract.Request
            {
                Id = engageResponse1.TransferInstructionId.Value
            });

            var engageInstruction2 = await _api.Execute(new GetTransferInstructionByIdContract.Request
            {
                Id = engageResponse2.TransferInstructionId.Value
            });

        
            releaseResponse.TransferId.ShouldNotBeNull();
            releaseResponse.TransferInstructionId.ShouldBeNull();
            engageInstruction1.ShouldBeNull();
            engageInstruction2.ShouldNotBeNull();

            releaseResponse.TransferInstructionId.ShouldBeNull();
            releaseResponse.TransferId.ShouldNotBeNull();
        }

        [TestMethod]
        public async Task WhenMoreReleasingInstructionsAreAvailableShouldPairWithOldest()
        {
            var releaseRequest = new ReleasePlayerContract.Request
            {
                ReleasingClubId = 1,
                EngagingClubId = 2,
                PlayerId = 1,
                PaymentsAmount = 0,
                TransferDate = new DateTime(2020, 01, 01)
            };
            var releaseResponse1 = await _api.Execute(releaseRequest);
            var releaseResponse2 = await _api.Execute(releaseRequest);

            var engageRequest = new EngageWithTransferAgreementContract.Request
            {
                ReleasingClubId = 1,
                EngagingClubId = 2,
                PlayerId = 1,
                PaymentsAmount = 0,
                TransferDate = new DateTime(2020, 01, 01)
            };

            var engageResponse = await _api.Execute(engageRequest);
      

            var releaseInstruction1 = await _api.Execute(new GetTransferInstructionByIdContract.Request
            {
                Id = releaseResponse1.TransferInstructionId.Value
            });
            var releaseInstruction2 = await _api.Execute(new GetTransferInstructionByIdContract.Request
            {
                Id = releaseResponse2.TransferInstructionId.Value
            });

            engageResponse.TransferId.ShouldNotBeNull();
            engageResponse.TransferInstructionId.ShouldBeNull();

            releaseInstruction1.ShouldBeNull();
            releaseInstruction2.ShouldNotBeNull();
        }
    }
}
