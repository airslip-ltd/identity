using JetBrains.Annotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Airslip.Identity.MongoDb.Contracts
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public record User
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public string Id { get; init; } = Guid.NewGuid().ToString("N");

        [BsonElement("openBankingProviders")]
        public ICollection<OpenBankingProvider> OpenBankingProviders { get; private set; } =
            new List<OpenBankingProvider>(1);

        [BsonElement("createdDate")]
        public long CreatedDate { get; init; } = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        [BsonElement("biometricOn")] public bool BiometricOn { get; private set; }

        [BsonElement("refreshTokens")]
        public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>(1);

        public void AddRefreshToken(string deviceId, string token)
        {
            RefreshTokens.Add(new RefreshToken(deviceId, token));
        }
        
        public void AddOpenBankingProvider(OpenBankingProvider openBankingProvider)
        {
            OpenBankingProviders.Add(openBankingProvider);
        }
        
        public string? GetOpenBankingProviderId(string name)
        {
            return OpenBankingProviders.FirstOrDefault(obp => obp.Name == name)?.Id;
        }
    }
    
    public record OpenBankingProvider
    {
        [BsonElement("name")] public string Name { get; }
        [BsonElement("id")] public string Id { get; }
        [BsonElement("applicationId")] public string ApplicationId { get; }
        [BsonElement("referenceId")]  public string ReferenceId { get; }

        public OpenBankingProvider(string name, string id, string applicationId, string referenceId)
        {
            Name = name;
            Id = id;
            ApplicationId = applicationId;
            ReferenceId = referenceId;
        }
    }

    public record RefreshToken(
        [property: BsonElement("deviceId")] string DeviceId,
        [property: BsonElement("token")] string Token);
}