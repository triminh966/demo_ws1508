using Amazon.DynamoDBv2.DataModel;

namespace Models 
{
     [DynamoDBTable("WSConnection")]
    public class WSConnection 
    {
        [DynamoDBHashKey]
        public string connectionId { get; set; }

        public string requestBody { get; set; }  
    }
}