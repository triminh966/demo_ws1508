using Services;
using Models;
using System.Threading.Tasks;

namespace DataModels
{
    public class VersionDataModel
    {
        public const string TABLE_NAME = Constants.VERSION_TABLE;
        private DynamoDBContextService versionContext = DynamoDBContextService.Instance;

        public Task<VersionModel> getVersion(int id)
        {
            return versionContext.GetByIdAsync<VersionModel>(id);
        }

        public Task<VersionModel> updateVersion(VersionModel vm)
        {
            return versionContext.SetAsync<VersionModel>(vm);
        }

        public Task<VersionModel> deleteVersion(VersionModel vm)
        {
            return versionContext.DeleteAsync<VersionModel>(vm);
        }
    }
}