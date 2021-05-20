using JetBrains.Annotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Airslip.Identity.MongoDb.Contracts
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public record User
    {
        public User(
            string id, 
            string applicationId, 
            string emailAddress, 
            string referenceId,
            List<UserInstitution> institutions)
        {
            Id = id;
            ApplicationId = applicationId;
            EmailAddress = emailAddress;
            ReferenceId = referenceId;
            Institutions = institutions;
            CreatedDate = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        }

        [BsonId]
        [BsonRepresentation(BsonType.String)]
        [BsonElement("id")]
        public string Id { get; private set; }
        [BsonElement("applicationId")] public string ApplicationId { get; init; }
        [BsonElement("emailAddress")] public string EmailAddress { get; init; }
        [BsonElement("referenceId")] public string ReferenceId { get; init; }
        [BsonElement("institutions")] public List<UserInstitution> Institutions { get; init; }
        [BsonElement("createdDate")] public long? CreatedDate { get; init; }

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
        public string? PreviousViewedAccountId { get; set; }
        
        public void UpdatePreviousViewedAccountId(string value)
        {
            PreviousViewedAccountId = value;
        }
        public void UpsertInstitution(UserInstitution userInstitutionIn)
        {
            UserInstitution? existingInstitution =
                Institutions.FirstOrDefault(institution => institution.Description == userInstitutionIn.Description);

            if (existingInstitution != null)
                Institutions.Remove(existingInstitution);

            Institutions.Add(userInstitutionIn);
        }

        public UserInstitution? GetInstitution(string institutionId)
        {
            return Institutions.FirstOrDefault(institution => institution.Description == institutionId);
        }

        public void UpdateUserProfile(
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

        public void UnauthoriseInstitution(string institution)
        {
            UserInstitution? userInstitution = GetInstitution(institution);
            userInstitution?.Unauthorise();
        }
    }
    
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class UserInstitution
    {
        public UserInstitution(string description)
        {
            Description = description;
        }

        [JsonConstructor]
        public UserInstitution(string description, string? consentToken)
        {
            Description = description;
            ConsentToken = consentToken;
            IsAuthorised = true;
        }

        [BsonElement("description")] public string Description { get; init; }

        [BsonElement("consentToken")] public string? ConsentToken { get; private set; }

        [BsonElement("isAuthorised")] public bool IsAuthorised { get; private set; }

        public void UpdateConsentToken(string consentToken)
        {
            ConsentToken = consentToken;
        }

        public void Unauthorise()
        {
            IsAuthorised = false;
        }
    }
}