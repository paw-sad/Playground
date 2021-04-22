using TransfersModule.Contract;
using TransfersModule.Persistence;

namespace TransfersModule.Queries
{
    internal class GetTransferInstructionByIdQuery
    {
        private readonly AppDbContext _db;

        public GetTransferInstructionByIdQuery(AppDbContext db)
        {
            _db = db;
        }

        public GetTransferInstructionByIdResponse Handle(GetTransferInstructionByIdRequest request)
        {
            var transferInstruction = _db.TransferInstructions.Find(request.Id);

            return Map(transferInstruction);
        }

        private static GetTransferInstructionByIdResponse Map(TransferInstruction transferInstruction)
        {
            return new GetTransferInstructionByIdResponse
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
