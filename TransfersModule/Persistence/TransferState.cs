namespace TransfersModule.Persistence
{
    internal enum TransferState
    {
        Draft,
        Confirmed,
        Completed,
        AwaitingCounterInstruction,
        MatchingException
    }
}