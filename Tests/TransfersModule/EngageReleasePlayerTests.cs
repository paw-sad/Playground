using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Driver;
using TransfersModule;
using TransfersModule.Contract;
using TransfersModule.Persistence;
using Shouldly;
using TransfersModule.Contract.Shared;

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
            var request = GetEngageRequest();

            // when calling engage
            var response = await _api.Execute(request);

            // then i should get new engaging transfer instruction
            var transferInstruction = await _api.Execute(new GetTransferInstructionByIdContract.Request
            {
                Id = response.TransferInstructionId.Value
            });

            transferInstruction.Type.ShouldBe(TransferInstructionType.Engaging);
        }

        private static EngageWithTransferAgreementContract.Request GetEngageRequest()
        {
            // given new request with payments equal 0
            return new EngageWithTransferAgreementContract.Request
            {
                ReleasingClubId = 1,
                EngagingClubId = 2,
                PlayerId = 1,
                PlayersContract = new PlayersContract
                {
                    Salary = new NoSalary(),
                    EmploymentContractStart = new DateTime(2020, 01, 01),
                    EmploymentContractEnd = new DateTime(2021, 01, 01)
                }
            };
        }

        [TestMethod]
        public async Task CanReleaseWithTransferAgreement()
        {
            // given new request with payments equal 0
            var request = GetReleaseRequest();

            // when calling engage
            var response = await _api.Execute(request);

            // then i should get new engaging transfer instruction
            var transferInstruction = await _api.Execute(new GetTransferInstructionByIdContract.Request
            {
                Id = response.TransferInstructionId.Value
            });

            transferInstruction.Type.ShouldBe(TransferInstructionType.Releasing);
        }

        private static ReleasePlayerContract.Request GetReleaseRequest()
        {
            return new ReleasePlayerContract.Request
            {
                ReleasingClubId = 1,
                EngagingClubId = 2,
                PlayerId = 1,
                PlayersContract = new PlayersContract
                {
                    Salary = new NoSalary(),
                    EmploymentContractStart = new DateTime(2020, 01, 01),
                    EmploymentContractEnd = new DateTime(2021, 01, 01)
                }
            };
        }

        [TestMethod]
        public async Task CanPairReleasingInstructionAndCreateTransfer()
        {
            var engageRequest = GetEngageRequest();

            await _api.Execute(engageRequest);
            var releaseRequest = GetReleaseRequest();
            var releaseResponse = await _api.Execute(releaseRequest);
            releaseResponse.TransferId.ShouldNotBeNull();
        }

        [TestMethod]
        public async Task CanPairEngagingInstructionAndCreateTransfer()
        {
            var releaseRequest = GetReleaseRequest();
            var releaseResponse = await _api.Execute(releaseRequest);

            // given new request with payments equal 0
            var engageRequest = GetEngageRequest();
            var engageResponse = await _api.Execute(engageRequest);
            engageResponse.TransferId.ShouldNotBeNull();

            engageResponse.TransferInstructionId.ShouldBeNull();
        }

        [TestMethod]
        public async Task DontPairTwoEngagingInstructions()
        {
            var engageRequest = GetEngageRequest();

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
            var engageRequest = GetEngageRequest();

            var engageResponse1 = await _api.Execute(engageRequest);

            var engageResponse2 = await _api.Execute(engageRequest);

            var releaseRequest = GetReleaseRequest();

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
            async Task<(Guid olderInstructionId, Guid newerInstructionId)> 
                GivenTwoSameReleasingInstructions()
            {
                var releaseRequest = GetReleaseRequest();

                var response = await _api.Execute(releaseRequest);
                var response2 = await _api.Execute(releaseRequest);
                return (response.TransferInstructionId.Value, response2.TransferInstructionId.Value);
            }

            async Task OnlyTheOldestInstructionShouldBeMatched(
                (Guid olderInstructionId, Guid newerInstructionId) instructionIds, EngageWithTransferAgreementContract.Response engageResponse)
            {
                var releaseInstruction1 = await _api.Execute(new GetTransferInstructionByIdContract.Request
                {
                    Id = instructionIds.olderInstructionId
                });
                var releaseInstruction2 = await _api.Execute(new GetTransferInstructionByIdContract.Request
                {
                    Id = instructionIds.newerInstructionId
                });

                engageResponse.TransferId.ShouldNotBeNull();
                engageResponse.TransferInstructionId.ShouldBeNull();

                releaseInstruction1.ShouldBeNull();
                releaseInstruction2.ShouldNotBeNull();
            }

            async Task<EngageWithTransferAgreementContract.Response> WhenSubmittingMatchingReleasingInstruction()
            {
                var engageRequest = GetEngageRequest();
                var engageResponse = await _api.Execute(engageRequest);
                return engageResponse;
            }
            
            var instructionIds = await GivenTwoSameReleasingInstructions();
            
            var engageResponse = await WhenSubmittingMatchingReleasingInstruction();
            
            await OnlyTheOldestInstructionShouldBeMatched(instructionIds, engageResponse);
        }

        public async Task Test_instructions_matching_when_there_are_differences_between_engaging_and_releasing_instruction()
        {
            // given engaging instruction was already entered
            // when entering releasing instruction that has only matching ReleasingClubId, EngagingClubId and Player Id
            // then instructions should be match anyway

            var engageRequest = new EngageWithTransferAgreementContract.Request
            {
                ReleasingClubId = 1,
                EngagingClubId = 2,
                PlayerId = 1,
            };

            await _api.Execute(engageRequest);
            var releaseRequest = new ReleasePlayerContract.Request
            {
                ReleasingClubId = 1,
                EngagingClubId = 2,
                PlayerId = 1,
            };
            var releaseResponse = await _api.Execute(releaseRequest);

        }
    }

}
