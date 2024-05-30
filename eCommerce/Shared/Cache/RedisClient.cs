using System;  
using System.Text.Json;  
using StackExchange.Redis;  
using System.Threading.Tasks;  
  
namespace Cache  
{  
    public class RedisClient : IRedisClient  
    {  
        private readonly string _serviceName = "my-redis-master.redis.svc.cluster.local:6379";  
        private readonly string _password = "SuperSecret7!";  
        private ConnectionMultiplexer _redis;  
        
        public void Connect()  
        {  
            try  
            {  
                string connectionString = $"{_serviceName},password={_password},abortConnect=false";  
                _redis = ConnectionMultiplexer.Connect(connectionString);  
            }  
            catch (RedisConnectionException ex)  
            {  
                Console.WriteLine($"Error connecting to Redis: {ex.Message}");  
                throw;  
            }  
            catch (Exception ex)  
            {  
                Console.WriteLine($"Unexpected error: {ex.Message}");  
                throw;  
            }  
        }  
  
        public IDatabase GetDatabase()  
        {  
            return _redis.GetDatabase();  
        }  
  
        public async Task StoreValue(string key, string value)  
        {  
            var db = GetDatabase();  
            await db.StringSetAsync(key, value);  
        }  
  
        public async Task<string?> GetValue(string key)  
        {  
            var db = GetDatabase();  
            return await db.StringGetAsync(key);  
        }  
  
        public async Task RemoveValue(string key)  
        {  
            var db = GetDatabase();  
            await db.KeyDeleteAsync(key);  
        }  
  
        public string SerializeObject<T>(T obj)  
        {  
            return JsonSerializer.Serialize(obj);  
        }  
  
        public T? DeserializeObject<T>(string json)  
        {  
            return JsonSerializer.Deserialize<T>(json);  
        }  
    }  
}