using Airslip.Common.Contracts;
using MediatR;

namespace Airslip.Identity.Api.Application.Queries
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