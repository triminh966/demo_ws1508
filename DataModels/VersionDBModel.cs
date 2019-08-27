using Services;
using Models;
using System.Threading.Tasks;

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