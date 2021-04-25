using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PublicApi.Contract;
using Shouldly;
using UsersService.Contract;
using LogInRequest = PublicApi.Contract.LogInRequest;

namespace Tests.PublicApiTests
{
    [TestClass]
    public class ReleasePlayerTests
    {
        [TestMethod]
        public void CanReleasePlayerFromMyOwnClub()
        {
            var userService = new UsersService.Api();
            var juventusClubId = 1;
            var someOtherClub = 2;
            userService.RegisterUser(new RegisterUserRequest
            {
                ClubId = 1,
                Password = "1234",
                UserName = "Juventus"
            });

            var api = new PublicApi.Api(userService);
            api.LogIn(new LogInRequest
            {
                UserName = "Juventus",
                Password = "1234"
            });

            api.ReleasePlayer(new ReleasePlayerRequest
            {
                EngagingClubId = someOtherClub,
                PaymentsAmount = 0,
                PlayerId = 1,
                ReleasingClubId = juventusClubId,
            });
        }

        [TestMethod]
        public void CantReleasePlayerNotFromMyClub()
        {
            var userService = new UsersService.Api();
            var juventusClubId = userService.RegisterUser(new RegisterUserRequest
            {
                ClubId = 1,
                Password = "1234",
                UserName = "Juventus"
            }).UserId;

            var api = new PublicApi.Api(userService);
            api.LogIn(new LogInRequest
            {
                UserName = "Juventus",
                Password = "1234"
            });

            Should.Throw<Exception>(() => api
                .EngagePlayerWithoutTransferAgreement(new EngageWithoutTransferAgreementRequest
                {
                    EngagingClubId = juventusClubId + 1,
                    PaymentsAmount = 0,
                    PlayerId = 1,
                    ReleasingClubId = 2,
                }));
        }
    }
}
