using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using DataModels;
using Models;
using Amazon.Lambda.Core;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.ApiGatewayManagementApi;
using Amazon.ApiGatewayManagementApi.Model;
using Amazon.Runtime;
using Amazon.Lambda.DynamoDBEvents;

namespace AWSVersion
{
    public class Version
    {
        private VersionDataModel vm = new VersionDataModel();
        public async Task<ApplicationVersion> UpdateVersion(ApplicationVersion version)
        {
            LambdaLogger.Log("Recived object version: " + version.version);
            return await vm.updateVersion(version);
        }

        public async Task<ApplicationVersion> GetVersion(int version)
        {
            LambdaLogger.Log("Recived version: " + version);
            return await vm.getVersion(version);
        }

        public async Task<List<ApplicationVersion>> GetVerionByApplication(ApplicationVersion version)
        {
            return await vm.getVersionByCondition(version);
        }

        public void NotifyStream()
        {
            Console.WriteLine("DynamoDB is calling");
        }
    }
}
