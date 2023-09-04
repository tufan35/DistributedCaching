
using StackExchange.Redis;

var connectiob = await ConnectionMultiplexer.ConnectAsync("localhost:6379");
var subscriber = connectiob.GetSubscriber();

while (true)
{
    Console.WriteLine("Mesaj");
    string mesj = Console.ReadLine();
    await subscriber.PublishAsync("mychannel", mesj);

}