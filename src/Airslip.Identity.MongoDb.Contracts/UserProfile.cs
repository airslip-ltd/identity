using JetBrains.Annotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Airslip.Identity.MongoDb.Contracts
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public record UserProfile
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public string UserId { get; init; }

        [BsonElement("email")] public string Email { get; init; }

        public UserProfile(string userId, string email)
        {
            UserId = userId;
            Email = email;
        }

        [BsonElement("firstName")] public string? FirstName { get; private set; }
        [BsonElement("surname")] public string? Surname { get; private set; }
        [BsonElement("gender")] public string? Gender { get; private set; }
        [BsonElement("dateOfBirth")] public long? DateOfBirth { get; private set; }
        [BsonElement("postalcode")] public string? Postalcode { get; private set; }
        [BsonElement("firstLineAddress")] public string? FirstLineAddress { get; private set; }
        [BsonElement("secondLineAddress")] public string? SecondLineAddress { get; private set; }
        [BsonElement("city")] public string? City { get; private set; }
        [BsonElement("county")] public string? County { get; private set; }
        [BsonElement("country")] public string? Country { get; private set; }
        [BsonElement("biometricOn")] public bool BiometricOn { get; private set; }

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

        public void UpdateBiometric(bool value)
        {
            BiometricOn = value;
        }
    }
}