using Amazon.DynamoDBv2.DataModel;

namespace Models 
{
     [DynamoDBTable("Version")]
    public class VersionModel 
    {
        [DynamoDBHashKey]
        public int id { get; set; }

        public string version { get; set; }  
    }
}