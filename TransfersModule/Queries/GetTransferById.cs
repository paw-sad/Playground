using System.Threading;
using System.Threading.Tasks;
using MediatR;
using TransfersModule.Contract;
using TransfersModule.Persistence;

namespace TransfersModule.Queries
{
    internal class GetTransferByIdQuery : IRequestHandler<GetTransferByIdContract.Request, GetTransferByIdContract.Response>
    {
        private readonly TransfersDbContext _db;

        public GetTransferByIdQuery(TransfersDbContext db)
        {
            _db = db;
        }

        public async Task<GetTransferByIdContract.Response> Handle(GetTransferByIdContract.Request request, CancellationToken ct)
        {
            var transfer = await _db.Transfers.FindAsync(request.Id);

            return Map(transfer);
        }

        private static GetTransferByIdContract.Response Map(Transfer transfer)
        {
            return new GetTransferByIdContract.Response
            {
                Id = transfer.Id,
                EngagingClubId = transfer.EngagingClubId,
                ReleasingClubId = transfer.ReleasingClubId,
                PlayerId = transfer.PlayerId,
                PaymentsAmount = transfer.PaymentsAmount,
                TransferDate = transfer.TransferDate,
                State = transfer.State
            };
        }
    }
}
