using System;

namespace TransfersModule.Contract
{
    public class EngageWithTransferAgreementRequest
    {
        public int EngagingClubId { get; set; }
        public int ReleasingClubId { get; set; }
        public int PlayerId { get; set; }
        public DateTime TransferDate { get; set; }
        public decimal PaymentsAmount { get; set; }
    }

    public class EngageWithTransferAgreementResponse
    {
        public Guid TransferInstructionId { get; set; }
    }
}
