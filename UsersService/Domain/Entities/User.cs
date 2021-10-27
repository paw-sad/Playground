using System;
using System.Collections.Generic;
using UsersService.Domain.Events;

namespace UsersService.Domain.Entities
{
    internal class User
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public IList<IUserEvent> NewEvents { get; set; } = new List<IUserEvent>();

        public static User Create(Guid id, string name)
        {
            var user = new User
            {
                Id = id,
                Name = name
            };

            user.NewEvents.Add(new UserCreatedEvent
            {
                Name = name
            });

            return user;
        }
    }
}
