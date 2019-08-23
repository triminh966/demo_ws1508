using Amazon.Lambda.Core;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.ApiGatewayManagementApi;
using Amazon.ApiGatewayManagementApi.Model;
using Amazon.Runtime;
using System;
using System.IO;
using System.Text;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using DataModels;
using Models;

namespace DemoWebsocket
{
    public class AWSWebsocket
    {
        private WebsocketDataModel wsdm = new WebsocketDataModel();

        public async Task<APIGatewayProxyResponse> Subcribe(APIGatewayProxyRequest request, ILambdaContext context)
        {
            try
            {
                var requestBody = JObject.FromObject(request).ToString();
                LambdaLogger.Log("Info connecting: " + requestBody);

                WSConnection wsData = new WSConnection();
                wsData.connectionId = request.RequestContext.ConnectionId;
                wsData.requestBody = requestBody;

                await wsdm.subcribe(wsData);

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

        public async Task<APIGatewayProxyResponse> Broadcast(APIGatewayProxyRequest request, ILambdaContext context)
        {
            LambdaLogger.Log(JObject.FromObject(request).ToString());
            try
            {
                var domainName = request.RequestContext.DomainName;
                var stage = request.RequestContext.Stage;
                var endpoint = $"https://{domainName}/{stage}";
                LambdaLogger.Log("API Gateway management endpoint:" + endpoint);
                var message = JsonConvert.DeserializeObject<JObject>(request.Body);
                var data = message["data"]?.ToString();

                var stream = new MemoryStream(UTF8Encoding.UTF8.GetBytes(data));

                var scanResponse = await wsdm.scanAllSubcribers();
                var apiClient = new AmazonApiGatewayManagementApiClient(new AmazonApiGatewayManagementApiConfig
                {
                    ServiceURL = endpoint
                });

                var count = 0;
                foreach (var item in scanResponse)
                {
                    var connectionId = item.connectionId;
                    var postConnectionRequest = new PostToConnectionRequest
                    {
                        ConnectionId = connectionId,
                        Data = stream
                    };

                    try
                    {
                        context.Logger.LogLine($"Post to connection {count}: {connectionId}");
                        stream.Position = 0;
                        await apiClient.PostToConnectionAsync(postConnectionRequest);
                        count++;
                    }
                    catch (AmazonServiceException e)
                    {
                        // API Gateway returns a status of 410 GONE when the connection is no
                        // longer available. If this happens, we simply delete the identifier
                        // from our DynamoDB table.
                        if (e.StatusCode == HttpStatusCode.Gone)
                        {
                            var wsConnection = new WSConnection();
                            wsConnection.connectionId = connectionId;
                            context.Logger.LogLine($"Deleting gone connection: {connectionId}");
                            await wsdm.deleteSubcriber(wsConnection);
                        }
                        else
                        {
                            context.Logger.LogLine($"Error posting message to {connectionId}: {e.Message}");
                            context.Logger.LogLine(e.StackTrace);
                        }
                    }
                }

                return new APIGatewayProxyResponse
                {
                    StatusCode = 200,
                    Body = "Connected."
                };
            }
            catch (Exception e)
            {
                LambdaLogger.Log("Error disconnecting: " + e.Message);
                LambdaLogger.Log(e.StackTrace);
                return new APIGatewayProxyResponse
                {
                    StatusCode = 500,
                    Body = $"Failed to send message: {e.Message}"
                };
            }
        }

        public async Task<APIGatewayProxyResponse> Disconnect(APIGatewayProxyRequest request, ILambdaContext context)
        {
            try
            {
                var requestBody = JObject.FromObject(request).ToString();
                LambdaLogger.Log("Info connecting: " + requestBody);
                var connectionId = request.RequestContext.ConnectionId;

                WSConnection wsData = new WSConnection();
                wsData.connectionId = connectionId;
                wsData.requestBody = requestBody;

                await wsdm.subcribe(wsData);

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


        private static readonly JsonSerializer JsonSerializer = new JsonSerializer();

        public Task StreamReceiver(DynamoDBEvent dynamoEvent, ILambdaContext context)
        {
            LambdaLogger.Log(JObject.FromObject(dynamoEvent).ToString());
            LambdaLogger.Log(JObject.FromObject(context).ToString());

            return Task.CompletedTask;
        }

        private static string SerializeStreamRecord(StreamRecord streamRecord)
        {
            using (var writer = new StringWriter())
            {
                JsonSerializer.Serialize(writer, streamRecord);
                return writer.ToString();
            }
        }
    }
}
