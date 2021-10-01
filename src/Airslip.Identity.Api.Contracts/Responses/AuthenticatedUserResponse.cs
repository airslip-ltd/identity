using Airslip.Common.Types.Hateoas;
using Airslip.Common.Types.Interfaces;
using JetBrains.Annotations;
using System.Collections.Generic;

namespace Airslip.Identity.Api.Contracts.Responses
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class AuthenticatedUserResponse : LinkResourceBase, ISuccess
    {
        public string BearerToken { get; }
        public long Expiry { get; }
        public string RefreshToken { get; }
        public bool BiometricOn { get; }
        public bool IsNewUser { get; }

        public AuthenticatedUserResponse(string bearerToken, long expiry, string refreshToken, bool biometricOn, bool isNewUser)
        {
            BearerToken = bearerToken;
            Expiry = expiry;
            RefreshToken = refreshToken;
            BiometricOn = biometricOn;
            IsNewUser = isNewUser;
        }

        public AuthenticatedUserResponse AddHateoasLinks(
            string baseUri,
            string bankTransactionsUri,
            bool isNewUser,
            string? countryCode)
        {
            if (isNewUser)
            {
                Links = new List<Link>
                {
                    new($"{baseUri}/v1/identity/login", "self", "POST"),
                    new($"{bankTransactionsUri}/v1/banks/{countryCode}", "next", "GET")
                };
            }
            else
            {
                Links = new List<Link>
                {
                    new($"{baseUri}/v1/identity/login", "self", "POST"),
                    new($"{bankTransactionsUri}/v1/accounts", "next", "GET")
                   
                };
            }

            return this;
        }
    }
}
