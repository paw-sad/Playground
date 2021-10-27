using System;

namespace UsersService.Infrastructure.DatabaseEntities
{
    internal abstract class UserEvent   
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
    }
}