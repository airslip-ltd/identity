using Airslip.Common.Auth.Enums;
using Airslip.Common.Repository.Enums;
using Airslip.Common.Repository.Interfaces;
using Airslip.Common.Types.Extensions;
using Airslip.Identity.Api.Contracts.Enums;
using Airslip.Identity.MongoDb.Contracts;
using Airslip.Identity.MongoDb.Contracts.Entities;
using JetBrains.Annotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;

namespace Airslip.Identity.Infrastructure.MongoDb
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class AirslipMongoDbContext
    {
        private readonly IMongoDatabase _database;
      
        private readonly string _userCollection = $"{nameof(User)}s_test".ToCamelCase();
        private readonly string _userProfileCollection = $"{nameof(UserProfile)}s_test".ToCamelCase();
        private readonly string _apiKeyCollection = $"{nameof(ApiKey)}s_test".ToCamelCase();

        public AirslipMongoDbContext(IMongoDatabase database)
        {
            _database = database;

            ConventionRegistry.Register(
                name: "CustomConventionPack", 
                conventions: new ConventionPack
                {
                    new CamelCaseElementNameConvention()
                },
                filter: t => true);
            
            // Enums as integers
            BsonSerializer.RegisterSerializer(new EnumSerializer<EntityStatusEnum>(BsonType.Int32));
            BsonSerializer.RegisterSerializer(new EnumSerializer<ApiKeyStatus>(BsonType.Int32));
            
            // Enum as string
            BsonSerializer.RegisterSerializer(new EnumSerializer<ApiKeyUsageType>(BsonType.Int32));
            
            // Map classes
            mapEntityWithId<ApiKey>();
            mapEntityWithId<User>();
            mapEntityWithId<UserProfile>();
            mapEntityNoId<RefreshToken>();
            mapEntityNoId<OpenBankingProvider>();
            
            CreateCollections();
        }

        private void mapEntityWithId<TType>() where TType : IEntityWithId
        {
            if (!BsonClassMap.IsClassMapRegistered(typeof(TType)))
            {
                BsonClassMap.RegisterClassMap<TType>(cm =>
                {
                    cm.AutoMap();
                    cm.MapIdMember(c => c.Id);
                });
            }
        }
        
        private void mapEntityNoId<TType>() where TType : IEntityNoId
        {
            if (!BsonClassMap.IsClassMapRegistered(typeof(TType)))
            {
                BsonClassMap.RegisterClassMap<TType>(cm =>
                {
                    cm.AutoMap();
                });
            }
        }

        public IMongoCollection<User> Users => _database.GetCollection<User>(_userCollection);
        public IMongoCollection<UserProfile> UserProfiles => _database.GetCollection<UserProfile>(_userProfileCollection);
        public IMongoCollection<ApiKey> ApiKeys => _database.GetCollection<ApiKey>(_apiKeyCollection);

        private void CreateCollections()
        {
            if (!CheckCollection(_userCollection))
                _database.CreateCollection(_userCollection);
            if (!CheckCollection(_userProfileCollection))
                _database.CreateCollection(_userProfileCollection);
            if (!CheckCollection(_apiKeyCollection))
                _database.CreateCollection(_apiKeyCollection);
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