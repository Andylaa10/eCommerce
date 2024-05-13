namespace Cache;

public static class RedisClientFactory
{
    public static RedisClient CreateRedisClient()
    {
        return new RedisClient("redis","");
    }
}