using System;
using System.Threading.Tasks;
using ApiUtil.HttpApi;
using ApiUtil.Log;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ApiUtil
{
    /// <summary>
    /// 服务间通信帮助类
    /// </summary>
    public class ServiceHelper
    {
        private readonly ILogger<ServiceHelper> _logger;
        private readonly IPlatformApi _httpApi;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="httpApi"></param>
        public ServiceHelper(ILogger<ServiceHelper> logger, [FromServices]IPlatformApi httpApi)
        {
            _logger = logger;
            _httpApi = httpApi;
        }

        /// <summary>
        /// 获取token中的信息
        /// </summary>
        /// <returns></returns>
        public async Task<TokenInfo> GetTokenInfo()
        {
            try
            {
                return await _httpApi.GetTokenInfoAsync(ConfigUtil.HttpCurrent.Request.Headers["Authorization"]);
            }
            catch (Exception e)
            {
                _logger.LogError(LogHelper.OutputClearness($"获取平台Token失败，获取用户信息失败,失败信息：{e.Message}"));
                return new TokenInfo{UserId = -1, UserName = "非平台用户", UserNumber = "非平台用户"};
            }
        }

        /// <summary>
        /// 获取平台字典中的信息
        /// </summary>
        /// <param name="dicType"></param>
        /// <returns></returns>
        public async Task<DictObject> GetDictObjectAsync(string dicType)
        {
            try
            {
                return await _httpApi.GetDictObjectAsync(dicType);
            }
            catch (Exception e)
            {
                _logger.LogError(LogHelper.OutputClearness($"获取平台字典失败,失败信息:{e.Message}"));
                return null;
            }
        }
    }
}
