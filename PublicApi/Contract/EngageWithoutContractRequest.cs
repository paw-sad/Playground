using System;

namespace PublicApi.Contract
{
    public class EngageWithoutTransferAgreementRequest
    {
        public int EngagingClubId { get; set; }
        public int ReleasingClubId { get; set; }
        public int PlayerId { get; set; }
        public DateTime TransferDate { get; set; }
        public decimal PaymentsAmount { get; set; }
    }

    public class EngageWithoutTransferAgreementResponse
    {
        public Guid TransferId { get; set; }
    }
}