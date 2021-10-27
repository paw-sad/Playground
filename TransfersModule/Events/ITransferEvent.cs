namespace TransfersService.Events
{
    public interface ITransferEvent
    {
    } 
    
    public interface ISerializableTransferEvent : ITransferEvent
    {
        public string EventType { get; }
    }
}