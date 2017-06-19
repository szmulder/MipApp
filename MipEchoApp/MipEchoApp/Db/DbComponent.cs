using System;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;

namespace MipEchoApp.Db
{
    public class DbComponent
    {
        protected DynamoDBContext Context { get; set; }
        public DbComponent()
        {
            var awsCredentials = ConfigSettings.GetAwsCredentials();
            var awsCredentialDb = new AwsCredentialComponent(awsCredentials);
            var region = RegionEndpoint.GetBySystemName(awsCredentials.RegionEndpoint);

            var dbConfig = new AmazonDynamoDBConfig { RegionEndpoint = region };

            var sessionCredentials = awsCredentialDb.GetAwsCredential();
            Context = new DynamoDBContext(new AmazonDynamoDBClient(sessionCredentials, dbConfig));
        }

        public void Update(RobotTable entity)
        {
            entity.id = 2;
            entity.LastModifiedUtc = DateTime.UtcNow.ToString("u");

            Context.SaveAsync(entity).Wait();
        }

        public RobotTable Get()
        {
            var id = 2;

            return Context.LoadAsync<RobotTable>(id, null).Result;
        }
    }
}
