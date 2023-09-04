
using StackExchange.Redis;

var connectiob = await ConnectionMultiplexer.ConnectAsync("localhost:6379");
var subscriber = connectiob.GetSubscriber();

await subscriber.SubscribeAsync("mychannel", (channel, message) =>
{
    Console.WriteLine(message);
});

Console.Read();