using Services;
using Models;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.DataModel;
using System.Collections.Generic;

namespace DataModels
{
    public class VersionDataModel
    {
        public const string TABLE_NAME = Constants.VERSION_TABLE;
        private DynamoDBContextService versionContext = DynamoDBContextService.Instance;

        public Task<ApplicationVersion> getVersion(int id)
        {
            return versionContext.GetByIdAsync<ApplicationVersion>(id);
        }

        public Task<List<ApplicationVersion>> getVersionByCondition(ApplicationVersion app)
        {
            var listCondition = new List<ScanCondition>();
            if (app.id >= 0) {
                listCondition.Add(new ScanCondition("id", ScanOperator.Equal, app.id));
            }
            if (app.applicationId != null) {
                listCondition.Add(new ScanCondition("applicationId", ScanOperator.Equal, app.applicationId));
            }

            return versionContext.ScanAsync<ApplicationVersion>(listCondition);
        }

        public Task<ApplicationVersion> updateVersion(ApplicationVersion vm)
        {
            return versionContext.SetAsync<ApplicationVersion>(vm);
        }

        public Task<ApplicationVersion> deleteVersion(ApplicationVersion vm)
        {
            return versionContext.DeleteAsync<ApplicationVersion>(vm);
        }
    }
}