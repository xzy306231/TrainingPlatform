using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace ApiUtil
{
    /// <summary>
    /// 配置文件工具
    /// </summary>
    public class ConfigUtil
    {
        /// <summary>
        /// 
        /// </summary>
        public static IServiceProvider ServiceProvider = null;
        private static IConfiguration _configuration;

        /// <summary>
        /// 初始化配置
        /// </summary>
        /// <param name="configuration"></param>
        public static void InitConfig(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private static string _localIp = string.Empty;
        /// <summary>
        /// 服务ip
        /// </summary>
        public static string LocalIp
        {
            get
            {
                if (string.IsNullOrEmpty(_localIp)) _localIp = _configuration["eureka:instance:hostName"];
                return _localIp;
            }
        }

        private static string _fastWebAddress;
        /// <summary>
        /// FastDFS Web地址
        /// </summary>
        public static string FastWebAddress
        {
            get
            {
                if (string.IsNullOrEmpty(_fastWebAddress)) _fastWebAddress = _configuration["FastDFS:FastDFSWebAddress"];
                return _fastWebAddress;
            }
        }

        private static string _fastIpAddress;
        /// <summary>
        /// FastDFS ip地址
        /// </summary>
        public static string FastIpAddress
        {
            get
            {
                if (string.IsNullOrEmpty(_fastIpAddress)) _fastIpAddress = _configuration["FastDFS:FastDFSIPAddress"];
                return _fastIpAddress;
            }
        }


        private static List<string> _fileSuffixList;
        /// <summary>
        /// 文件后缀忽略列表
        /// </summary>
        public static List<string> FileSuffixList
        {
            get
            {
                if (_fileSuffixList != null) return _fileSuffixList;
                var config = _configuration.GetSection("fileSuffix").GetChildren().ToList().Select(c => c.Value).ToList();
                _fileSuffixList = config;
                return _fileSuffixList;
            }
        }

        private static string GetEncryptString(string conn, string useId = "useid", string pwd = "pwd")
        {
            var connString = string.Empty;
            var sqlConn = new Dictionary<string, string>();
            var strList = conn.Split(new[] { ';' });
            foreach (var str in strList)
            {
                var encryptList = str.Split("= ");
                switch (encryptList.Length)
                {
                    case 2 when !sqlConn.ContainsKey(encryptList[0]):
                        sqlConn.Add(encryptList[0].Trim(), encryptList[1].Trim());
                        break;
                    case 1:
                        var normalList = str.Split(new[] { '=' });
                        if (normalList.Length == 2 && !sqlConn.ContainsKey(normalList[0]))
                            sqlConn.Add(normalList[0].Trim(), normalList[1].Trim());
                        break;
                }
            }

            if (sqlConn.ContainsKey(useId))
                sqlConn[useId] = AesHelper.Decrypt(sqlConn[useId]);

            if (sqlConn.ContainsKey(pwd))
                sqlConn[pwd] = AesHelper.Decrypt(sqlConn[pwd]);

            foreach (var pair in sqlConn)
            {
                connString += $"{pair.Key}={pair.Value};";
            }

            return connString;
        }

        private static string _mySqlConnectionString = string.Empty;
        /// <summary>
        /// MySql默认连接串
        /// </summary>
        public static string MySqlConnectionString
        {
            get
            {
                if (!string.IsNullOrEmpty(_mySqlConnectionString)) return _mySqlConnectionString;

                //连接字符串没加密，直接读取返回
                _mySqlConnectionString = _configuration["DefaultSqlConnectionString:IsEncrypted"] == "false"
                    ? _configuration["DefaultSqlConnectionString:MySql"]
                    : GetEncryptString(_configuration["DefaultSqlConnectionString:MySql"]);

                return _mySqlConnectionString;
            }
        }

        private static string _sqlServerConnectionString = string.Empty;
        /// <summary>
        /// SqlServer默认连接串
        /// </summary>
        public static string SqlServerConnectionString
        {
            get
            {
                if (!string.IsNullOrEmpty(_sqlServerConnectionString)) return _sqlServerConnectionString;

                _sqlServerConnectionString = _configuration["DefaultSqlConnectionString:IsEncrypted"] == "false"
                    ? _configuration["DefaultSqlConnectionString:SqlServer"]
                    : GetEncryptString(_configuration["DefaultSqlConnectionString:SqlServer"], "User ID", "Password");

                return _sqlServerConnectionString;
            }
        }

        private static string _allowUrl = string.Empty;
        /// <summary>
        /// 链接白名单(可不做身份验证)
        /// </summary>
        public static List<string> AllowUrl
        {
            get
            {
                if (string.IsNullOrEmpty(_allowUrl))
                {
                    _allowUrl = _configuration["AllowUrl"];
                }
                List<string> listUrls = new List<string>();
                if (!string.IsNullOrEmpty(_allowUrl))
                {
                    string[] urls = System.Text.RegularExpressions.Regex.Split(_allowUrl, ",");
                    if (urls.Length > 0)
                    {
                        foreach (var url in urls)
                        {
                            if (!listUrls.Contains(url))
                            {
                                listUrls.Add(url);
                            }
                        }
                    }
                }

                return listUrls;
            }
        }

        private static string _filePath = string.Empty;
        /// <summary>
        /// 文件路径
        /// </summary>
        public static string FilePath
        {
            get
            {
                if (string.IsNullOrEmpty(_filePath)) _filePath = _configuration["CommonSettings:FilePath"];
                return _filePath;
            }
        }

        private static string _zipFolder;
        /// <summary>
        /// SCORM压缩包下载路径
        /// </summary>
        public static string ZipFolder
        {
            get
            {
                if (string.IsNullOrEmpty(_zipFolder)) _zipFolder = _configuration["CommonSettings:ZipFolder"];
                return _zipFolder;
            }
        }

        private static string _courseFolder;
        /// <summary>
        /// SCORM解压的课程包路径
        /// </summary>
        public static string CourseFolder
        {
            get
            {
                if (string.IsNullOrEmpty(_courseFolder)) _courseFolder = _configuration["CommonSettings:CourseFolder"];
                return _courseFolder;
            }
        }

        private static string _scormSiteUrl;
        /// <summary>
        /// SCORM网络路径
        /// </summary>
        public static string ScormSiteUrl
        {
            get
            {
                if (string.IsNullOrEmpty(_scormSiteUrl)) _scormSiteUrl = _configuration["CommonSettings:SiteUrl"];
                return _scormSiteUrl;
            }
        }


        private static string _isOpenCache = string.Empty;
        /// <summary>
        /// 是否使用Redis
        /// </summary>
        public static bool IsOpenCache
        {
            get
            {
                if (string.IsNullOrEmpty(_isOpenCache))
                {
                    _isOpenCache = _configuration["Redis:IsOpenRedis"];
                }

                return _isOpenCache.ToLower() == "true";
            }
        }

        private static string _redisConnectionString = string.Empty;
        /// <summary>
        /// Redis默认连接串
        /// </summary>
        public static string RedisConnectionString
        {
            get
            {
                if (string.IsNullOrEmpty(_redisConnectionString))
                {
                    _redisConnectionString = _configuration["Redis:ConnectionString"];
                }
                return _redisConnectionString;
            }
        }

        private static string _redisInstanceName;
        /// <summary>
        /// 
        /// </summary>
        public static string RedisInstanceName
        {
            get
            {
                if (string.IsNullOrEmpty(_redisInstanceName))
                {
                    _redisInstanceName = _configuration["Redis:InstanceName"];
                }
                return _redisInstanceName;
            }
        }


        /// <summary>
        /// 统一请求页面实体
        /// </summary>
        public static HttpContext HttpCurrent
        {
            get
            {
                object factory = ServiceProvider.GetService(typeof(IHttpContextAccessor));
                HttpContext context = ((IHttpContextAccessor) factory).HttpContext;
                return context;
            }
        }

        private static string _getUserIp;
        /// <summary>
        /// 获取请求的真实ip
        /// </summary>
        public static string GetUserIp
        {
            get
            {
                var context = HttpCurrent;
                _getUserIp = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
                if (string.IsNullOrEmpty(_getUserIp)) _getUserIp = context.Connection.RemoteIpAddress.ToString();
                return _getUserIp;
            }
        }


    }
}
