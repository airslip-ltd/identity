using Airslip.Common.Auth.Enums;
using Airslip.Common.Repository.Enums;
using Airslip.Common.Repository.Interfaces;
using Airslip.Common.Types.Extensions;
using Airslip.Identity.Api.Contracts.Entities;
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
            
            // Enum as string
            BsonSerializer.RegisterSerializer(new EnumSerializer<ApiKeyUsageType>(BsonType.String));
            
            // Map classes
            mapEntityWithId<ApiKey>();
            mapEntityWithId<User>();
            mapEntityWithId<UserProfile>();
            mapEntityNoId<RefreshToken>();
            mapEntityNoId<OpenBankingProvider>();
            
            // Ensure collections exist
            CreateCollection<ApiKey>();
            CreateCollection<User>();
            CreateCollection<UserProfile>();
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

        public IMongoCollection<User> Users => CollectionByType<User>();
        public IMongoCollection<UserProfile> UserProfiles => CollectionByType<UserProfile>();
        public IMongoCollection<ApiKey> ApiKeys => CollectionByType<ApiKey>();

        public IMongoCollection<TType> CollectionByType<TType>()
        {
            return _database.GetCollection<TType>(DeriveCollectionName<TType>());
        }

        private void CreateCollection<TType>()
        {
            string collectionName = DeriveCollectionName<TType>();
            if (!CheckCollection(collectionName))
                _database.CreateCollection(collectionName);
        }

        private bool CheckCollection(string collectionName)
        {
            BsonDocument filter = new("name", collectionName);
            IAsyncCursor<BsonDocument>? collectionCursor =
                _database.ListCollections(new ListCollectionsOptions {Filter = filter});
            return collectionCursor.Any();
        }

        private static string DeriveCollectionName<TType>()
        {
            return $"{typeof(TType).Name}s".ToCamelCase();
        }
    }
}