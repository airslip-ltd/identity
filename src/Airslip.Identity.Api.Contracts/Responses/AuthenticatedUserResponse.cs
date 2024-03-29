﻿using Airslip.Common.Types.Hateoas;
using Airslip.Common.Types.Interfaces;
using Airslip.Identity.Api.Contracts.Models;
using JetBrains.Annotations;
using System.Collections.Generic;

namespace Airslip.Identity.Api.Contracts.Responses;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public class AuthenticatedUserResponse : LinkResourceBase, ISuccess
{
    public string BearerToken { get; }
    public long Expiry { get; }
    public string RefreshToken { get; }
    public bool IsNewUser { get; }
    public UserModel User { get; }

    public AuthenticatedUserResponse(string bearerToken, long expiry, string refreshToken, bool isNewUser, UserModel user)
    {
        BearerToken = bearerToken;
        Expiry = expiry;
        RefreshToken = refreshToken;
        IsNewUser = isNewUser;
        User = user;
    }
}