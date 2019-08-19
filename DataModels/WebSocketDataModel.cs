using Services;
using Models;
using System.Threading.Tasks;
using Amazon.Lambda.Core;

namespace DataModels
{
    public class WebsocketDataModel
    {
        public const string TABLE_NAME = Constants.WS_TABLE;
        private DynamoDBContextService wsContext = DynamoDBContextService.Instance;

        public async Task<WSConnection> connect(WSConnection item)
        {
            LambdaLogger.Log("Received object version: " + item.connectionId);
            LambdaLogger.Log("Received object request: " + item.requestBody);
            await wsContext.SetAsync<WSConnection>(item);
            return item;
        }
    }
}