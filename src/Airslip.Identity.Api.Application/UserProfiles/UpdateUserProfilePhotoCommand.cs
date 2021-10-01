using Airslip.Common.Types.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Airslip.Identity.Api.Application.UserProfiles
{
    public class UpdateUserProfilePhotoCommand : IRequest<IResponse>
    {
        public string UserId { get; }
        public IFormFile? Photo { get; }

        public UpdateUserProfilePhotoCommand(string userId, IFormFile? photo)
        {
            UserId = userId;
            Photo = photo;
        }
    }
}