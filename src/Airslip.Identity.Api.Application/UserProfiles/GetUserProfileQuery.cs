using Airslip.Common.Contracts;
using MediatR;

namespace Airslip.BankTransactions.Api.Application.UserProfiles
{
    public class GetUserProfileQuery : IRequest<IResponse>
    {
        public string UserId { get; }

        public GetUserProfileQuery(string userId)
        {
            UserId = userId;
        }
    }
}