using JetBrains.Annotations;

namespace Airslip.Identity.MongoDb.Contracts
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public record MongoDbSettings : IMongoDbSettings
    {
        public string ConnectionString { get; init; } = string.Empty;
        public string DatabaseName { get; init; } = string.Empty;
    }

    public interface IMongoDbSettings
    {
        string ConnectionString { get; init; }
        string DatabaseName { get; init; }
    }
}