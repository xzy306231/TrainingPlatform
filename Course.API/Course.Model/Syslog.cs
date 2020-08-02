using System;
using System.Collections.Generic;
using System.Text;


public class SysLogModel
{
    /// <summary>
    /// 操作者工号
    /// </summary>
    public string opNo { get; set; }
    /// <summary>
    /// 操作者姓名
    /// </summary>
    public string opName { get; set; }
    /// <summary>
    /// 操作类型：1.登录，2：新增，3：修改，4：删除
    /// </summary>
    public int opType { get; set; }
    /// <summary>
    /// 日志描述
    /// </summary>
    public string logDesc { get; set; }
    /// <summary>
    /// 结果：1.成功，2.失败
    /// </summary>
    public sbyte logSuccessd { get; set; }
    /// <summary>
    /// 模块名称
    /// </summary>
    public string moduleName { get; set; }
}

