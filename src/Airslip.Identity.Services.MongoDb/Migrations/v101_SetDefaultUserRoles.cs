using Airslip.Common.Auth.Data;
using Airslip.Common.Services.MongoDb;
using Airslip.Common.Types.Enums;
using Airslip.Common.Utilities.Extensions;
using Airslip.Identity.Api.Contracts.Data;
using Airslip.Identity.Api.Contracts.Entities;
using MongoDB.Driver;
using MongoDB.Bson;
using MongoDBMigrations;
using System.Text;
using System;
using System.Linq;
using Version = MongoDBMigrations.Version;

namespace Airslip.SmartReceipts.Services.MongoDb.Migrations
{
    public class v101_SetDefaultUserRoles : IMigration
    {
        public Version Version => new Version(1,0,1);
        public string Name => "Update match type to use new string based field name.";
        
        public void Up(IMongoDatabase database)
        {
            var collectionName = DeriveCollectionName<User>();

            var collection = database.GetCollection<BsonDocument>(collectionName);
            var list = collection.Find(FilterDefinition<BsonDocument>.Empty).ToList();
            
            FieldDefinition<BsonDocument, string> fieldDefenition = "userRole";
            foreach (var item in list)
            {
                string? currentUserRole = item.Names.Contains("userRole") ? item["userRole"].ToString() : null;
                AirslipUserType userType = Enum
                    .Parse<AirslipUserType>(item["airslipUserType"].ToString() ?? "Standard");
                string defaultRole = (userType == AirslipUserType.Merchant ? UserRoles.Administrator : UserRoles.User);
                collection.UpdateOne(new BsonDocument("_id", item["_id"]),
                    Builders<BsonDocument>.Update.Set(fieldDefenition, currentUserRole ?? defaultRole));
            }
        }

        public void Down(IMongoDatabase database)
        {
            
        }
        
        private static string DeriveCollectionName<TType>() => (typeof (TType).Name + "s").ToCamelCase();
    }
}