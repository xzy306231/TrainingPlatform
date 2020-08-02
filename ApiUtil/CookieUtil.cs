using System;
using Microsoft.AspNetCore.Http;

namespace ApiUtil
{
    /// <summary>
    /// 
    /// </summary>
    public class CookieUtil
    {
        /// <summary>
        /// 设置本地cookie
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>  
        /// <param name="minutes">过期时长，单位：分钟</param>      
        public static void SetCookies(string key, string value, int minutes = 10)
        {
            ConfigUtil.HttpCurrent.Response.Cookies.Append(key, value, new CookieOptions
            {
                Expires = DateTime.Now.AddMinutes(minutes)
            });
        }

        /// <summary>
        /// 删除指定的cookie
        /// </summary>
        /// <param name="key">键</param>
        public static void DeleteCookies(string key)
        {
            //这个地方想判断就判断下，不过内部封装的方法应该是已经做过处理
            ConfigUtil.HttpCurrent.Response.Cookies.Delete(key);
        }

        /// <summary>
        /// 获取cookies
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="defaultValue"></param>
        /// <returns>返回对应的值</returns>
        public static string GetCookies(string key, string defaultValue = "")
        {
            string value = string.Empty;
            ConfigUtil.HttpCurrent.Request.Cookies.TryGetValue(key, out value);
            if (string.IsNullOrEmpty(value))
            {
                value = defaultValue;
            }
            return value;
        }
    }
}
