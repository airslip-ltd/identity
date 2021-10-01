using Airslip.Common.Types.Interfaces;
using Airslip.Common.Types.Hateoas;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;

namespace Airslip.Identity.Api.Contracts.Responses
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class UserProfileResponse : LinkResourceBase, ISuccess
    {
        public string? Email { get; }
        public string? FirstName { get; }
        public string? Surname { get; }
        public string? Gender { get; }
        public DateTimeOffset? DateOfBirth { get; }
        public string? Postalcode { get; }
        public string? FirstLineAddress { get; }
        public string? SecondLineAddress { get; }
        public string? City { get; }
        public string? County { get; }
        public string? Country { get; }

        public UserProfileResponse(
            string? email,
            string? firstName,
            string? surname,
            string? gender,
            DateTimeOffset? dateOfBirth,
            string? postalcode,
            string? firstLineAddress,
            string? secondLineAddress,
            string? city,
            string? county,
            string? country)
        {
            Email = email;
            FirstName = firstName;
            Surname = surname;
            Gender = gender;
            DateOfBirth = dateOfBirth;
            Postalcode = postalcode;
            FirstLineAddress = firstLineAddress;
            SecondLineAddress = secondLineAddress;
            City = city;
            County = county;
            Country = country;
        }

        public override T AddHateoasLinks<T>(string baseUri, params string[] identifiers)
        {
            Links = new List<Link>
            {
                new($"{baseUri}/v1/user", "self", "GET")
            };

            return (this as T)!;
        }
    }
}