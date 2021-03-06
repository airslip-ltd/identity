using Airslip.Common.Repository.Types.Interfaces;

namespace Airslip.Identity.Api.Contracts.Entities
{
    public record OpenBankingProvider : IEntityNoId
    {
        public string Name { get; }
        public string Id { get; }
        public string ApplicationId { get; }
        public string ReferenceId { get; }

        public OpenBankingProvider(string name, string id, string applicationId, string referenceId)
        {
            Name = name;
            Id = id;
            ApplicationId = applicationId;
            ReferenceId = referenceId;
        }
    }
}