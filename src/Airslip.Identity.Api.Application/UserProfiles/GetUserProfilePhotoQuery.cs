using Airslip.Common.Contracts;
using MediatR;

namespace Airslip.BankTransactions.Api.Application.UserProfiles
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