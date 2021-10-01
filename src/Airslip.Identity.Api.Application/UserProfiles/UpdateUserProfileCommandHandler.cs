using Airslip.Common.Types;
using Airslip.Common.Types.Interfaces;
using Airslip.Identity.Api.Application.Interfaces;
using Airslip.Identity.Api.Contracts.Entities;
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

        public async Task<IResponse> Handle(UpdateUserProfileCommand request, CancellationToken cancellationToken)
        {
            UserProfile userProfile = await _userProfileService.Get(request.UserId);

            userProfile.Update(
                request.FirstName,
                request.Surname,
                request.Gender,
                request.DateOfBirth?.ToUnixTimeMilliseconds(),
                request.Postalcode,
                request.FirstLineAddress,
                request.SecondLineAddress,
                request.City,
                request.County,
                request.Country);

            await _userProfileService.Update(userProfile);

            return Success.Instance;
        }
    }
}