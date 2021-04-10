using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redis.Data.Extensions
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddRedisDB(this IServiceCollection services)
        {
            return AddRedisDB(services, "127.0.0.1:6379");
        }
        public static IServiceCollection AddRedisDB(this IServiceCollection services, string connectionString)
        {
            services.AddTransient<IRedisContext, RedisContext>();
            return services.AddSingleton<IConnectionMultiplexer>((sp) => ConnectionMultiplexer.Connect(connectionString));
        }
    }
}
