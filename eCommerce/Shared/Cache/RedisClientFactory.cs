namespace Cache;

public static class RedisClientFactory
{
    public static IRedisClient CreateRedisClient()
    {
        return new RedisClient("redis",""); 
    }
}