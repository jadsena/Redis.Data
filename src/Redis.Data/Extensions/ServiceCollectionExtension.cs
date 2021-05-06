using Redis.Data;
using Redis.Data.Options;
using StackExchange.Redis;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddRedisDB(this IServiceCollection services)
        {
            return AddRedisDB(services, "127.0.0.1:6379");
        }
        public static IServiceCollection AddRedisDB(this IServiceCollection services, string connectionString, int database = -1)
        {
            services.Configure<DatabaseOptions>((c) => c.Database = database );
            services.AddTransient<IRedisContext, RedisContext>();
            return services.AddSingleton<IConnectionMultiplexer>((sp) => ConnectionMultiplexer.Connect(connectionString));
        }
    }
}
