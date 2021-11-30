﻿using Airslip.Common.Types.Hateoas;
using Airslip.Common.Types.Interfaces;
using Airslip.Identity.Api.Contracts.Entities;
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
        public bool IsNewUser { get; }
        public UserSummary User { get; }

        public AuthenticatedUserResponse(string bearerToken, long expiry, string refreshToken, bool isNewUser, User user)
        {
            BearerToken = bearerToken;
            Expiry = expiry;
            RefreshToken = refreshToken;
            IsNewUser = isNewUser;
            User = new UserSummary(user.Email, user.FirstName, user.LastName);
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

    public record UserSummary(string Email, string? FirstName, string? LastName);
}
