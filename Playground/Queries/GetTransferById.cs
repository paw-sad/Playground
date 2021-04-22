﻿using TransfersModule.Contract;
using TransfersModule.Persistence;

namespace TransfersModule.Queries
{
    internal class GetTransferByIdQuery
    {
        private readonly AppDbContext _db;

        public GetTransferByIdQuery(AppDbContext db)
        {
            _db = db;
        }

        public GetTransferByIdResponse Handle(GetTransferByIdRequest request)
        {
            var transfer = _db.Transfers.Find(request.Id);

            return Map(transfer);
        }

        private static GetTransferByIdResponse Map(Transfer transfer)
        {
            return new GetTransferByIdResponse
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