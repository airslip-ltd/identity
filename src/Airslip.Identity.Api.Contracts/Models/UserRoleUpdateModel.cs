using Airslip.Common.Repository.Entities;
using Airslip.Common.Repository.Enums;
using Airslip.Common.Repository.Interfaces;
using Airslip.Common.Types.Enums;
using Airslip.Identity.Api.Contracts.Entities;
using JetBrains.Annotations;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Airslip.Identity.Api.Contracts.Models
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public record UserRoleUpdateModel : IModel
    {
        public string? Id { get; set; }
        public EntityStatus EntityStatus { get; set; }
        public string? UserRole { get; set; }
        public string Email { get; set; } = string.Empty;
        public string? FirstName { get; init; }
        public string? LastName { get; init; }
    }
}