﻿using Airslip.Common.Auth.Data;
using Airslip.Common.Repository.Types.Entities;
using Airslip.Common.Repository.Types.Enums;
using Airslip.Common.Repository.Types.Interfaces;
using Airslip.Common.Types.Enums;
using Airslip.Common.Utilities;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;

namespace Airslip.Identity.Api.Contracts.Entities
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public record User : IEntityWithOwnership
    {
        public string Id { get; set; } = CommonFunctions.GetId();
        public string Email { get; init; }
        public string? FirstName { get; init; }
        public string? LastName { get; init; }
        public string UserRole { get; set; } = UserRoles.User;
        public string? DisplayName { get; init; }
        public BasicAuditInformation? AuditInformation { get; set; }
        public EntityStatus EntityStatus { get; set; } = EntityStatus.Active;
        public AirslipUserType AirslipUserType { get; set; } = AirslipUserType.Standard;
        public string? UserId { get; set; }
        public string? EntityId { get; set; }

        [Obsolete]
        public ICollection<OpenBankingProvider> OpenBankingProviders { get; private set; } =
            new List<OpenBankingProvider>(1);

        public long CreatedDate { get; init; } = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        public bool BiometricOn { get; private set; }

        public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>(1);
        public DataConsent DataConsent { get; set; } = new ();
        public User(string email, string? firstName, string? lastName, string? userRole = UserRoles.User)
        {
            Email = email;
            FirstName = firstName;
            LastName = lastName;
            UserRole = userRole ?? UserRoles.User;
        }
        
        public void AddRefreshToken(string deviceId, string token)
        {
            RefreshTokens.Add(new RefreshToken(deviceId, token));
        }
        
        public void UpdateDataConsent(DataConsent dataConsent)
        {
            DataConsent = dataConsent;
        }

        public void ChangeFromUnregisteredToStandard()
        {
            if(AirslipUserType == AirslipUserType.Unregistered)
                AirslipUserType = AirslipUserType.Standard;
        }
    }
}