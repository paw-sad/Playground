using System;

namespace UsersService.Infrastructure.DatabaseEntities
{
    internal class UserCreatedEvent : UserEvent
    {
        public string Name { get; set; }
    }
}