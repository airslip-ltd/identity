using Airslip.Common.Repository.Entities;
using Airslip.Common.Services.MongoDb;
using Airslip.Common.Services.MongoDb.Extensions;
using Airslip.Identity.Api.Contracts.Entities;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using System.Threading.Tasks;

namespace Airslip.Identity.Infrastructure.MongoDb
{
    public static class Initialisation
    {
        public static Task InitialiseSettings(IMongoDatabase mongoDatabase)
        {
            // // Map classes
            AirslipMongoDbBase.MapEntity<RefreshToken>();
            AirslipMongoDbBase.MapEntity<OpenBankingProvider>();
            AirslipMongoDbBase.MapEntity<BasicAuditInformation>();
            
            mongoDatabase.CreateCollectionForEntity<ApiKey>();
            mongoDatabase.CreateCollectionForEntity<QrCode>();
            mongoDatabase.CreateCollectionForEntity<User>();
            
            return Task.CompletedTask;
        }
    }
}