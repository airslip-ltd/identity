using JetBrains.Annotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

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
        [BsonElement("createdDate")] public long CreatedDate { get; init; }
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
        [BsonElement("settings")] public UserSettings? Settings { get; private set; }

        public string? PreviousViewedAccountId { get; set; }
    }
    
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class UserInstitution
    {
        public UserInstitution(string description)
        {
            Description = description;
        }

        [BsonElement("description")] public string Description { get; init; }

        [BsonElement("consentToken")] public string? ConsentToken { get; private set; }

        [BsonElement("isAuthorised")] public bool IsAuthorised { get; private set; }
    }
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class UserSettings
    {
        [BsonElement("hasFaceId")]
        public bool? HasFaceId { get; }

        public UserSettings(bool? hasFaceId)
        {
            HasFaceId = hasFaceId;
        }
    }
}