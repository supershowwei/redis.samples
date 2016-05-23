using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ServiceStack.Redis;

namespace RedisSamples
{
    [TestClass]
    public class UnitTest1
    {
        private IRedisClient redisClient;

        [TestMethod]
        public void TestMethod1()
        {
            this.redisClient = this.CreateRedisClient();

            this.SetCache("hello", "world");

            var cacheData = this.GetCache("hello");

            this.Subscribe("dotblogs");

            this.Publish("dotblogs", "helloworld");
        }

        private IRedisClient CreateRedisClient()
        {
            var redisClientManager = new PooledRedisClientManager("127.0.0.1:6379");
            redisClientManager.FailoverTo("127.0.0.1:6380");

            return redisClientManager.GetClient();
        }

        private void SetCache(string key, string value)
        {
            // 快取 Key 為 "hello"，Value 為 "world" 的資料，而且 10 秒後過期。
            this.redisClient.As<string>().SetValue(key, value, TimeSpan.FromSeconds(10));
        }

        private string GetCache(string key)
        {
            // 取得 Key 為 "hello" 的 Value。
            return this.redisClient.As<string>().GetValue(key);
        }

        private void Subscribe(string channelName)
        {
            IRedisSubscription subscription = redisClient.CreateSubscription();

            // 註冊接收訊息事件
            subscription.OnMessage = (channel, msg) =>
            {
                Console.WriteLine(msg);
            };

            subscription.OnUnSubscribe = (channel) =>
            {
                Console.WriteLine("OnUnSubscribe");
            };

            Task.Run(() =>
            {
                // 由於 SubscribeToChannels 這個方法會使得 Thread 被 Blocked，
                // 因此需要將 SubscribeToChannels 放在背景執行。
                subscription.SubscribeToChannels(channelName);
            });
        }

        private void Publish(string channelName, string message)
        {
            this.redisClient.PublishMessage(channelName, message);
        }
    }
}
