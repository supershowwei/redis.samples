using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StackExchange.Redis;

namespace RedisSamples
{
    [TestClass]
    public class RedisMessageBrokerTest
    {
        [TestMethod]
        public void Test_CreateRedisConnection()
        {
            var redisConn = this.CreateRedisConnection("localhost", 6379);
        }

        [TestMethod]
        public void Test_Subscribe()
        {
            var redisConn = this.CreateRedisConnection("localhost", 6379);

            this.Subscribe(redisConn, "dotblogs");
        }

        [TestMethod]
        public void Test_Publish()
        {
            var redisConn = this.CreateRedisConnection("localhost", 6379);

            this.Publish(redisConn, "dotblogs", "Hello Dotblogs");
        }

        private ConnectionMultiplexer CreateRedisConnection(string host, int port)
        {
            return ConnectionMultiplexer.Connect($"{host}:{port}");
        }

        private void Subscribe(ConnectionMultiplexer redisConn, string channelName)
        {
            // 建立 Subscriber Instance
            var redisSubscriber = redisConn.GetSubscriber();

            // 訂閱名為 dotblogs 的頻道並宣告處理方式
            redisSubscriber.Subscribe(channelName, (channel, message) =>
            {
                string dotblogsMessage = (string)message;
            });
        }

        private void Publish(ConnectionMultiplexer redisConn, string channelName, string message)
        {
            // 建立 Subscriber Instance
            var redisSubscriber = redisConn.GetSubscriber();

            // 發佈 hello dotblogs 訊息到 dotblogs Channel 上。
            redisSubscriber.Publish(channelName, message);
        }
    }
}
