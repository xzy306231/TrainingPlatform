using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CoursewareUploadHelper.Helper.File
{
    /// <summary>
    /// 文件目录操作类
    /// </summary>
    public static class DirectoryHelper
    {
        /// <summary>
        /// 在当前目录下创建目录
        /// </summary>
        /// <param name="orignFolder">当前目录</param>
        /// <param name="newFloder">新目录</param>
        /// <param name="ex">异常信息</param>
        public static void CreateFolder(string orignFolder, string newFloder, Action<Exception> ex = null)
        {
            try
            {
                Directory.SetCurrentDirectory(orignFolder);
                Directory.CreateDirectory(newFloder);
            }
            catch (Exception e)
            {
                ex?.Invoke(e);
            }
        }

        /// <summary>
        /// 创建文件夹
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="ex">异常信息</param>
        public static void CreateFolder(string path, Action<Exception> ex = null)
        {
            // 判断目标目录是否存在如果不存在则新建之
            if (Directory.Exists(path)) return;
            try
            {
                Directory.CreateDirectory(path);
            }
            catch (Exception e)
            {
                ex?.Invoke(e);
            }
        }

        /// <summary>
        /// 递归删除文件夹目录及文件
        /// </summary>
        /// <param name="dir">目录路径</param>
        /// <param name="ex">异常信息</param>
        /// <returns></returns>
        public static void DeleteFolder(string dir, Action<Exception> ex = null)
        {
            if (!Directory.Exists(dir)) return;
            try
            {
                Directory.Delete(dir, true);
            }
            catch (Exception e)
            {
                ex?.Invoke(e);
            }
        }

        /// <summary>
        /// 指定文件夹下面的所有内容copy到目标文件夹下面
        /// </summary>
        /// <param name="srcPath">原始路径</param>
        /// <param name="aimPath">目标文件夹</param>
        /// <param name="ex">异常信息</param>
        public static void CopyDir(string srcPath, string aimPath, Action<Exception> ex = null)
        {
            try
            {
                // 检查目标目录是否以目录分割字符结束如果不是则添加之
                if (aimPath[aimPath.Length - 1] != Path.DirectorySeparatorChar)
                    aimPath += Path.DirectorySeparatorChar;
                // 判断目标目录是否存在如果不存在则新建之
                if (!Directory.Exists(aimPath))
                    Directory.CreateDirectory(aimPath);
                // 得到源目录的文件列表，该里面是包含文件以及目录路径的一个数组
                //如果你指向copy目标文件下面的文件而不包含目录请使用下面的方法
                //string[] fileList = Directory.GetFiles(srcPath);
                string[] fileList = Directory.GetFileSystemEntries(srcPath);
                //遍历所有的文件和目录
                foreach (string file in fileList)
                {
                    //先当作目录处理如果存在这个目录就递归Copy该目录下面的文件

                    if (Directory.Exists(file))
                        CopyDir(file, aimPath + Path.GetFileName(file), ex);
                    //否则直接Copy文件
                    else
                        System.IO.File.Copy(file, aimPath + Path.GetFileName(file), true);
                }
            }
            catch (Exception e)
            {
                ex?.Invoke(e);
            }
        }

        /// <summary>
        /// 获取文件夹大小
        /// </summary>
        /// <param name="dirPath">文件夹路径</param>
        /// <param name="ex"></param>
        /// <returns></returns>
        public static long GetDirectoryLength(string dirPath, Action<Exception> ex = null)
        {
            if (!Directory.Exists(dirPath))
            {
                ex?.Invoke(new ArgumentException(nameof(dirPath)));
                return 0;
            }
            var di = new DirectoryInfo(dirPath);
            long len = di.GetFiles().Sum(fi => fi.Length);
            var dis = di.GetDirectories();
            if (dis.Length <= 0) return len;
            len += dis.Sum(t => GetDirectoryLength(t.FullName, ex));
            return len;
        }

        /// <summary>
        /// 获取目录中所有文件的名称
        /// </summary>
        /// <param name="path">路径</param>
        /// <returns></returns>
        public static IEnumerable<string> GetListFile(string path)
        {
            var dirInfo = new DirectoryInfo(path);
            var info = dirInfo.GetFiles("*.*");
            return info.Select(file => file.Name);
        }

        #region 获取指定文件夹下所有子目录及文件(树形)

        /// <summary>
        /// 获取指定文件夹下所有子目录及文件
        /// </summary>
        /// <param name="path">详细路径</param>
        /// <param name="ex">异常信息</param>
        public static string GetFoldAll(string path, Action<Exception> ex = null)
        {
            string str = "";
            DirectoryInfo thisOne = new DirectoryInfo(path);
            str = ListTreeShow(thisOne, 0, str, ex);
            return str;
        }

        /// <summary>
        /// 获取指定文件夹下所有子目录及文件函数
        /// </summary>
        /// <param name="theDir">指定目录</param>
        /// <param name="nLevel">默认起始值,调用时,一般为0</param>
        /// <param name="rn">用于迭加的传入值,一般为空</param>
        /// <param name="ex">异常信息</param>
        /// <returns></returns>
        private static string ListTreeShow(DirectoryInfo theDir, int nLevel, string rn, Action<Exception> ex)//递归目录 文件
        {
            try
            {
                var subDirectories = theDir.GetDirectories();//获得目录
                foreach (var dirinfo in subDirectories)
                {
                    if (nLevel == 0)
                    {
                        rn += "├";
                    }
                    else
                    {
                        string s = "";
                        for (int i = 1; i <= nLevel; i++)
                        {
                            s += "│&nbsp;";
                        }
                        rn += s + "├";
                    }
                    rn += "<b>" + dirinfo.Name + "</b><br />";
                    var fileInfo = dirinfo.GetFiles();   //目录下的文件
                    foreach (var fInfo in fileInfo)
                    {
                        if (nLevel == 0)
                        {
                            rn += "│&nbsp;├";
                        }
                        else
                        {
                            string f = "";
                            for (int i = 1; i <= nLevel; i++)
                            {
                                f += "│&nbsp;";
                            }
                            rn += f + "│&nbsp;├";
                        }
                        rn += fInfo.Name + " <br />";
                    }
                    rn = ListTreeShow(dirinfo, nLevel + 1, rn, ex);
                }
            }
            catch (Exception e)
            {
                ex?.Invoke(e);
            }
            return rn;
        }

        ///  <summary>
        ///  获取指定文件夹下所有子目录及文件(下拉框形)
        ///  </summary>
        ///  <param name="path">详细路径</param>
        /// <param name="dropName">下拉列表名称</param>
        /// <param name="tplPath">默认选择模板名称</param>
        /// <param name="ex">异常信息</param>
        public static string GetFoldAll(string path, string dropName, string tplPath, Action<Exception> ex = null)
        {
            string strDrop = "<select name=\"" + dropName + "\" id=\"" + dropName + "\"><option value=\"\">--请选择详细模板--</option>";
            string str = "";
            DirectoryInfo thisOne = new DirectoryInfo(path);
            str = ListTreeShow(thisOne, 0, str, tplPath, ex);
            return strDrop + str + "</select>";

        }

        /// <summary>
        /// 获取指定文件夹下所有子目录及文件函数
        /// </summary>
        /// <param name="theDir">指定目录</param>
        /// <param name="nLevel">默认起始值,调用时,一般为0</param>
        /// <param name="rn">用于迭加的传入值,一般为空</param>
        /// <param name="tplPath">默认选择模板名称</param>
        /// <param name="ex">异常信息</param>
        /// <returns></returns>
        private static string ListTreeShow(DirectoryInfo theDir, int nLevel, string rn, string tplPath, Action<Exception> ex)
        {
            try
            {
                var subDirectories = theDir.GetDirectories();//获得目录

                foreach (var dirinfo in subDirectories)
                {
                    rn += "<option value=\"" + dirinfo.Name + "\"";
                    if (string.Equals(tplPath, dirinfo.Name, StringComparison.CurrentCultureIgnoreCase))
                    {
                        rn += " selected ";
                    }
                    rn += ">";

                    if (nLevel == 0)
                    {
                        rn += "┣";
                    }
                    else
                    {
                        string s = "";
                        for (int i = 1; i <= nLevel; i++)
                        {
                            s += "│&nbsp;";
                        }
                        rn += s + "┣";
                    }
                    rn += "" + dirinfo.Name + "</option>";

                    var fileInfo = dirinfo.GetFiles();   //目录下的文件
                    foreach (var fInfo in fileInfo)
                    {
                        rn += "<option value=\"" + dirinfo.Name + "/" + fInfo.Name + "\"";
                        if (string.Equals(tplPath, fInfo.Name, StringComparison.CurrentCultureIgnoreCase))
                        {
                            rn += " selected ";
                        }
                        rn += ">";

                        if (nLevel == 0)
                        {
                            rn += "│&nbsp;├";
                        }
                        else
                        {
                            string f = "";
                            for (int i = 1; i <= nLevel; i++)
                            {
                                f += "│&nbsp;";
                            }
                            rn += f + "│&nbsp;├";
                        }
                        rn += fInfo.Name + "</option>";
                    }
                    rn = ListTreeShow(dirinfo, nLevel + 1, rn, tplPath, ex);
                }
            }
            catch (Exception e)
            {
                ex?.Invoke(e);
            }
            return rn;
        }
        #endregion
    }
}
