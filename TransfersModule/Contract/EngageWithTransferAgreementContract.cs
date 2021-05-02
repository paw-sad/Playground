using System;
using MediatR;
using TransfersModule.Contract.Shared;

namespace TransfersModule.Contract
{
    public class EngageWithTransferAgreementContract
    {
        public class Request : IRequest<Response>
        {
            public int EngagingClubId { get; set; }
            public int ReleasingClubId { get; set; }
            public int PlayerId { get; set; }
            public PlayersContract PlayersContract { get; set; }
        }

        public class Response
        {
            public Guid? TransferInstructionId { get; set; }
            public Guid? TransferId { get; set; }
        }
    }
}
