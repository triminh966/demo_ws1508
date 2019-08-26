using Amazon.Lambda.Core;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.ApiGatewayManagementApi;
using Amazon.ApiGatewayManagementApi.Model;
using Amazon.Runtime;
using Amazon.Lambda.DynamoDBEvents;
using Amazon.DynamoDBv2.Model;
using System;
using System.IO;
using System.Text;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using DataModels;
using Models;
using System.Collections.Generic;

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

                return await _broadcast(scanResponse, apiClient);
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

        private async Task<APIGatewayProxyResponse> _broadcast(List<WSConnection> list, AmazonApiGatewayManagementApiClient client)
        {
            var stream = new MemoryStream(UTF8Encoding.UTF8.GetBytes("Hello from the other side"));
            var count = 0;
                foreach (var item in list)
                {
                    var connectionId = item.connectionId;
                    var postConnectionRequest = new PostToConnectionRequest
                    {
                        ConnectionId = connectionId,
                        Data = stream
                    };

                    try
                    {
                        LambdaLogger.Log($"Post to connection {count}: {connectionId}");
                        stream.Position = 0;
                        await client.PostToConnectionAsync(postConnectionRequest);
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
                            LambdaLogger.Log($"Deleting gone connection: {connectionId}");
                            await wsdm.deleteSubcriber(wsConnection);
                        }
                        else
                        {
                            LambdaLogger.Log($"Error posting message to {connectionId}: {e.Message}");
                            LambdaLogger.Log(e.StackTrace);
                        }
                    }
                }
                
                return new APIGatewayProxyResponse
                {
                    StatusCode = 200,
                    Body = "Connected."
                };
        }
         
        public async Task<APIGatewayProxyResponse> StreamReceiver(DynamoDBEvent dynamoEvent, ILambdaContext context)
        {
            var scanResponse = await wsdm.scanAllSubcribers();
                var apiClient = new AmazonApiGatewayManagementApiClient(new AmazonApiGatewayManagementApiConfig
                {
                    ServiceURL = "hhtps://gqafvi5306.execute-api.us-east-1.amazonaws.com/dev/"
                });
            return await _broadcast(scanResponse, apiClient);
        }
    }
}
