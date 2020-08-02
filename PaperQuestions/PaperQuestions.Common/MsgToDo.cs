using System;
using System.Collections.Generic;
using System.Text;


    public class MsgToDo
    {
        /// <summary>
        /// 1：课程审核，2：课件审核
        /// </summary>
        public int todoType { get; set; }

        /// <summary>
        /// 课程或课件ID
        /// </summary>
        public long commonId { get; set; }

        /// <summary>
        /// 消息名称
        /// </summary>
        public string msgName { get; set; }

        /// <summary>
        /// 消息详情
        /// </summary>
        public string msgBody { get; set; }

        /// <summary>
        /// 1：未处理，2：已处理
        /// </summary>
        public sbyte finishFlag { get; set; }

        public string pubTime { get; set; }
    }

