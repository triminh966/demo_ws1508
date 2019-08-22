using Services;
using Models;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Amazon.DynamoDBv2.Model;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.DataModel;
using System.Collections.Generic;

namespace DataModels
{
    public class WebsocketDataModel
    {
        public const string TABLE_NAME = Constants.WS_TABLE;
        private DynamoDBContextService wsContext = DynamoDBContextService.Instance;

        public async Task<WSConnection> subcribe(WSConnection item)
        {
            LambdaLogger.Log("Received object version: " + item.connectionId);
            LambdaLogger.Log("Received object request: " + item.requestBody);
            await wsContext.SetAsync<WSConnection>(item);
            return item;
        }

        public async Task<List<WSConnection>> scanAllSubcribers()
        {
            var condition = new ScanCondition("connectionId", ScanOperator.IsNotNull);
            var listCondition = new List<ScanCondition>();
            listCondition.Add(condition);

            return await wsContext.ScanAsync<WSConnection>(listCondition);
        }

        public async Task<List<WSConnection>> scanSubcribers(List<ScanCondition> conditions)
        {
            return await wsContext.ScanAsync<WSConnection>(conditions);
        }

        public async Task<WSConnection> deleteSubcriber(WSConnection con)
        {
            return await wsContext.DeleteAsync(con);
        }
    }
}