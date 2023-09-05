using StackExchange.Redis;

namespace CachingDistributed.Services
{
    public class RedisService
    {
        /// <summary>
        /// Redis sentine lredis ver,tabanın master sunucusu düşmesi durumunda baska bir yedek sunucunun otomatik olarak yerine geçmesini   veri hizmetlrinin kesitnsiiz devam etmesini sağlar
        /// sentinel master sunucunun durumunu izlemek ve değişiklikeri algılamak diğer sentinel sunucularoyla birlikte calısmaktadır
        /// sentinel mimarisinde birkac sinetinel sunucusuyla calıslması inerilmektedir.  bu sayede kesintisiz sunucular görevini yapabilecektir.  
        /// </summary>

        static ConfigurationOptions sentinelOps => new()
        {
            EndPoints =
            {
                {"localhost",6383 },
                {"localhost",6384 },
                {"localhost",6385 }

            },
            CommandMap = CommandMap.Sentinel,
            AbortOnConnectFail = false
        };
        static ConfigurationOptions masterOps => new()
        {
            AbortOnConnectFail = false
        };


        public static async Task<IDatabase> RedisMasterDb()
        {
            ConnectionMultiplexer sentinelConn = await ConnectionMultiplexer.SentinelConnectAsync(sentinelOps);
            System.Net.EndPoint masterEndpoitn = null;
            foreach (var endPoint in sentinelConn.GetEndPoints())
            {


                IServer server = sentinelConn.GetServer(endPoint);
                if (!server.IsConnected)
                    continue;
                masterEndpoitn = await server.SentinelGetMasterAddressByNameAsync("mymaster");
                break;
            }

            ///dockerize edilen ipler 
            var localMasterIp = masterEndpoitn.ToString() switch
            {
                "172.18.0.2:6379" => "localhost:6379",
                "172.18.0.3:6379" => "localhost:6380",
                "172.18.0.4:6379" => "localhost:6381",
                "172.18.0.5:6379" => "localhost:6382",
            };

            ConnectionMultiplexer masterConn = await ConnectionMultiplexer.ConnectAsync(localMasterIp);
            var database = masterConn.GetDatabase();
            return database;
        }
    }
}
