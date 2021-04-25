using System.Threading;
using System.Threading.Tasks;
using MediatR;
using TransfersModule.Contract;
using TransfersModule.Persistence;

namespace TransfersModule.Queries
{
    internal class GetTransferInstructionByIdQuery : IRequestHandler<GetTransferInstructionByIdContract.Request, GetTransferInstructionByIdContract.Response>
    {
        private readonly TransfersDbContext _db;

        public GetTransferInstructionByIdQuery(TransfersDbContext db)
        {
            _db = db;
        }

        public async Task<GetTransferInstructionByIdContract.Response> Handle(GetTransferInstructionByIdContract.Request request, CancellationToken ct)
        {
            var transferInstruction = await _db.TransferInstructions.FindAsync(request.Id);

            return Map(transferInstruction);
        }

        private static GetTransferInstructionByIdContract.Response Map(TransferInstruction transferInstruction)
        {
            return new GetTransferInstructionByIdContract.Response
            {
                Id = transferInstruction.Id,
                EngagingClubId = transferInstruction.EngagingClubId,
                ReleasingClubId = transferInstruction.ReleasingClubId,
                PlayerId = transferInstruction.PlayerId,
                PaymentsAmount = transferInstruction.PaymentsAmount,
                TransferDate = transferInstruction.TransferDate,
                Type = transferInstruction.Type,
                TransferId = transferInstruction.TransferId
            };
        }
    }
}
