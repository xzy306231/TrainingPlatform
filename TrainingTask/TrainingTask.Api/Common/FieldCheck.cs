using System.Collections.Generic;
using ApiUtil.HttpApi;

namespace TrainingTask.Api.Common
{
    public static class FieldCheck
    {
        public static string SortByCreateTime_hour(string str)
        {
            switch (str)
            {
                case "createtime":
                    return "CreateTime";
                case "classhour":
                    return "ClassHour";
                default:
                    return null;
            }
        }

        public static string SortByStatus_Result(string str)
        {
            switch (str)
            {
                case "status":
                    return "FinishPercent";
                case "result":
                    return "PassPercent";
                default:
                    return null;
            }
        }

        public static string Order(string str)
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

        /// <summary>
        /// 所属部门
        /// </summary>
        public static Dictionary<string, string> Departments { get; set; } = new Dictionary<string, string>();

        public static void SetDepartment(DictObject departments)
        {
            if(departments == null) return;
            foreach (var dictValue in departments.Result)
            {
                if (Departments.ContainsKey(dictValue.DicCode)) Departments[dictValue.DicCode] = dictValue.CodeDsc;
                else Departments.Add(dictValue.DicCode, dictValue.CodeDsc);
            }
        }

        public static string GetDepartment(string key)
        {
            return Departments.ContainsKey(key) ? Departments[key] : "not found key";
        }
    }
}
