﻿using JetBrains.Annotations;
using System.ComponentModel.DataAnnotations;

namespace Airslip.Identity.Api.Contracts.Requests
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public record ForgotPasswordRequest
    {
        [Required] [EmailAddress] public string? Email { get; init; }
    }
}