using Airslip.Common.Repository.Interfaces;
using JetBrains.Annotations;
using System;

namespace Airslip.Identity.MongoDb.Contracts.Entities
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public record UserProfile : IEntityWithId
    {
        public string Id { get; init; } = Guid.NewGuid().ToString("N");
        
        public string UserId { get; init; }

        public string Email { get; init; }

        public UserProfile(string userId, string email)
        {
            UserId = userId;
            Email = email;
        }

        public string? FirstName { get; private set; }
        public string? Surname { get; private set; }
        public string? Gender { get; private set; }
        public long? DateOfBirth { get; private set; }
        public string? Postalcode { get; private set; }
        public string? FirstLineAddress { get; private set; }
        public string? SecondLineAddress { get; private set; }
        public string? City { get; private set; }
        public string? County { get; private set; }
        public string? Country { get; private set; }

        public void Update(
            string? firstName,
            string? surname,
            string? gender,
            long? dateOfBirth,
            string? postalcode,
            string? firstLineAddress,
            string? secondLineAddress,
            string? city,
            string? county,
            string? country)
        {
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
    }
}