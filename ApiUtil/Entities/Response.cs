using System.Collections.Generic;
using Newtonsoft.Json;

namespace ApiUtil.Entities
{
    /// <summary>
    /// 返回页数据信息
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PageData<T>
    {
        /// <summary>
        /// 数据行
        /// </summary>
        [JsonProperty(PropertyName = "list")]
        public List<T> Rows { get; set; } = new List<T>();

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty(PropertyName = "total")]
        public long Totals { get; set; }
    }

    /// <summary>
    /// 封装返回json格式
    /// 正常信息
    /// </summary>
    public class ResponseInfo : ResponseError
    {
        /// <summary>
        /// 
        /// </summary>
        public ResponseInfo() : base("OK", 200) { }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty(PropertyName = "result")]
        public object Result { get; set; }
    }

    /// <summary>
    /// 封装返回json格式
    /// 异常信息
    /// </summary>
    public class ResponseError
    {
        /// <summary>
        /// 
        /// </summary>
        public ResponseError(string message = "Error", int code = 400)
        {
            Code = code;
            Message = message;
        }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty(PropertyName = "code")]
        public int Code { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty(PropertyName = "message")]
        public string Message { get; set; }
    }
}
