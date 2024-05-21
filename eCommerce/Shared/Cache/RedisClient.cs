﻿using System.Text.Json;
using StackExchange.Redis;

namespace Cache;

public class RedisClient
{
    private readonly string _serviceName;
    private readonly string _password;

    private ConnectionMultiplexer redis;

    public RedisClient(string serviceName,string password)
    {
        _serviceName = serviceName;
        _password = password;
    }

    public void Connect()
    {
        string connectionString = $"{_serviceName},password={_password}";
        redis = ConnectionMultiplexer.Connect(connectionString);
    }

    private IDatabase GetDatabase()
    {
        return redis.GetDatabase();
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