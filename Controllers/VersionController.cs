using System;
using System.Threading.Tasks;
using Services;
using Models;
using Amazon.Lambda.Core;

namespace AWSVersion
{
    public class Version
    {
        private DynamoDBContext service = DynamoDBContext.Instance;
        public void UpdateVersion(VersionModel version)
        {
            LambdaLogger.Log("Recived object version: " + version.version);
            service.WriteAsync(version);
        }

        public async Task<VersionModel> GetVersion(int version) 
        {
            return await service.GetAsync(version);
        }

        public void NotifyStream()
        {
            Console.WriteLine("DynamoDB is calling");
        }
    }
}
