using System;
using System.Threading.Tasks;
using DataModels;
using Models;
using Amazon.Lambda.Core;

namespace AWSVersion
{
    public class Version
    {
        private VersionDataModel vm = new VersionDataModel();
        public void UpdateVersion(VersionModel version)
        {
            LambdaLogger.Log("Recived object version: " + version.version);
            vm.updateVersion(version);
        }

        public async Task<VersionModel> GetVersion(int version) 
        {
            return await vm.getVersion(version);
        }

        public void NotifyStream()
        {
            Console.WriteLine("DynamoDB is calling");
        }
    }
}
