using Airslip.Common.Types.Configuration;
using Microsoft.Extensions.Options;
using MongoDBMigrations;
using MongoDBMigrations.Document;

namespace Airslip.Identity.Services.MongoDb.Implementations
{
    public class MongoDbMigrator : IMongoDbMigrator
    {
        private readonly MongoDbSettings _options;

        public MongoDbMigrator(IOptions<MongoDbSettings> options)
        {
            _options = options.Value;
        }

        public void Migrate(Version targetVersion)
        {
            new MigrationEngine()
                .UseDatabase(_options.ConnectionString, _options.DatabaseName, MongoEmulationEnum.AzureCosmos)
                .UseAssemblyOfType<MongoDbMigrator>()
                .UseSchemeValidation(false)
                .Run(targetVersion);
        }
    }

    public interface IMongoDbMigrator
    {
        void Migrate(Version targetVersion);
    }
}