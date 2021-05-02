using UsersService.Contract;

namespace PublicApi
{
    public class AuthorizationService
    {
        public static bool CanExecuteCommand(GetUserByIdResponse user, TransfersModule.Contract.EngageWithoutTransferAgreement.Request command)
        {
            return user.ClubId == command.EngagingClubId;
        }

        public static bool CanExecuteCommand(GetUserByIdResponse user, TransfersModule.Contract.ReleasePlayerContract.Request command)
        {
            return user.ClubId == command.ReleasingClubId;
        }
    }
}
