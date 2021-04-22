using System;

namespace PublicApi.Contract
{
    public class ReleasePlayerRequest
    {
        public int EngagingClubId { get; set; }
        public int ReleasingClubId { get; set; }
        public int PlayerId { get; set; }
        public DateTime TransferDate { get; set; }
        public decimal PaymentsAmount { get; set; }
    }

    public class ReleasePlayerResponse
    {
        public Guid TransferInstructionId { get; set; }
    }
}