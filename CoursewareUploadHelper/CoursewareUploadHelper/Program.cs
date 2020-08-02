using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using CoursewareUploadHelper.Entity;
using CoursewareUploadHelper.Entity.Course;
using CoursewareUploadHelper.Helper.File;
using FreeSql;
using Microsoft.Extensions.Configuration;

namespace CoursewareUploadHelper
{
    class Program
    {
        // ReSharper disable once IdentifierTypo

        #region ::::: ORM对象 :::::

        public static IFreeSql FSqlCourseware { get; set; }
        public static IFreeSql FSqlCourse { get; set; }
        public static IFreeSql FSqlPeopleManage { get; set; }
        public static IFreeSql FSqlTag { get; set; }

        #endregion

        /// <summary>
        /// 配置信息
        /// </summary>
        public static IConfigurationRoot Config;

        /// <summary>
        /// 导入账号id
        /// </summary>
        public static TPersonInfo Teacher;

        static void Main()
        {
            WriteToConsole("课件导入工具启动...初始化中...");

            //配置信息
            Config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true).Build();
            if (Config == null)
            {
                WriteRedMsg("配置信息出错，请检查配置文件完整性！程序退出！");
                return;
            }

            //初始化orm
            if (!InitOrm())
            {
                WriteRedMsg("数据库连接失败，请检查数据库连接！程序退出！");
                return;
            }
            WriteGreenMsg("数据库连接成功...");

            #region ::::: 导入课件程序 :::::

            var importProcessor = ImportProcessor.ImportProcessor.GetInstance();
            importProcessor.LogDebug += logInfo => { WriteToConsole($"{logInfo}"); };
            importProcessor.LogError += logInfo => { WriteRedMsg($"{logInfo}"); };

            #endregion

            #region ::::: 替换课件程序 :::::

            var replaceProcessor = ReplaceProcessor.GetInstance();
            replaceProcessor.LogDebug += logInfo => { WriteToConsole($"{logInfo}"); };
            replaceProcessor.LogError += logInfo => { WriteRedMsg($"{logInfo}"); };

            #endregion

            #region ::::: 新导入课件程序 :::::

            var newImportProcessor = NewImportProcessor.GetInstance();
            newImportProcessor.LogDebug += logInfo => { WriteToConsole($"{logInfo}"); };
            newImportProcessor.LogError += logInfo => { WriteRedMsg($"{logInfo}"); };

            #endregion

            #region ::::: 执行对应操作 :::::

            var importOld = Config["ProcessState:import_old"];//导入课件课程，派思
            var replaceOld = Config["ProcessState:replace_old"];//课件替换，新课件可拖放进度
            var modifySubTitle = Config["ProcessState:modifySubTitle_old"];//修改字幕，课程中的字幕
            var updateTags = Config["ProcessState:updateTags"];//更新知识点
            var outputSubTitle = Config["ProcessState:outputSubTitle"];//输出字幕文本文件
            var importNew = Config["ProcessState:import_new"];//新课件课程导入工具


            while (true)
            {
                WriteYellowMsg("请选择要进行的操作类型，输入数字后回车：", false);

                WriteMsgBase(
                    $"{(importOld.Equals("0") ? "[1]导入课件 " : "")}" +
                    $" {(replaceOld.Equals("0") ? "[2]替换课件 " : "")}" +
                    $" {(modifySubTitle.Equals("0") ? "[3]修改字幕 " : "")}"+
                    $" {(updateTags.Equals("0") ? "[4]更新知识点 " : "")}" +
                    $" {(outputSubTitle.Equals("0") ? "[5]生成字幕文件 " : "")}" +
                    $" {(importNew.Equals("0") ? "[6]导入新课件课程" : "")}" +
                    $"  [0]退出",
                    ConsoleColor.Cyan,
                    false);

                var processIndex = Console.ReadLine();

                switch (processIndex)
                {
                    case "1"://导入课件
                        if (!importOld.Equals("0"))
                        {
                            WriteRedMsg("导入课件功能已暂停，联系开发人员放开限制");
                            break;
                        }
                        WriteYellowMsg($"将要进行的操作:导入课件", false);

                        WriteGreenMsg(@"程序初始化完成，运行中...", false);

                        importProcessor.Run();//导入派思结构课程

                        AddOrUpdateAppSetting("ProcessState:import", "1");//设置不显示
                        WriteYellowMsg("程序运行结束，按任意键结束", false);
                        Console.ReadKey();
                        return;
                    case "2"://替换课件
                        if (!replaceOld.Equals("0"))
                        {
                            WriteRedMsg("替换课件功能已暂停，联系开发人员放开限制");
                            break;
                        }
                        WriteYellowMsg($"将要进行的操作:替换课件", false);
                        long id = 0;
                        while (true)
                        {
                            var coursePaths = new List<string>();
                            var filePath = Directory.GetCurrentDirectory() + @"\Course.txt";
                            if (File.Exists(filePath))
                            {
                                FileOperate.ReadTextByLine(filePath, s => coursePaths.Add(s));
                            }
                            id++;
                            Console.WriteLine($"需要替换的课件id为{id}? y or n");
                            var readResult = Console.ReadLine() ?? "";
                            if (!string.IsNullOrEmpty(readResult) &&
                                readResult.Trim().ToLower() != "y")
                            {
                                if (!MatchFirstParam(ref id)) break;
                            }
                            var path = string.Empty;
                            Console.WriteLine($"课程目录为【{coursePaths[(int)id - 1]}】? y or n");
                            var readPathResult = Console.ReadLine() ?? "";
                            if (!string.IsNullOrEmpty(readPathResult) && readPathResult.Trim().ToLower() != "y")
                            {
                                if (!MatchSecondParam(ref path)) break;
                            }
                            else
                            {
                                path = coursePaths[(int)id - 1];
                            }
                            WriteGreenMsg($"将要替换id为{id}的课程，课程文件目录为{path}", false);
                            replaceProcessor.Run(id, path);
                        }
                        break;
                    case "3"://修改字幕
                        if (!modifySubTitle.Equals("0"))
                        {
                            WriteRedMsg("修改字幕功能已暂停，联系开发人员放开限制");
                            break;
                        }
                        WriteYellowMsg($"将要进行的操作:修改字幕", false);
                        Thread.Sleep(2000);
                        replaceProcessor.ReplaceSubtitle();
                        break;
                    case "4"://更新知识点
                        if (!updateTags.Equals("0"))
                        {
                            WriteRedMsg("更新知识点功能已暂停，联系开发人员放开限制");
                            break;
                        }
                        WriteYellowMsg("将要进行的操作:更新知识点", false);
                        Thread.Sleep(2000);
                        importProcessor.UpdateTag();
                        break;
                    case "5"://生成字幕文件
                        if (!outputSubTitle.Equals("0"))
                        {
                            WriteRedMsg("生成字幕文件功能已暂停，联系开发人员放开限制");
                            break;
                        }
                        WriteYellowMsg("将要进行的操作:从数据库反向导出字幕文件", false);
                        Thread.Sleep(2000);
                        replaceProcessor.CreateSubtitleFile();
                        break;
                    case "6"://导入新课件
                        if (!importNew.Equals("0"))
                        {
                            WriteRedMsg("导入新课件功能已暂停，联系开发人员放开限制");
                            break;
                        }
                        WriteYellowMsg("将要进行的操作:导入新课件课程");
                        newImportProcessor.Run();
                        WriteYellowMsg("程序运行结束，按任意键结束", false);
                        Console.ReadKey();
                        return;
                    case "0":
                        #region ::::: 程序退出 :::::

                        WriteGreenMsg("close...");
                        WriteRedMsg("3..", false);
                        Thread.Sleep(1000);
                        WriteRedMsg("2..", false);
                        Thread.Sleep(1000);
                        WriteRedMsg("1..", false);
                        Thread.Sleep(1000);
                        #endregion

                        return;
                }
            }


            #endregion
        }

        /// <summary>
        /// 获取存储目录地址
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GetUrlWithoutDrive(string path)
        {
            path = path.Replace('\\', '/');
            var temp = path.Split(":/");
            if (temp.Length == 2) path = temp[1].Substring(4);
            return path;
        }

        /// <summary>
        /// 初始化orm
        /// </summary>
        static bool InitOrm()
        {
            var result = true;
            try
            {
                FSqlCourse = new FreeSqlBuilder()
                    .UseConnectionString(DataType.SqlServer, Config["SqlConnection:Course"]).Build();
                FSqlCourseware = new FreeSqlBuilder()
                    .UseConnectionString(DataType.SqlServer, Config["SqlConnection:Courseware"]).Build();
                FSqlPeopleManage = new FreeSqlBuilder()
                    .UseConnectionString(DataType.SqlServer, Config["SqlConnection:PeopleManage"]).Build();
                FSqlTag = new FreeSqlBuilder()
                    .UseConnectionString(DataType.SqlServer, Config["SqlConnection:Knowledge"]).Build();

                #region :::::  :::::

                var userNumber = Config["CreatorInfo:userNumber"];//导入人账号
                Teacher = FSqlPeopleManage.Select<TPersonInfo>().Where(p => p.UserNumber.Equals(userNumber)).ToOne();

                if (Teacher == null)
                {
                    WriteRedMsg($"用户名[{userNumber}]不存在，请先在配置文件中填写上传课件课程的用户名！");
                    return false;
                }

                #endregion
            }
            catch (Exception e)
            {
                WriteRedMsg(e.Message);
                result = false;
            }
            return result;
        }

        /// <summary>
        /// 替换课件匹配第一个参数
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private static bool MatchFirstParam(ref long id)
        {
            WriteYellowMsg("请输入需要替换的课程id，按回车继续.[退出请输入quit并回车]", false);
            var readLine = Console.ReadLine()?.Trim();
            if (readLine != null && readLine.Equals("quit"))
                return false;
            if (long.TryParse(readLine, out var result))
            {
                id = result;
                var course = FSqlCourse.Select<TCourse>(id).ToOne();
                if (course == null)
                {
                   WriteRedMsg($"数据库中不存在id为{id}的课程，请重试！",false);
                }
                else
                {
                    WriteGreenMsg($"找到id为{id}的课程，课程名为【{course.CourseName}】",false);
                    return true;
                }
            }
            else
            {
                WriteRedMsg("请输入整形，非数字相关的请勿输入，请重试！", false);
            }

            return MatchFirstParam(ref id);
        }

        /// <summary>
        /// 替换课件匹配第二个参数
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private static bool MatchSecondParam(ref string path)
        {
            WriteYellowMsg("请将需要替换的课程目录粘贴到此，按回车继续.[退出请输入quit并回车]", false);
            var readLine = Console.ReadLine()?.Trim();
            if (readLine != null && readLine.Equals("quit"))
                return false;
            if (Directory.Exists(readLine))
            {
                path = readLine;
                return true;
            }
            WriteRedMsg("该路径不存在或错误，请重新输入！", false);
            return MatchSecondParam(ref path);
        }

        /// <summary>
        /// 更新配置文件中的字段
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void AddOrUpdateAppSetting<T>(string key, T value)
        {
            try
            {
                var filePath = Path.Combine(AppContext.BaseDirectory, "appSettings.json");
                string json = File.ReadAllText(filePath);
                dynamic jsonObj = Newtonsoft.Json.JsonConvert.DeserializeObject(json);

                var sectionPath = key.Split(":")[0];
                if (!string.IsNullOrEmpty(sectionPath))
                {
                    var keyPath = key.Split(":")[1];
                    jsonObj[sectionPath][keyPath] = value;
                }
                else
                {
                    jsonObj[sectionPath] = value; // if no sectionpath just set the value
                }
                string output = Newtonsoft.Json.JsonConvert.SerializeObject(jsonObj, Newtonsoft.Json.Formatting.Indented);
                File.WriteAllText(filePath, output);

            }
            catch (Exception)
            {
                WriteRedMsg("Error writing app settings");
            }
        }

        #region ::::: Console :::::

        /// <summary>
        /// 输出到命令行窗口
        /// 一般消息
        /// </summary>
        /// <param name="message"></param>
        /// <param name="showTime"></param>
        public static void WriteToConsole(string message, bool showTime = true) =>
            WriteMsgBase(message, ConsoleColor.White, showTime);

        /// <summary>
        /// 输出到命令行窗口
        /// 告警消息
        /// </summary>
        /// <param name="message"></param>
        /// <param name="showTime"></param>
        public static void WriteYellowMsg(string message, bool showTime = true) =>
            WriteMsgBase(message, ConsoleColor.Yellow, showTime);

        /// <summary>
        /// 输出到命令行窗口
        /// 执行正确的消息
        /// </summary>
        /// <param name="message"></param>
        /// <param name="showTime"></param>
        public static void WriteGreenMsg(string message, bool showTime = true) =>
            WriteMsgBase(message, ConsoleColor.Green, showTime);

        /// <summary>
        /// 输出到命令行窗口
        /// 错误消息
        /// </summary>
        /// <param name="message"></param>
        /// <param name="showTime"></param>
        public static void WriteRedMsg(string message, bool showTime = true) =>
            WriteMsgBase(message, ConsoleColor.Red, showTime);

        /// <summary>
        /// 日志输出基础方法
        /// </summary>
        /// <param name="message"></param>
        /// <param name="color"></param>
        /// <param name="showTime"></param>
        public static void WriteMsgBase(string message, ConsoleColor color, bool showTime = true)
        {
            Console.WriteLine();
            Console.ForegroundColor = color;
            Console.WriteLine(!showTime ? $"{message}" : $"[{DateTime.Now:HH:mm:ss}] -- {message}");
            Console.ForegroundColor = ConsoleColor.White;
        }

        #endregion
    }
}
