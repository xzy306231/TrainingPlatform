using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;


public class PubMethod
{
    public static void Log(SysLogModel syslog)
    {
        syslog.moduleName = "考试管理";
        //RabbitMQClient rabbitMQClient = new RabbitMQClient();
        //rabbitMQClient.PushMessage(syslog, 1);
    }

    public static object GetPropertyValue(object obj, string property)
    {
        PropertyInfo propertyInfo = obj.GetType().GetProperty(property);
        return propertyInfo.GetValue(obj, null);
    }
    public static void ErrorLog(Exception exception, string path)
    {
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
        StreamWriter sw = new StreamWriter(path + DateTime.Now.ToString("yyyyMMdd") + ".Log", true, Encoding.Unicode);
        sw.WriteLine("");
        sw.WriteLine("-----------时间：" + DateTime.Now.ToString("yyyy年MM月dd日 HH时mm分ss秒") + "-----------");
        sw.WriteLine("-----------消息标题-----------");
        sw.WriteLine(exception.Message);
        sw.WriteLine("-----------详细信息-----------");
        sw.WriteLine(exception.ToString());
        sw.Close();
    }
}

