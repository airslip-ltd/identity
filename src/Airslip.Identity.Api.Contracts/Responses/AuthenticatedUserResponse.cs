﻿using Airslip.Common.Contracts;
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
        public long Expiry { get; }
        public string RefreshToken { get; }
        [JsonIgnore] public bool HasAddedInstitution { get; }
        public UserSettingsResponse Settings { get; }

        public AuthenticatedUserResponse(
            string bearerToken,
            long expiry,
            string refreshToken,
            bool hasAddedInstitution,
            UserSettingsResponse settings)
        {
            BearerToken = bearerToken;
            Expiry = expiry;
            RefreshToken = refreshToken;
            HasAddedInstitution = hasAddedInstitution;
            Settings = settings;
        }

        public AuthenticatedUserResponse AddHateoasLinks(
            string baseUri,
            string bankTransactionsUri,
            bool hasAddedInstitution,
            string? countryCode)
        {
            if (hasAddedInstitution)
            {
                Links = new List<Link>
                {
                    new($"{baseUri}/v1/identity/login", "self", "POST"),
                    new($"{bankTransactionsUri}/v1/accounts", "next", "GET")
                };
            }
            else
            {
                Links = new List<Link>
                {
                    new($"{baseUri}/v1/identity/login", "self", "POST"),
                    new($"{bankTransactionsUri}/v1/institutions/{countryCode}", "next", "GET")
                };
            }

            return this;
        }
    }
}
