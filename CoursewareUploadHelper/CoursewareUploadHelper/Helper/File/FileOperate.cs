using System;
using System.Globalization;
using System.IO;

namespace CoursewareUploadHelper.Helper.File
{
    /// <summary>
    /// 文件操作
    /// </summary>
    public static class FileOperate
    {
        /// <summary>
        /// 逐行读文本
        /// </summary>
        /// <param name="path">全路径</param>
        /// <param name="strLine">当前行回调</param>
        public static void ReadTextByLine(string path, Action<string> strLine)
        {
            string line;
            var file = new StreamReader(path);
            while ((line = file.ReadLine()) != null)
            {
                strLine(line);
            }
            file.Close();
        }

        /// <summary>
        /// 文件是否存在
        /// </summary>
        /// <param name="filePath">绝对路径</param>
        /// <returns></returns>
        public static bool IsFileExit(string filePath)
        {
            return System.IO.File.Exists(filePath);
        }

        /// <summary>
        /// 新建文件
        /// </summary>
        /// <param name="path">全路径</param>
        /// <param name="ex">异常信息</param>
        public static void CreateFile(string path, Action<Exception> ex = null)
        {
            if (IsFileExit(path)) return;
            try
            {
                System.IO.File.Create(path);
            }
            catch (Exception e)
            {
                ex?.Invoke(e);
            }
        }

        /// <summary>
        /// 读文本
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <param name="ex"></param>
        public static string ReadText(string path, Action<Exception> ex = null)
        {
            try
            {
                using (var str = new StreamReader(path))
                {
                    return str.ReadToEnd();
                }
            }
            catch (Exception e)
            {
                ex?.Invoke(e);
                return null;
            }
        }

        /// <summary>
        /// 读文本流
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <param name="ex"></param>
        /// <returns></returns>
        public static byte[] ReadStream(string path, Action<Exception> ex = null)
        {
            try
            {
                return System.IO.File.ReadAllBytes(path);
            }
            catch (Exception e)
            {
                ex?.Invoke(e);
                return null;
            }
        }

        /// <summary>
        /// 写文件
        /// 文件存在则覆盖
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <param name="text">要写的内容</param>
        /// <param name="ex">异常信息</param>
        public static void WriteText(string path, string text, Action<Exception> ex = null)
        {
            try
            {
                System.IO.File.WriteAllText(path, text);
            }
            catch (Exception e)
            {
                ex?.Invoke(e);
            }
        }

        /// <summary>
        /// 追加文本
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <param name="text">追加文本</param>
        /// <param name="ex">异常信息</param>
        public static void AppendText(string path, string text, Action<Exception> ex = null)
        {
            try
            {
                System.IO.File.AppendAllText(path, text + Environment.NewLine);
            }
            catch (Exception e)
            {
                ex?.Invoke(e);
            }
        }

        /// <summary>
        /// 清空文本
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <param name="ex">异常信息</param>
        public static void ClearFileText(string path, Action<Exception> ex = null)
        {
            try
            {
                WriteText(path, string.Empty);
            }
            catch (Exception e)
            {
                ex?.Invoke(e);
            }
        }

        /// <summary>
        /// TODO:待测试
        /// 删除行
        /// </summary>
        /// <param name="path"></param>
        /// <param name="textOfLine"></param>
        /// <param name="ex">异常信息</param>
        public static void RemoveLineText(string path, string textOfLine, Action<Exception> ex = null)
        {
            string result = null;
            try
            {
                ReadTextByLine(path, str =>
                {
                    if (!str.Contains(textOfLine))
                        result += str + Environment.NewLine;
                });
                WriteText(path, result);
            }
            catch (Exception e)
            {
                ex?.Invoke(e);
            }
        }

        /// <summary>
        /// 拷贝文件
        /// </summary>
        /// <param name="orignFile">原始文件</param>
        /// <param name="newFile">新文件路径</param>
        /// <param name="ex">异常信息</param>
        public static void FileCoppy(string orignFile, string newFile, Action<Exception> ex = null)
        {
            try
            {
                System.IO.File.Copy(orignFile, newFile, true);
            }
            catch (Exception e)
            {
                ex?.Invoke(e);
            }
        }

        /// <summary>
        /// 移动文件
        /// </summary>
        /// <param name="orignFile">原始路径</param>
        /// <param name="newFile">新路径</param>
        /// <param name="ex">异常信息</param>
        public static void FileMove(string orignFile, string newFile, Action<Exception> ex = null)
        {
            try
            {
                System.IO.File.Move(orignFile, newFile);
            }
            catch (Exception e)
            {
                ex?.Invoke(e);
            }
        }

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="ex">异常信息</param>
        public static void FileDel(string path, Action<Exception> ex = null)
        {
            try
            {
                System.IO.File.Delete(path);
            }
            catch (Exception e)
            {
                ex?.Invoke(e);
            }
        }

        /// <summary>
        /// 取后缀名
        /// </summary>
        /// <param name="filename">文件名</param>
        /// <param name="ex">异常信息</param>
        /// <returns>.gif|.html格式</returns>
        public static string GetPostfixStr(string filename, Action<Exception> ex = null)
        {
            if (string.IsNullOrEmpty(filename))
            {
                ex?.Invoke(new ArgumentNullException(nameof(filename)));
                return null;
            }
            var strArray = filename.Split('.');
            return strArray[strArray.Length - 1];
        }

        /// <summary>
        /// 获取指定文件详细属性
        /// </summary>
        /// <param name="filePath">文件详细路径</param>
        /// <param name="ex">异常信息</param>
        /// <returns></returns>
        public static string GetFileAttibe(string filePath, Action<Exception> ex = null)
        {
            string str = string.Empty;
            if (string.IsNullOrEmpty(filePath))
            {
                ex?.Invoke(new ArgumentNullException(nameof(filePath)));
                return str;
            }

            FileInfo objFi = new FileInfo(filePath);
            str += "详细路径:" + objFi.FullName + "<br>文件名称:" + objFi.Name + "<br>文件长度:" + objFi.Length.ToString() +
                   "字节<br>创建时间" + objFi.CreationTime.ToString(CultureInfo.InvariantCulture) + "<br>最后访问时间:" +
                   objFi.LastAccessTime.ToString(CultureInfo.InvariantCulture) + "<br>修改时间:" +
                   objFi.LastWriteTime.ToString(CultureInfo.InvariantCulture) + "<br>所在目录:" + objFi.DirectoryName +
                   "<br>扩展名:" + objFi.Extension;
            return str;
        }
    }
}
