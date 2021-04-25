using System;
using MediatR;

namespace TransfersModule.Contract
{
    public class ReleasePlayerContract
    {
        public class Request : IRequest<Response>
        {
            public int EngagingClubId { get; set; }
            public int ReleasingClubId { get; set; }
            public int PlayerId { get; set; }
            public DateTime TransferDate { get; set; }
            public decimal PaymentsAmount { get; set; }
        }

        public class Response
        {
            public Guid TransferInstructionId { get; set; }
        }
    }
}