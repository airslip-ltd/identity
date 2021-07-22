using Airslip.Common.Contracts;
using Airslip.Identity.Api.Contracts.Responses;
using Airslip.Identity.MongoDb.Contracts;
using JetBrains.Annotations;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Airslip.Identity.Api.Application.UserProfiles
{
    [UsedImplicitly(ImplicitUseTargetFlags.Itself)]
    public class GetUserProfileQueryHandler : IRequestHandler<GetUserProfileQuery, IResponse>
    {
        private readonly IUserProfileService _userProfileService;

        public GetUserProfileQueryHandler(IUserProfileService userProfileService)
        {
            _userProfileService = userProfileService;
        }
        
        public async Task<IResponse> Handle(GetUserProfileQuery query, CancellationToken cancellationToken)
        {
            UserProfile userProfile = await _userProfileService.Get(query.UserId);

            return userProfile.ToUserProfileResponse();
        }
    }

    public static class UserProfileResponseExtensions
    {
        public static UserProfileResponse ToUserProfileResponse(this UserProfile userProfile)
        {
            return new(
                userProfile.Email,
                userProfile.FirstName,
                userProfile.Surname,
                userProfile.Gender,
                userProfile.DateOfBirth != null ? DateTimeOffset.FromUnixTimeMilliseconds(userProfile.DateOfBirth.Value) : null,
                userProfile.Postalcode,
                userProfile.FirstLineAddress,
                userProfile.SecondLineAddress,
                userProfile.City,
                userProfile.County,
                userProfile.Country);
        }
    }
}