namespace UsersService.Domain.Events
{
    internal class UserCreatedEvent : IUserEvent
    {
        public string Name { get; set; }
    }
}