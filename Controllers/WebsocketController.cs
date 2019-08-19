using Amazon.Lambda.Core;
using Amazon.Lambda.APIGatewayEvents;
using System;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using DataModels;
using Models;

namespace DemoWebsocket
{
    public class AWSWebsocket
    {
        private WebsocketDataModel wsdm = new WebsocketDataModel();
        
        public async Task<APIGatewayProxyResponse> Connect(APIGatewayProxyRequest request, ILambdaContext context)
        {
            try
            {
                var connectionId = request.RequestContext.ConnectionId;
                var requestBody = JObject.FromObject(request).ToString();
                
                WSConnection wsData = new WSConnection();
                wsData.connectionId = connectionId;
                wsData.requestBody = requestBody;

                await wsdm.connect(wsData);
                
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
