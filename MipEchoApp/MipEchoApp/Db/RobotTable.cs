using Amazon.DynamoDBv2.DataModel;

namespace MipEchoApp.Db
{
    [DynamoDBTable("RobotTable")]
    public class RobotTable
    {
        [DynamoDBHashKey]
        public int id { get; set; }

        [DynamoDBProperty]
        public string Action { get; set; }

        [DynamoDBProperty]
        public string LastModifiedUtc { get; set; }
    }
}
