using Airslip.Common.Contracts;
using Airslip.Common.Types.Hateoas;
using JetBrains.Annotations;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Airslip.Identity.Api.Contracts.Responses
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class AuthenticatedUserResponse : LinkResourceBase, ISuccess
    {
        public string BearerToken { get; }
        
        [JsonIgnore]
        public bool HasAddedInstitution { get; }

        public AuthenticatedUserResponse(string bearerToken, bool hasAddedInstitution)
        {
            BearerToken = bearerToken;
            HasAddedInstitution = hasAddedInstitution;
        }
        
        public AuthenticatedUserResponse AddHateoasLinks(string baseUri, bool hasAddedInstitution, string? countryCode)
        {
            if (hasAddedInstitution)
            {
                Links = new List<Link>
                {
                    new ($"{baseUri}/v1/authenticate/login", "self", "POST"),
                    new ($"{baseUri}/v1/accounts", "next", "GET")
                };
            }
            else
            {
                Links = new List<Link>
                {
                    new ($"{baseUri}/v1/authenticate/login", "self", "POST"),
                    new ($"{baseUri}/v1/institutions/{countryCode}", "next", "GET")

                };
            }

            return this;
        }
    }
}
