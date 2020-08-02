using System.Collections.Generic;
using System.IO;
using ApiUtil;

namespace Courseware.Api.Common
{
    public static class FieldCheck
    {
        public static string CheckFieldSortByCreateTimeSize(string str)
        {
            switch (str)
            {
                case "createtime":
                    return "CreateTime";
                case "filesize":
                    return "FileSize";
                default:
                    return null;
            }
        }

        public static string CheckFieldSortByCheckTimeSize(string str)
        {
            switch (str)
            {
                case "checktime":
                    return "CheckDate";
                case "filesize":
                    return "FileSize";
                default:
                    return null;
            }
        }

        public static string CheckFieldOrder(string str)
        {
            switch (str)
            {
                case "asc":
                    return string.Empty;
                case "desc":
                    return "-";
                default:
                    return null;
            }
        }

        public static string CheckSuffixStr(string fileSuffix)
        {
            return CheckSuffix(fileSuffix) ? "0" : "1";
        }

        public static bool CheckSuffix(string fileSuffix)
        {
            fileSuffix = fileSuffix.Trim().ToUpper();
            return ConfigUtil.FileSuffixList.Exists(s => s.Equals(fileSuffix));
        }

        public static string CheckResourceLevel(string level)
        {
            switch (level)
            {
                case "1":
                    return "非密";
                case "2":
                    return "内部";
                default:
                    return "秘密";
            }
        }

        public static string SplitResourceName(string name)
        {
            if (string.IsNullOrEmpty(name)) return name;
            name = Path.GetFileNameWithoutExtension(name);
            //name = name.Substring(0, name.LastIndexOf(".", StringComparison.Ordinal));
            return name; //name.Split('.').Last();
        }

        private static Dictionary<string,string> SourceDict=
            new Dictionary<string, string>
            {
                {"1","video"},//视频
                {"2","flash" },
                {"3","document" },//文档
                {"4","picture" },//图片
                {"5","zip" },//SCORM课件资源包
                {"6","web" }//自定义课件
            };

        public static string GetSourceKey(string srcName)
        {
            foreach (var keyValue in SourceDict)
            {
                if (keyValue.Value.Equals(srcName))
                    return keyValue.Key;
            }

            return "0";
        }

        public static string GetSourceName(string srcKey)
        {
            foreach (var keyValue in SourceDict)
            {
                if (keyValue.Key.Equals(srcKey))
                    return keyValue.Value;
            }

            return "未知类型";
        }

        /// <summary>
        /// 审核中
        /// </summary>
        public static string CheckingStatus = "2";
        /// <summary>
        /// 通过
        /// </summary>
        public static string PassStatus = "3";
        /// <summary>
        /// 拒绝
        /// </summary>
        public static string FailStatus = "4";
        /// <summary>
        /// 非密
        /// </summary>
        public static string ResourceLevel1 = "1";
        /// <summary>
        /// 内部
        /// </summary>
        public static string ResourceLevel2 = "2";
        /// <summary>
        /// 秘密
        /// </summary>
        public static string ResourceLevel3 = "3";
    }
}
