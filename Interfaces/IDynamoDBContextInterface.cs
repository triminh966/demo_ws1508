using System.Threading.Tasks;

public interface IDynamoDbContextInterface
{
    Task<T> GetByIdAsync<T>(int id);
    //Task<T> GetAsync(T item);
    void SetAsync<T>(T item);
    void DeleteAsync<T>(T item);
}