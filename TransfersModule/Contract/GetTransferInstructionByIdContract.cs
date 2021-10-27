using System;
using MediatR;
using TransfersService.Contract.Shared;
using PlayersContract = TransfersService.Contract.Shared.PlayersContract;

namespace TransfersService.Contract
{
  public class GetTransferInstructionByIdContract : IRequest<GetTransferByIdContract.Response>
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
            public PlayersContract PlayersContract { get; set; }
        }
    }
}
