using Airslip.Common.Auth.Enums;
using Airslip.Common.Repository.Entities;
using Airslip.Common.Repository.Enums;
using Airslip.Common.Repository.Interfaces;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Airslip.Identity.Api.Contracts.Entities
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public record User : IEntity
    {
        public string Id { get; set; } = Guid.NewGuid().ToString("N");

        public BasicAuditInformation? AuditInformation { get; set; }

        public EntityStatus EntityStatus { get; set; } = EntityStatus.Active;

        public AirslipUserType AirslipUserType { get; set; } = AirslipUserType.Standard;
        public string? EntityId { get; set; }

        public ICollection<OpenBankingProvider> OpenBankingProviders { get; private set; } =
            new List<OpenBankingProvider>(1);

        public long CreatedDate { get; init; } = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        public bool BiometricOn { get; private set; }

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
}