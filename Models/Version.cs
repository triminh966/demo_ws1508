using Amazon.DynamoDBv2.DataModel;

namespace Models
{
    [DynamoDBTable("Version")]
    public class ApplicationVersion
    {
        [DynamoDBHashKey]
        public int id { get; set; }

        [DynamoDBHashKey]
        public string applicationId { get; set; }

        public string version { get; set; }
    }
}