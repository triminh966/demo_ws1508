using System;
using Models;
using System.Threading.Tasks;

namespace Services
{
    public class DynamoDBServiceContext
    {
        private IDynamoDbContext<VersionModel> vesionContext;

        public DynamoDBServiceContext(IDynamoDbContext<VersionModel> versionContext)
        {
            vesionContext = versionContext;
        }

        public async Task<VersionModel> GetVersionAsync(string id)
        {
            try
            {
                return await vesionContext.GetByIdAsync(id);
            }
            catch (Exception ex)
            {
                throw new Exception($"Amazon error in GetUser table operation! Error: {ex}");
            }
        }
    }
}