using Airslip.Common.Repository.Entities;
using Airslip.Common.Repository.Enums;
using Airslip.Common.Repository.Interfaces;
using Airslip.Common.Types.Enums;
using Airslip.Identity.Api.Contracts.Entities;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;

namespace Airslip.Identity.Api.Contracts.Models
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public record UserModel : IModel
    {
        public string? Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string? FirstName { get; init; }
        public string? LastName { get; init; }
        public string? DisplayName { get; init; }
        public string? UserRole { get; private set; }
        public long CreatedDate { get; init; } = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        public bool BiometricOn { get; private set; }

        [JsonIgnore]
        public BasicAuditInformation? AuditInformation { get; set; }
        [JsonIgnore]
        public EntityStatus EntityStatus { get; set; } = EntityStatus.Active;
        [JsonConverter(typeof(StringEnumConverter))] 
        public AirslipUserType AirslipUserType { get; set; } = AirslipUserType.Standard;
        [JsonIgnore]
        public string? EntityId { get; set; }
        [JsonIgnore]
        public ICollection<OpenBankingProvider> OpenBankingProviders { get; private set; } =
            new List<OpenBankingProvider>(1);
        [JsonIgnore]
        public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>(1);
        public DataConsent DataConsent { get; set; } = new ();
        public void SetRole(string userRole)
        {
            UserRole = userRole;
        }
    }
}