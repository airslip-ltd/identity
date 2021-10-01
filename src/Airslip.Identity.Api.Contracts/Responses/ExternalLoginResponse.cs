using Airslip.Common.Types.Interfaces;
using System;

namespace Airslip.Identity.Api.Contracts.Responses
{
    public record ExternalLoginResponse(
        string FirstName,
        string Surname,
        string Email,
        string DeviceId,
        DateTimeOffset? ExpiryDate) : ISuccess;
}