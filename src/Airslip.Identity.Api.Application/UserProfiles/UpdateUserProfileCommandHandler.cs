using Airslip.Common.Contracts;
using Airslip.Identity.MongoDb.Contracts;
using JetBrains.Annotations;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Airslip.Identity.Api.Application.UserProfiles
{
    [UsedImplicitly(ImplicitUseTargetFlags.Itself)]
    public class UpdateUserProfileCommandHandler : IRequestHandler<UpdateUserProfileCommand, IResponse>
    {
        private readonly IUserProfileService _userProfileService;

        public UpdateUserProfileCommandHandler(IUserProfileService userProfileService)
        {
            _userProfileService = userProfileService;
        }

        public async Task<IResponse> Handle(UpdateUserProfileCommand command, CancellationToken cancellationToken)
        {
            UserProfile userProfile = await _userProfileService.Get(command.UserId);

            userProfile.Update(
                command.FirstName,
                command.Surname,
                command.Gender,
                command.DateOfBirth?.ToUnixTimeMilliseconds(),
                command.Postalcode,
                command.FirstLineAddress,
                command.SecondLineAddress,
                command.City,
                command.County,
                command.Country);

            await _userProfileService.Update(userProfile);

            return Success.Instance;
        }
    }
}