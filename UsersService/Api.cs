using System.Collections.Generic;
using System.Linq;
using UsersService.Contract;

namespace UsersService
{
   public class Api
    {
        private readonly List<User> _users = new();

        public LogInResponse Authenticate(LogInRequest request)
        {
            var user = _users.First(u => u.UserName == request.UserName && u.Password == request.Password);
            return new LogInResponse
            {
                UserId = user.UserId
            };
        }

        public RegisterUserResponse RegisterUser(RegisterUserRequest request)
        {
            var newUser = new User
            {
                ClubId = request.ClubId,
                Password = request.Password,
                UserName = request.UserName,
                UserId = _users.Count + 1
            };
          _users.Add(newUser);

          return new RegisterUserResponse
          {
              UserId = newUser.UserId
          };
        }

        public GetUserByIdResponse GetUserById(GetUserByIdRequest request)
        {
            var user = _users.First(u => u.UserId == request.UserId);

            return new GetUserByIdResponse
            {
                UserId = user.UserId,
                ClubId = user.ClubId,
                UserName = user.UserName
            };
        }

        private class User
        {
            public string UserName { get; set; }
            public string Password { get; set; }
            public int ClubId { get; set; }
            public int UserId { get; set; }
        }
    }
}
