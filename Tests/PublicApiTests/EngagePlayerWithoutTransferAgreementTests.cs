using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PublicApi.Contract;
using Shouldly;
using UsersService.Contract;
using LogInRequest = PublicApi.Contract.LogInRequest;

namespace Tests.PublicApiTests
{
    [TestClass]
    public class EngagePlayerWithoutTransferAgreementTests
    {
        [TestMethod]
        public void CanEngagePlayerForMyOwnClub()
        {
            var userService = new UsersService.Api(); 
            var juventusClubId = 1;
            var someOtherClub = 2;

               userService.RegisterUser(new RegisterUserRequest
            {
                ClubId = juventusClubId,
                Password = "1234",
                UserName = "Juventus"
            });

            var api = new PublicApi.Api(userService);
            api.LogIn(new LogInRequest
            {
                UserName = "Juventus",
                Password = "1234"
            });

            api.EngagePlayerWithoutTransferAgreement(new EngageWithoutTransferAgreementRequest
            {
                EngagingClubId = juventusClubId,
                PaymentsAmount = 0,
                PlayerId = 1,
                ReleasingClubId = someOtherClub,
            });
        } 
        
        [TestMethod]
        public void CantEngagePlayerNotForMyClub()
        {
            var userService = new UsersService.Api();
            var juventusClubId = 1;
            var someOtherClub = 2;

            userService.RegisterUser(new RegisterUserRequest
            {
                ClubId = juventusClubId,
                Password = "1234",
                UserName = "Juventus"
            });

            var api = new PublicApi.Api(userService);
            api.LogIn(new LogInRequest
            {
                UserName = "Juventus",
                Password = "1234"
            });

            Should.Throw<Exception>(() => api
                .EngagePlayerWithoutTransferAgreement(new EngageWithoutTransferAgreementRequest
            {
                EngagingClubId = someOtherClub,
                PaymentsAmount = 0,
                PlayerId = 1,
                ReleasingClubId = juventusClubId,
            }));
        }
    }
}
