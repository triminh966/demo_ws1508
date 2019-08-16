using Amazon.Lambda;
using Amazon.DynamoDBv2.Model;
using Amazon.DynamoDBv2;
using Amazon.Lambda.Core;
using Amazon.Lambda.APIGatewayEvents;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DemoWebsocket
{
    public class AWSWebsocket
    {
        IAmazonDynamoDB ddbClient = new AmazonDynamoDBClient();
        public async Task<APIGatewayProxyResponse> Connect(APIGatewayProxyRequest request, ILambdaContext context)
        {
            LambdaLogger.Log(JObject.FromObject(request).ToString());
            try
            {
                var connectionId = request.RequestContext.ConnectionId;
                LambdaLogger.Log("ConnectionId:" + connectionId);
                Dictionary<string, AttributeValue> attributes = new Dictionary<string, AttributeValue>();
                attributes["connectionIdc"] = new AttributeValue
                {
                    S = connectionId
                };
                PutItemRequest ddbRequest = new PutItemRequest()
                {
                    TableName = "WsConnection",
                    Item = attributes
                };
                await ddbClient.PutItemAsync(ddbRequest);
                return new APIGatewayProxyResponse
                {
                    StatusCode = 200,
                    Body = "Connected."
                };
            }
            catch (Exception e)
            {
                LambdaLogger.Log("Error connecting: " + e.Message);
                LambdaLogger.Log(e.StackTrace);
                return new APIGatewayProxyResponse
                {
                    StatusCode = 500,
                    Body = $"Failed to connecting: {e.Message}"
                };
            }
        }
        public void Disconnect()
        {
        }
    }
}
