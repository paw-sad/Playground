using System;
using TransfersModule.Persistence;

namespace TransfersModule.Events
{
    internal class EngageWithTransferAgreementInstructionCreatedEvent
    {
        public int EngagingClubId { get; set; }
        public int ReleasingClubId { get; set; }
        public int PlayerId { get; set; }
        public PlayersContract PlayersContract { get; set; }
    }
}