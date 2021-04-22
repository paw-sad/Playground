using System;
using TransfersModule.Persistence;

namespace TransfersModule.Contract
{
    public class GetTransferInstructionByIdRequest
    {
        public Guid Id { get; set; }
    }

    public class GetTransferInstructionByIdResponse
    {
        public Guid Id { get; set; }
        public int EngagingClubId { get; set; }
        public int ReleasingClubId { get; set; }
        public int PlayerId { get; set; }
        public decimal PaymentsAmount { get; set; }
        public DateTime TransferDate { get; set; }
        public TransferInstructionType Type { get; set; }
        public Guid? TransferId { get; set; }
    }
}
