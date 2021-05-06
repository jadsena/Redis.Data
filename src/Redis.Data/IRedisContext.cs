using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redis.Data
{
    public interface IRedisContext
    {
        IDatabase Database { get; }

        bool Delete(string key);
        Task<bool> DeleteAsync(string key);
        string Get(string key);
        T Get<T>(string key);
        Dictionary<string, string> GetAll();
        Task<Dictionary<string, string>> GetAllAsync();
        Task<string> GetAsync(string key);
        Task<T> GetAsync<T>(string key);
        bool Set(string key, string value);
        Task<bool> SetAsync(string key, string value);
        void SetDatabase(int db = -1, object asyncState = null);
    }
}
