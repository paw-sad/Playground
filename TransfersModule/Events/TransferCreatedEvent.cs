﻿using System;
using TransfersModule.Persistence;

namespace TransfersModule.Events
{
    internal class TransferCreatedEvent
    {
        public int EngagingClubId { get; set; }
        public int ReleasingClubId { get; set; }
        public int PlayerId { get; set; }
        public PlayersContract PlayersContract { get; set; }
        public TransferType Type { get; set; }
    }
}