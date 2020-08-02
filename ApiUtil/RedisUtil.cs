using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Redis;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace ApiUtil
{
    /// <summary>
    /// 
    /// </summary>
    public class RedisUtil
    {
        private readonly RedisCache _redisCache;
        private readonly ILogger<RedisUtil> _logger;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        public RedisUtil(ILogger<RedisUtil> logger)
        {
            _logger = logger;
            //获取配置项信息
            var options = new RedisCacheOptions { Configuration = ConfigUtil.RedisConnectionString };//, InstanceName = ConfigUtil.RedisInstanceName
            //初始化Redis
            if (ConfigUtil.IsOpenCache) _redisCache = new RedisCache(options);
        }

        /// <summary>
        /// 添加数据
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <param name="expireTime">过期时间</param>
        public async Task Add(string key, object value, int expireTime = 10)
        {
            if (string.IsNullOrEmpty(key)) return;
            var strValue = string.Empty;
            try
            {
                strValue = JsonConvert.SerializeObject(value);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Redis.Add转换失败:{ex.Message}");
            }
            if (!string.IsNullOrEmpty(strValue))
            {
                await _redisCache.SetStringAsync(key, strValue,
                    new DistributedCacheEntryOptions {AbsoluteExpiration = DateTime.Now.AddMinutes(expireTime)});
            }
        }

        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns></returns>
        public async Task<string> Get(string key, string defaultValue = "")
        {
            if (string.IsNullOrEmpty(key)) return defaultValue;
            var value = await _redisCache.GetStringAsync(key);
            if (string.IsNullOrEmpty(value)){ value = defaultValue; }
            return value;
        }

        /// <summary>
        /// 获取数据（对象）
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="key">键</param>
        /// <returns></returns>
        public async Task<T> Get<T>(string key)
        {
            var value = await Get(key);
            if (string.IsNullOrEmpty(value)) return default(T);
            var obj = default(T);
            try
            {
                obj = JsonConvert.DeserializeObject<T>(value);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Redis.Get转换失败：{ex.Message},数据：{value}");
            }
            return obj;
        }

        /// <summary>
        /// 移除数据
        /// </summary>
        /// <param name="key">键</param>
        public async Task Remove(string key)
        {
            if (!string.IsNullOrEmpty(key))
                await _redisCache.RemoveAsync(key);
        }

        /// <summary>
        /// 刷新数据
        /// </summary>
        /// <param name="key">键</param>
        public async Task Refresh(string key)
        {
            if (!string.IsNullOrEmpty(key))
                await _redisCache.RefreshAsync(key);
        }

        /// <summary>
        /// 重置数据
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <param name="expireTime">过期时间</param>
        public async Task Replace(string key, object value, int expireTime = 10)
        {
            if (string.IsNullOrEmpty(key)) return;
            await Remove(key);
            await Add(key, value, expireTime);
        }
    }
}
