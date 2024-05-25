using System.Text.Json;
using StackExchange.Redis;

namespace Cache;

public interface IRedisClient
{
    void Connect();
    IDatabase GetDatabase();
    Task StoreValue(string key, string value);
    Task<string?> GetValue(string key);
    Task RemoveValue(string key);
    string SerializeObject<T>(T obj);
    T? DeserializeObject<T>(string json);
}