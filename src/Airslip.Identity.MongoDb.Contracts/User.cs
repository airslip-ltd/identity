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
            string referenceId)
        {
            Id = id;
            ApplicationId = applicationId;
            EmailAddress = emailAddress;
            ReferenceId = referenceId;
            CreatedDate = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        }

        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public string Id { get; private set; }
        [BsonElement("applicationId")] public string ApplicationId { get; init; }
        [BsonElement("emailAddress")] public string EmailAddress { get; init; }
        [BsonElement("referenceId")] public string ReferenceId { get; init; }
        [BsonElement("createdDate")] public long CreatedDate { get; init; }
        [BsonElement("biometricOn")] public bool BiometricOn { get; private set; }
        [BsonElement("refreshTokens")] public List<RefreshToken> RefreshTokens { get; set; } = new();

        public void AddRefreshToken(string deviceId, string token)
        {
            RefreshTokens.Add(new RefreshToken(deviceId, token));
        }
    }

    public record RefreshToken(
        [property: BsonElement("deviceId")] string DeviceId,
        [property: BsonElement("token")] string Token);
}