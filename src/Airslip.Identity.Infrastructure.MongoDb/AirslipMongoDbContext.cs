using Airslip.Common.Types.Extensions;
using Airslip.Identity.MongoDb.Contracts;
using JetBrains.Annotations;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Airslip.Identity.Infrastructure.MongoDb
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class AirslipMongoDbContext
    {
        private readonly IMongoDatabase _database;
      
        private readonly string _userCollection = $"{nameof(User)}s".ToCamelCase();
        private readonly string _userProfileCollection = $"{nameof(UserProfile)}s".ToCamelCase();

        public AirslipMongoDbContext(IMongoDatabase database)
        {
            _database = database;

            CreateCollections();
        }

        public IMongoCollection<User> Users => _database.GetCollection<User>(_userCollection);
        public IMongoCollection<UserProfile> UserProfiles => _database.GetCollection<UserProfile>(_userProfileCollection);

        private void CreateCollections()
        {
            if (!CheckCollection(_userCollection))
                _database.CreateCollection(_userCollection);
            if (!CheckCollection(_userProfileCollection))
                _database.CreateCollection(_userProfileCollection);
        }

        private bool CheckCollection(string collectionName)
        {
            BsonDocument filter = new("name", collectionName);
            IAsyncCursor<BsonDocument>? collectionCursor =
                _database.ListCollections(new ListCollectionsOptions {Filter = filter});
            return collectionCursor.Any();
        }
    }
}