using Airslip.Common.Types.Extensions;
using Airslip.Identity.MongoDb.Contracts;
using JetBrains.Annotations;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Collections.Generic;

namespace Airslip.Identity.Infrastructure.MongoDb
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class AirslipMongoDbContext
    {
        private readonly IMongoDatabase _database;
        //private readonly string _applicationUserCollection = $"{nameof(ApplicationUser)}s".ToCamelCase();
        private readonly string _userCollection = $"{nameof(User)}s".ToCamelCase();

        public AirslipMongoDbContext(IOptions<MongoDbSettings> settings)
        {
            MongoClient client = new(settings.Value.ConnectionString);
            _database = client.GetDatabase(settings.Value.DatabaseName);

            CreateCollections();
            SetupIndexes();
        }

        public IMongoCollection<User> Users => _database.GetCollection<User>(_userCollection);
        //public IMongoCollection<ApplicationUser> ApplicationUsers => _database.GetCollection<ApplicationUser>(_applicationUserCollection);
        private void CreateCollections()
        {
            if (!CheckCollection(_userCollection))
                _database.CreateCollection(_userCollection);
        }

        private bool CheckCollection(string collectionName)
        {
            BsonDocument filter = new("name", collectionName);
            IAsyncCursor<BsonDocument>? collectionCursor =
                _database.ListCollections(new ListCollectionsOptions {Filter = filter});
            return collectionCursor.Any();
        }

        private void SetupIndexes()
        {
            CreateIndexOptions indexOptions = new();
            IndexKeysDefinitionBuilder<User>? userBuilders = Builders<User>.IndexKeys;

            Users.Indexes.CreateManyAsync(new List<CreateIndexModel<User>>
            {
                new(userBuilders.Ascending("Institutions.Description"), indexOptions)
            });
        }
    }
}