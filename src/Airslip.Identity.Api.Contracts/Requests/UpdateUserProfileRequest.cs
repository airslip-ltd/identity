using System;

namespace Airslip.Identity.Api.Contracts.Requests
{
    public record UpdateUserProfileRequest(
        string? FirstName,
        string? Surname,
        string? Gender,
        DateTimeOffset? DateOfBirth,
        string? Postalcode,
        string? FirstLineAddress,
        string? SecondLineAddress,
        string? City,
        string? County,
        string? Country);
}