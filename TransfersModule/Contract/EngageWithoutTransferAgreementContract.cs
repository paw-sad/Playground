using System;
using MediatR;
using TransfersModule.Contract.Shared;

namespace TransfersModule.Contract
{
    public class EngageWithoutTransferAgreement
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
            public Guid TransferId { get; set; }
        }
    }
}