using Airslip.Common.Contracts;
using MediatR;

namespace Airslip.BankTransactions.Api.Application.Admin
{
    public class GetYapilyUserQuery : IRequest<IResponse>
    {
        public string UserId { get; }

        public GetYapilyUserQuery(string userId)
        {
            UserId = userId;
        }
    }
}