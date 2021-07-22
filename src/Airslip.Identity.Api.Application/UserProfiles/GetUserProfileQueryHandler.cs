using Airslip.BankTransactions.Api.Contracts.Requests;
using Airslip.BankTransactions.Api.Contracts.Responses;
using Airslip.BankTransactions.MongoDb.Contracts;
using Airslip.Common.Contracts;
using JetBrains.Annotations;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Airslip.BankTransactions.Api.Application.UserProfiles
{
    [UsedImplicitly(ImplicitUseTargetFlags.Itself)]
    public class GetUserProfileQueryHandler : IRequestHandler<GetUserProfileQuery, IResponse>
    {
        private readonly IUserService _userService;

        public GetUserProfileQueryHandler(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<IResponse> Handle(GetUserProfileQuery query, CancellationToken cancellationToken)
        {
            User user = await _userService.Get(query.UserId);

            return user.ToUserProfileResponse();
        }
    }

    public static class UserProfileResponseExtensions
    {
        public static UserProfileResponse ToUserProfileResponse(this User user)
        {
            return new(
                user.EmailAddress,
                user.FirstName,
                user.Surname,
                user.Gender,
                user.DateOfBirth != null ? DateTimeOffset.FromUnixTimeMilliseconds(user.DateOfBirth.Value) : null,
                user.Postalcode,
                user.FirstLineAddress,
                user.SecondLineAddress,
                user.City,
                user.County,
                user.Country,
                new UserSettingsRequest(user.Settings.HasFaceId));
        }
    }
}