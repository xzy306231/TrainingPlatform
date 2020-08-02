using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ApiUtil;

namespace Courseware.Api.Common
{
    public class FileSystemHelper
    {
        /// <summary>
        /// 注意sPath是完整的物理路径，如果文件不存在，包括文件名将返回空白
        /// </summary>
        /// <param name="sPath"></param>
        /// <returns></returns>
        public static string GetUniqueFileName(string sPath)
        {
            if (!File.Exists(sPath)) return "";
            var sFileNameWithoutExtension = Path.GetFileNameWithoutExtension(sPath);
            var sPathWithoutFileName = Path.GetDirectoryName(sPath);
            var extension = Path.GetExtension(sPath);
            for (int i = 1; i < 1000; i++)
            {
                var newName = $"{sFileNameWithoutExtension}({i.ToString()}){extension}";
                if (!File.Exists($"{sPathWithoutFileName}/{newName}")) return newName;
            }
            return ""; // shouldn't get here
        }

        public static string FindManifestFile(string sPathToPackageFolder)
        {
            // returns full path to imsmanifest.xml
            var di = new DirectoryInfo(sPathToPackageFolder);
            string fullPath = WalkDirectoryTree(di, "imsmanifest.xml", false);
            return fullPath;
        }

        public static string GetScoUrl(string pathToSco, string hrefSco)
        {
            var url = "";
            var sitUrl = ConfigUtil.ScormSiteUrl;
            var courseFolder = ConfigUtil.CourseFolder;
            pathToSco = pathToSco.Replace(@"\", @"/");
            //把pathToSco的所有东西都移到courseFolder中
            int i = pathToSco.ToLower().IndexOf(courseFolder.ToLower(), StringComparison.Ordinal);
            if (i >= 0)
            {
                pathToSco = pathToSco.Substring(i, pathToSco.Length - i);
            }

            url = $"{sitUrl}/{pathToSco}/{hrefSco}";
            return url;
        }

        static string WalkDirectoryTree(DirectoryInfo root, string sFileNameToSearchFor, bool bReturnRelativePath)
        {
            FileInfo[] files = null;
            DirectoryInfo[] subDirs;

            // First, process all the files directly under this folder
            // 首先，直接处理这个文件夹下的所有文件
            try
            {
                files = root.GetFiles("*.*");
            }
            // This is thrown if even one of the files requires permissions greater than the application provides.
            // 如果其中一个文件需要的权限大于应用程序提供的权限，则会引发此问题。
            catch (UnauthorizedAccessException e)
            {
                Console.WriteLine(e.Message);
            }

            catch (DirectoryNotFoundException e)
            {
                Console.WriteLine(e.Message);
            }

            if (files != null)
            {
                foreach (var fi in files)
                {
                    if (fi.Name == sFileNameToSearchFor)
                    {
                        string fullPath = Path.GetFullPath(fi.FullName);
                        string relativePath = Path.GetRelativePath(ConfigUtil.ZipFolder, fullPath);
                        return bReturnRelativePath ? relativePath : fullPath;
                    }
                }

                // Now find all the subdirectories under this directory.
                // 现在找到这个目录下的所有子目录。
                subDirs = root.GetDirectories();

                foreach (DirectoryInfo dirInfo in subDirs)
                {
                    // Resursive call for each subdirectory.
                    // 调用每个子目录。
                    string sPath = WalkDirectoryTree(dirInfo, sFileNameToSearchFor, bReturnRelativePath);
                    if (!string.IsNullOrWhiteSpace(sPath))
                    {
                        return sPath;
                    }
                }
            }
            return "";
        }
    }
}
