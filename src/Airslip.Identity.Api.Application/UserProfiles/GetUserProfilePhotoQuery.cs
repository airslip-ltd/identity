using Airslip.Common.Types.Interfaces;
using MediatR;

namespace Airslip.Identity.Api.Application.UserProfiles
{
    public class GetUserProfilePhotoQuery : IRequest<IResponse>
    {
        public string UserId { get; }

        public GetUserProfilePhotoQuery(string userId)
        {
            UserId = userId;
        }
    }
}