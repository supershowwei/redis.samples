using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StackExchange.Redis;

namespace RedisSamples
{
    [TestClass]
    public class RedisCacheSampleTest
    {
        [TestMethod]
        public void Test_CreateRedisConnection()
        {
            var redisConn = this.CreateRedisConnection("localhost", 6379);
        }

        [TestMethod]
        public void Test_SetLimitedCache()
        {
            var redisConn = this.CreateRedisConnection("localhost", 6379);

            this.SetLimitedCache(redisConn, "Hello", "World");
        }

        [TestMethod]
        public void Test_GetCache()
        {
            var redisConn = this.CreateRedisConnection("localhost", 6379);

            this.SetLimitedCache(redisConn, "Hello", "World");

            var cacheData = this.GetCache(redisConn, "Hello");

            Assert.AreEqual("World", cacheData);
        }

        private ConnectionMultiplexer CreateRedisConnection(string host, int port)
        {
            return ConnectionMultiplexer.Connect($"{host}:{port}");
        }

        private void SetLimitedCache(ConnectionMultiplexer redisConn, string key, string value)
        {
            // 取得 Redis Database
            var redisDatabase = redisConn.GetDatabase();

            // 快取 10 秒後過期字串資料
            redisDatabase.StringSet(key, value, TimeSpan.FromSeconds(10));
        }

        private string GetCache(ConnectionMultiplexer redisConn, string key)
        {
            // 取得 Redis Database
            var redisDatabase = redisConn.GetDatabase();

            // 取得快取資料
            var value = redisDatabase.StringGet(key);

            return value;
        }
    }
}
