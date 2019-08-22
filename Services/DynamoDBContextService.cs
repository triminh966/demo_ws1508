using System;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using System.Collections.Generic;

namespace Services
{
    public class DynamoDBContextService : IDynamoDbContextInterface
    {
        private static Amazon.DynamoDBv2.DataModel.DynamoDBContext context;
        private static DynamoDBContextService instance;
        private static readonly object padlock = new object();
        private DynamoDBContextService()
        {
        }

        public static DynamoDBContextService Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        var client = new AmazonDynamoDBClient();
                        context = new Amazon.DynamoDBv2.DataModel.DynamoDBContext(client);
                        instance = new DynamoDBContextService();
                    }

                    return instance;
                }
            }
        }

        public async Task<T> GetByIdAsync<T>(int id)
        {
            try
            {
                return await context.LoadAsync<T>(id);
            }
            catch (Exception ex)
            {
                throw new Exception($"Amazon error in Get operation! Error: {ex}");
            }
        }

        public async Task<T> SetAsync<T>(T item)
        {
            try
            {
                await context.SaveAsync<T>(item);
                return item;

            }
            catch (Exception ex)
            {
                throw new Exception($"Amazon error in Write operation! Error: {ex}");
            }
        }

        public async Task<List<T>> ScanAsync<T>(List<ScanCondition> item)
        {
            try
            {
                return await context.ScanAsync<T>(item).GetRemainingAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Amazon error in Scan operation! Error: {ex}");
            }
        }

        public async Task<T> DeleteAsync<T>(T item)
        {
            try
            {
                await context.DeleteAsync(item);
                return item;
            }
            catch (Exception ex)
            {
                throw new Exception($"Amazon error in Delete operation! Error: {ex}");
            }
        }
    }
}