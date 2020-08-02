using System.Collections.Generic;
using Newtonsoft.Json;
using WebApiClient;
using WebApiClient.Attributes;

namespace ApiUtil.HttpApi
{
    /// <summary>
    /// Token信息
    /// </summary>
    public class TokenInfo
    {
        /// <summary>
        /// 用户id
        /// </summary>
        [JsonProperty("userId")]
        public int UserId { get; set; }

        /// <summary>
        /// 工号
        /// </summary>
        [JsonProperty("userNumber")]
        public string UserNumber { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        [JsonProperty("userName")]
        public string UserName { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class DictObject
    {
        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("code")]
        public int Code { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("result")]
        public List<DictValue> Result { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("message")]
        public string Message { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class DictValue
    {
        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("id")]
        public int Id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("dicCode")]
        public string DicCode { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("codeDsc")]
        public string CodeDsc { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("parentId")]
        public int ParentId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("createTime")]
        public string CreateTime { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("createBy")]
        public object CreateBy { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("remark")]
        public object Remark { get; set; }
    }


    /// <summary>
    /// 
    /// </summary>
    [HttpHost("http://127.0.0.1:80")]
    public interface IPlatformApi : IHttpApi
    {
        /// <summary>
        /// 获取Token信息
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpGet("adminv1/token/getTokenInfo")]
        ITask<TokenInfo> GetTokenInfoAsync(string token);

        /// <summary>
        /// 获取
        /// </summary>
        /// <param name="dicType"></param>
        /// <returns></returns>
        [HttpGet("adminv1/dic/getDicTypeList")]
        ITask<DictObject> GetDictObjectAsync(string dicType);
    }

}
