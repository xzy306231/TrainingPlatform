using Newtonsoft.Json;

namespace ApiUtil.Entities
{
    /// <summary>
    /// 
    /// </summary>
    public class MessageEntity
    {
        /// <summary>
        /// 用户id
        /// </summary>
        [JsonProperty("userId")]
        public int UserId { get; set; }

        /// <summary>
        /// 消息标题
        /// </summary>
        [JsonProperty("msgTitle")]
        public string MsgTitle { get; set; }

        /// <summary>
        /// 消息内容
        /// </summary>
        [JsonProperty("msgBody")]
        public string MsgBody { get; set; }
    }
}
