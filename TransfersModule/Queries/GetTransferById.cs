using System.Threading;
using System.Threading.Tasks;
using MediatR;
using MongoDB.Driver;
using TransfersService.Commands;
using TransfersService.Contract;
using TransfersService.Persistence;
using TransfersService.Persistence.Entities;
using TransferState = TransfersService.Contract.Shared.TransferState;

namespace TransfersService.Queries
{
    internal class GetTransferByIdQuery : IRequestHandler<GetTransferByIdContract.Request, GetTransferByIdContract.Response>
    {
        private readonly TransferRepository _db;

        public GetTransferByIdQuery(TransferRepository db)
        {
            _db = db;
        }

        public async Task<GetTransferByIdContract.Response> Handle(GetTransferByIdContract.Request request, CancellationToken ct)
        {
            var filter = Builders<Transfer>.Filter.Eq(x => x.Id, request.Id);
            var transfer = await _db.Query().Find(filter).FirstAsync(ct);

            return Map(transfer);
        }

        private static GetTransferByIdContract.Response Map(Transfer transfer)
        {
            return new GetTransferByIdContract.Response()
            {
                Id = transfer.Id,
                EngagingClubId = transfer.EngagingClubId,
                ReleasingClubId = transfer.ReleasingClubId,
                PlayerId = transfer.PlayerId,
                State = (TransferState)transfer.State,
                PlayersContract = PlayerContractMapper.Map(transfer.PlayersContract),
            };
        }
    }
}
