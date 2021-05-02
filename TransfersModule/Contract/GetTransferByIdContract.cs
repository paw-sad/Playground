using System;
using MediatR;
using TransfersModule.Contract.Shared;

namespace TransfersModule.Contract
{
    public class GetTransferByIdContract
    {
        public class Request : IRequest<Response>
        {
            public Guid Id { get; set; }
        }

        public class Response
        {
            public Guid Id { get; set; }
            public int EngagingClubId { get; set; }
            public int ReleasingClubId { get; set; }
            public int PlayerId { get; set; }
            public TransferState State { get; set; }
            public PlayersContract PlayersContract { get; set; }
        }
    }
}
