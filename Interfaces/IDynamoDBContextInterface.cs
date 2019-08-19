using System.Threading.Tasks;

public interface IDynamoDbContextInterface
{
    Task<T> GetByIdAsync<T>(int id);
    //Task<T> GetAsync(T item);
    Task<T> SetAsync<T>(T item);
    Task<T> DeleteAsync<T>(T item);
}