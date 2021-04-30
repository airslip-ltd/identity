using Airslip.Common.Contracts;
using System;

namespace Airslip.Identity.Api.Contracts.Responses
{
    public record ExternalLoginResponse(
        string FirstName,
        string Surname,
        string Email,
        DateTimeOffset? ExpiryDate) : ISuccess;
}