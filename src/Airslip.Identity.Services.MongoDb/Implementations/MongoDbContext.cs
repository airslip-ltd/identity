using Airslip.Common.Services.MongoDb;
using Airslip.Common.Services.MongoDb.Extensions;
using Airslip.Common.Types.Configuration;
using Airslip.Identity.Api.Application.Interfaces;
using Airslip.Identity.Api.Contracts.Entities;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Linq;
using System.Threading.Tasks;
using Version = MongoDBMigrations.Version;

namespace Airslip.Identity.Services.MongoDb.Implementations
{
    public class MongoDbContext : AirslipMongoDbBase, IIdentityContext
    {
        public MongoDbContext(MongoClient mongoClient, IMongoDbMigrator mongoDbMigrator, 
            IOptions<MongoDbSettings> options) : base(mongoClient,options)
        {
            mongoDbMigrator.Migrate(new Version(1,0,1));
        }
        
        public async Task<string?> GetProviderId(string id, string provider)
        {
            IMongoCollection<User> collection = Database.CollectionByType<User>();
            User? user = await collection.Find(user => user.Id == id).FirstOrDefaultAsync();
            return user.OpenBankingProviders.Where(o => o.Name == provider)
                .Select(u => u.Id)
                .FirstOrDefault();
        }

        public async Task<User?> GetByEmail(string email)
        {
            IMongoCollection<User> coll = Database.CollectionByType<User>();

            return await coll.Find(o =>
                    o.Email == email)
                .FirstOrDefaultAsync();
        }

        public async Task UpdateOrReplaceRefreshToken(string id, string deviceId, string token)
        {
            IMongoCollection<User> collection = Database.CollectionByType<User>();

            FilterDefinitionBuilder<User>? filterDefinitionBuilder = Builders<User>.Filter;
            FilterDefinition<User> userFilter = filterDefinitionBuilder.Eq(user => user.Id, id);
            FilterDefinition<User> filter = userFilter & filterDefinitionBuilder.Eq("refreshTokens.deviceId", deviceId);

            UpdateDefinition<User> update = Builders<User>.Update.Set("refreshTokens.$.token", token);

            UpdateResult updateResult = await collection.UpdateOneAsync(filter, update);

            if (updateResult.ModifiedCount > 0)
                return;

            // This is only invoked when a new device is registered
            User userIn = await collection.Find(userFilter).FirstOrDefaultAsync();

            userIn.AddRefreshToken(deviceId, token);

            await collection.ReplaceOneAsync(
                user => user.Id == id, userIn,
                new ReplaceOptions { IsUpsert = true });
        }
    }
}