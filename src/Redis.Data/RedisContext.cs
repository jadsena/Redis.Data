using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Redis.Data
{
    public class RedisContext : IRedisContext
    {
        private readonly IConnectionMultiplexer _connection;
        private readonly IDatabase _db;
        public RedisContext(IConnectionMultiplexer connection)
        {
            _connection = connection;
            _db = connection.GetDatabase();
        }
        public bool Set(string key, string value)
        {
            return _db.StringSet(key, value);
        }
        public string Get(string key)
        {
            return _db.StringGet(key);
        }
        public T Get<T>(string key)
        {
            string vlr = _db.StringGet(key);
            if (string.IsNullOrWhiteSpace(vlr)) return default;
            return JsonSerializer.Deserialize<T>(vlr);
        }
        public bool Delete(string key)
        {
            return _db.KeyDelete(key);
        }
        public async Task<bool> SetAsync(string key, string value)
        {
            return await _db.StringSetAsync(key, value);
        }
        public async Task<string> GetAsync(string key)
        {
            return await _db.StringGetAsync(key);
        }
        public async Task<bool> DeleteAsync(string key)
        {
            return await _db.KeyDeleteAsync(key);
        }
        public async Task<T> GetAsync<T>(string key)
        {
            string vlr = await _db.StringGetAsync(key);
            if (string.IsNullOrWhiteSpace(vlr)) return default;
            return JsonSerializer.Deserialize<T>(vlr);
        }
        public async Task<Dictionary<string, string>> GetAllAsync()
        {
            return await GetAllFromDataBaseAsync();
        }
        public Dictionary<string, string> GetAll()
        {
            return GetAllFromDataBase();
        }
        private Dictionary<string, string> GetAllFromDataBase(string sPattern = "*")
        {
            int dbName = _db.Database; //or 0, 1, 2
            Dictionary<string, string> dicKeyValue = new Dictionary<string, string>();
            var keys = _connection.GetServer(_connection.Configuration).Keys(dbName, pattern: sPattern);
            string[] keysArr = keys.Select(key => (string)key).ToArray();

            foreach (var key in keysArr)
            {
                dicKeyValue.Add(key, Get(key));
            }

            return dicKeyValue;
        }
        private async Task<Dictionary<string, string>> GetAllFromDataBaseAsync(string sPattern = "*")
        {
            int dbName = _db.Database; //or 0, 1, 2
            Dictionary<string, string> dicKeyValue = new Dictionary<string, string>();
            var keys = _connection.GetServer(_connection.Configuration).Keys(dbName, pattern: sPattern);
            string[] keysArr = keys.Select(key => (string)key).ToArray();

            foreach (var key in keysArr)
            {
                dicKeyValue.Add(key, await GetAsync(key));
            }

            return dicKeyValue;
        }
    }
}
