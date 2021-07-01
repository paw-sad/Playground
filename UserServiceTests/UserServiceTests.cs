using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using UsersService;
using UsersService.Contract;

namespace Tests.UsersServiceTests
{
    [TestClass]
    public class UserServiceTests
    {
        [TestMethod]
        public void CanAuthenticateIfPasswordAndLoginAreCorrect()
        {
            var api = new Api();
            var registerUserResponse = api.RegisterUser(new RegisterUserRequest
            {
                UserName = "Juventus",
                Password = "1234",
                ClubId = 1
            });

            var logInResponse = api.Authenticate(new LogInRequest
            {
                UserName = "Juventus",
                Password = "1234"
            });

            logInResponse.UserId.ShouldBe(registerUserResponse.UserId);
        }

        [TestMethod]
        public void CantAuthenticateIfPasswordIsNotAreCorrect()
        {
            var api = new Api();
            api.RegisterUser(new RegisterUserRequest
            {
                UserName = "Juventus",
                Password = "1234",
                ClubId = 1
            });

            Should.Throw<Exception>(() => api.Authenticate(new LogInRequest
            {
                UserName = "Juventus",
                Password = "0000"
            }));
        }

        [TestMethod]
        public void CanGetUserById()
        {
            var api = new Api();
            var registerUserResponse = api.RegisterUser(new RegisterUserRequest
            {
                UserName = "Juventus",
                Password = "1234",
                ClubId = 1
            });

            var getUserResponse = api.GetUserById(new GetUserByIdRequest
            {
                UserId = registerUserResponse.UserId

            });

            getUserResponse.ClubId.ShouldBe(1);
            getUserResponse.UserId.ShouldBe(registerUserResponse.UserId);
            getUserResponse.UserName.ShouldBe("Juventus");
        }
    }
}
