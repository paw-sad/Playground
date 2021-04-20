﻿using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Playground;
using Playground.Contract;
using Playground.Persistence;
using Shouldly;

namespace Tests
{
    [TestClass]
    public class EngageReleasePlayerTests
    {
        [TestInitialize]
        public void Setup()
        {
            var db = new TestDbContext();
            db.Database.ExecuteSqlRaw("DELETE TransferInstructions");
            db.Database.ExecuteSqlRaw("DELETE Transfers");
        }

        [TestMethod]
        public void CanEngageWithTransferAgreement()
        {
            // given new request with payments equal 0
            var request = new EngageWithTransferAgreementRequest
            {
                ReleasingClubId = 1,
                EngagingClubId = 2,
                PlayerId = 1,
                PaymentsAmount = 0,
                TransferDate = new DateTime(2020, 01, 01)
            };
            
            // when calling engage
            var response = TransfersAPI.EngageWithTransferAgreement(request);

            // then i should get new engaging transfer instruction
            var transferInstruction = TransfersAPI.GetTransferInstructionById(new GetTransferInstructionByIdRequest
            {
                Id = response.TransferInstructionId
            });

            transferInstruction.Type.ShouldBe(TransferInstructionType.Engaging);
        }

        [TestMethod]
        public void CanReleaseWithTransferAgreement()
        {
            // given new request with payments equal 0
            var request = new ReleasePlayerRequest
            {
                ReleasingClubId = 1,
                EngagingClubId = 2,
                PlayerId = 1,
                PaymentsAmount = 0,
                TransferDate = new DateTime(2020, 01, 01)
            };

            // when calling engage
            var response = TransfersAPI.ReleasePlayer(request);

            // then i should get new engaging transfer instruction
            var transferInstruction = TransfersAPI.GetTransferInstructionById(new GetTransferInstructionByIdRequest
            {
                Id = response.TransferInstructionId
            });

            transferInstruction.Type.ShouldBe(TransferInstructionType.Releasing);
        }

        [TestMethod]
        public void CanPairReleasingInstructionAndCreateTransfer()
        {
            var engageRequest = new EngageWithTransferAgreementRequest
            {
                ReleasingClubId = 1,
                EngagingClubId = 2,
                PlayerId = 1,
                PaymentsAmount = 0,
                TransferDate = new DateTime(2020, 01, 01)
            };

            var engageResponse = TransfersAPI.EngageWithTransferAgreement(engageRequest);
            var releaseRequest = new ReleasePlayerRequest
            {
                ReleasingClubId = 1,
                EngagingClubId = 2,
                PlayerId = 1,   
                PaymentsAmount = 0,
                TransferDate = new DateTime(2020, 01, 01)
            };
            var releaseResponse = TransfersAPI.ReleasePlayer(releaseRequest);

            var engageInstruction = TransfersAPI.GetTransferInstructionById(new GetTransferInstructionByIdRequest
            {
                Id = engageResponse.TransferInstructionId
            });
            engageInstruction.TransferId.ShouldNotBeNull();

            var releaseInstruction = TransfersAPI.GetTransferInstructionById(new GetTransferInstructionByIdRequest
            {
                Id = releaseResponse.TransferInstructionId
            });

            releaseInstruction.TransferId.ShouldNotBeNull();
        }

        [TestMethod]
        public void CanPairEngagingInstructionAndCreateTransfer()
        {
            var releaseRequest = new ReleasePlayerRequest
            {
                ReleasingClubId = 1,
                EngagingClubId = 2,
                PlayerId = 1,
                PaymentsAmount = 0,
                TransferDate = new DateTime(2020, 01, 01)
            };
            var releaseResponse = TransfersAPI.ReleasePlayer(releaseRequest);

            // given new request with payments equal 0
            var engageRequest = new EngageWithTransferAgreementRequest
            {
                ReleasingClubId = 1,
                EngagingClubId = 2,
                PlayerId = 1,
                PaymentsAmount = 0,
                TransferDate = new DateTime(2020, 01, 01)
            };

            var engageResponse = TransfersAPI.EngageWithTransferAgreement(engageRequest);

            var engageInstruction = TransfersAPI.GetTransferInstructionById(new GetTransferInstructionByIdRequest
            {
                Id = engageResponse.TransferInstructionId
            });
            engageInstruction.TransferId.ShouldNotBeNull();

            var releaseInstruction = TransfersAPI.GetTransferInstructionById(new GetTransferInstructionByIdRequest
            {
                Id = releaseResponse.TransferInstructionId
            });

            releaseInstruction.TransferId.ShouldNotBeNull();
        }    
        
        [TestMethod]
        public void DontPairTwoEngagingInstructions()
        {
            var engageRequest = new EngageWithTransferAgreementRequest
            {
                ReleasingClubId = 1,
                EngagingClubId = 2,
                PlayerId = 1,
                PaymentsAmount = 0,
                TransferDate = new DateTime(2020, 01, 01)
            };

            var engageResponse1 = TransfersAPI.EngageWithTransferAgreement(engageRequest);

            var engageInstruction1 = TransfersAPI.GetTransferInstructionById(new GetTransferInstructionByIdRequest
            {
                Id = engageResponse1.TransferInstructionId
            });

            var engageResponse2 = TransfersAPI.EngageWithTransferAgreement(engageRequest);

            var engageInstruction2 = TransfersAPI.GetTransferInstructionById(new GetTransferInstructionByIdRequest
            {
                Id = engageResponse2.TransferInstructionId
            });

            engageInstruction1.TransferId.ShouldBeNull();
            engageInstruction2.TransferId.ShouldBeNull();
        }

        [TestMethod]
        public void WhenMoreEngagingInstructionsAreAvailableShouldPairWithOldest()
        {
            var engageRequest = new EngageWithTransferAgreementRequest
            {
                ReleasingClubId = 1,
                EngagingClubId = 2,
                PlayerId = 1,
                PaymentsAmount = 0,
                TransferDate = new DateTime(2020, 01, 01)
            };

            var engageResponse1 = TransfersAPI.EngageWithTransferAgreement(engageRequest);

            var engageResponse2 = TransfersAPI.EngageWithTransferAgreement(engageRequest);

            var releaseRequest = new ReleasePlayerRequest
            {
                ReleasingClubId = 1,
                EngagingClubId = 2,
                PlayerId = 1,
                PaymentsAmount = 0,
                TransferDate = new DateTime(2020, 01, 01)
            };
            var releaseResponse = TransfersAPI.ReleasePlayer(releaseRequest);

            var engageInstruction1 = TransfersAPI.GetTransferInstructionById(new GetTransferInstructionByIdRequest
            {
                Id = engageResponse1.TransferInstructionId
            });

            var engageInstruction2 = TransfersAPI.GetTransferInstructionById(new GetTransferInstructionByIdRequest
            {
                Id = engageResponse2.TransferInstructionId
            });

            var releaseInstruction = TransfersAPI.GetTransferInstructionById(new GetTransferInstructionByIdRequest
            {
                Id = releaseResponse.TransferInstructionId
            });

            engageInstruction1.TransferId.ShouldNotBeNull();
            engageInstruction2.TransferId.ShouldBeNull();
            releaseInstruction.TransferId.ShouldNotBeNull();
        }
        
        [TestMethod]
        public void WhenMoreReleasingInstructionsAreAvailableShouldPairWithOldest()
        {
          var releaseRequest = new ReleasePlayerRequest
            {
                ReleasingClubId = 1,
                EngagingClubId = 2,
                PlayerId = 1,
                PaymentsAmount = 0,
                TransferDate = new DateTime(2020, 01, 01)
            };
            var releaseResponse1 = TransfersAPI.ReleasePlayer(releaseRequest);
            var releaseResponse2 = TransfersAPI.ReleasePlayer(releaseRequest);
            var engageRequest = new EngageWithTransferAgreementRequest
            {
                ReleasingClubId = 1,
                EngagingClubId = 2,
                PlayerId = 1,
                PaymentsAmount = 0,
                TransferDate = new DateTime(2020, 01, 01)
            };

            var engageResponse = TransfersAPI.EngageWithTransferAgreement(engageRequest);
            var engageInstruction = TransfersAPI.GetTransferInstructionById(new GetTransferInstructionByIdRequest
            {
                Id = engageResponse.TransferInstructionId
            });

            var releaseInstruction1 = TransfersAPI.GetTransferInstructionById(new GetTransferInstructionByIdRequest
            {
                Id = releaseResponse1.TransferInstructionId
            });
            var releaseInstruction2 = TransfersAPI.GetTransferInstructionById(new GetTransferInstructionByIdRequest
            {
                Id = releaseResponse2.TransferInstructionId
            });

            releaseInstruction1.TransferId.ShouldNotBeNull();
            releaseInstruction2.TransferId.ShouldBeNull();
            engageInstruction.TransferId.ShouldNotBeNull();
        }
    }
}
