using System;
using Newtonsoft.Json;

namespace ApiUtil.Entities
{
    /// <summary>
    /// 待办
    /// </summary>
    public class TodoEntity
    {
        //public TodoEntity()
        //{
            
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="finish">是否处理</param>
        /// <param name="id">资源id</param>
        public TodoEntity(bool finish, long id)
        {
            FinishFlag = finish ? 2 : 1;
            CommonId = (int) id;
        }

        /// <summary>
        /// 消息通知类型
        /// 1.课程审核
        /// 2.课件审核
        /// </summary>
        [JsonProperty("todoType")]
        public static int TodoType { get; set; }

        /// <summary>
        /// 任务状态
        /// 1.未处理
        /// 2.已处理
        /// </summary>
        [JsonProperty("finishFlag")]
        public int FinishFlag { get; set; }

        /// <summary>
        /// 课件id
        /// </summary>
        [JsonProperty("commonId")]
        public int CommonId { get; set; }

        /// <summary>
        /// 待办消息名
        /// </summary>
        [JsonProperty("msgName")]
        public string Name { get; set; }

        /// <summary>
        /// 通知详细内容
        /// 处理结果
        /// </summary>
        [JsonProperty("msgBody")]
        public string Body { get; set; }

        /// <summary>
        /// 通知发送时间
        /// </summary>
        [JsonProperty("pubTime")]
        public string HandleTime { get; set; } = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }
}
