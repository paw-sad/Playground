using System;
using System.Collections.Generic;

namespace UsersService.Infrastructure.DatabaseEntities
{
    internal class User
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}
