using System;
using ApiUtil.HttpApi;
using Newtonsoft.Json;

namespace ApiUtil.Entities
{
    /// <summary>
    /// 操作类型
    /// </summary>
    public enum OptionType
    {
        /// <summary>
        /// 登陆
        /// </summary>
        Login = 1,

        /// <summary>
        /// 新增
        /// </summary>
        Create,

        /// <summary>
        /// 修改
        /// </summary>
        Update,

        /// <summary>
        /// 删除
        /// </summary>
        Delete,
    }

    /// <summary>
    /// 日志
    /// </summary>
    public class SystemLogEntity
    {
        /// <summary>
        /// 
        /// </summary>
        public SystemLogEntity()
        {
            LogIp = ConfigUtil.GetUserIp;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tokenInfo"></param>
        /// <param name="result"></param>
        /// <param name="option"></param>
        public SystemLogEntity(TokenInfo tokenInfo, bool result = true
            , OptionType option = Entities.OptionType.Update) : this()
        {
            UserName = tokenInfo.UserName;
            UserCard = tokenInfo.UserNumber;
            OptionType = (int) option;
            IsSuccessd = result ? 1 : 2;
        }

        /// <summary>
        /// 日志产生模块
        /// </summary>
        [JsonProperty("moduleName")]
        public static string ModuleName { get; set; }

        /// <summary>
        /// 操作者工号
        /// </summary>
        [JsonProperty("opNo")]
        public string UserCard { get; set; }

        /// <summary>
        /// 操作者名字
        /// </summary>
        [JsonProperty("opName")]
        public string UserName { get; set; }

        /// <summary>
        /// 操作类型（默认为3）
        /// 1：登录 2：新增 3：修改 4：删除
        /// </summary>
        [JsonProperty("opType")]
        public int OptionType { get; set; } = 3;

        /// <summary>
        /// 日志描述
        /// </summary>
        [JsonProperty("logDesc")]
        public string LogDesc { get; set; }

        /// <summary>
        /// 方法执行状态（默认为1）
        /// 1.成功 2.失败
        /// </summary>
        [JsonProperty("logSuccessd")]
        public int IsSuccessd { get; set; } = 1;

        /// <summary>
        /// 异常堆栈信息
        /// </summary>
        [JsonProperty("logMessage")]
        public string ErrorMsg { get; set; }

        /// <summary>
        /// 操作者ip
        /// </summary>
        [JsonProperty("logIp")]
        public string LogIp { get; set; }
    }
}
