using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using ApiUtil;
using ApiUtil.Log;
using FastDFS.Client;
using Microsoft.Extensions.Logging;

namespace Courseware.Api.Common
{
    public class FastDfsHelper
    {
        private static readonly FastDfsHelper Instance = new FastDfsHelper();

        private FastDfsHelper()
        {
            var pEndPoints = new List<IPEndPoint>{ new IPEndPoint(IPAddress.Parse(ConfigUtil.FastIpAddress),22122) };
            ConnectionManager.Initialize(pEndPoints);
        }

        public static FastDfsHelper GetInstance() => Instance;


        public void SetLogger(ILogger logger)
        {
            if (Instance._logger == null)
            {
                Instance._logger = logger;
            }
        }

        public string StorageGroup { get; set; } = "group1";

        private ILogger _logger;

        /// <summary>
        /// 下载SCORM资源压缩包
        /// </summary>
        /// <param name="resourceName"></param>
        /// <param name="fastDfsPath"></param>
        /// <param name="folder"></param>
        /// <returns></returns>
        public async Task<string> ZipFileDownLoad(string resourceName, string fastDfsPath, string folder)
        {
            var storageNode = await FastDFSClient.GetStorageNodeAsync(StorageGroup);
            var pathToFile = Path.Combine(folder, resourceName);
            var newFileName = FileSystemHelper.GetUniqueFileName(pathToFile);//$"{folder}{Path.DirectorySeparatorChar}{Guid.NewGuid()}.zip";
            if (!string.IsNullOrEmpty(newFileName))
            {
                var fileName = newFileName;
                pathToFile = Path.Combine(folder, fileName);
            }
            try
            {
                using (var fileStream = File.OpenWrite(pathToFile))
                {
                    await FastDFSClient.DownloadFileAsync(storageNode, fastDfsPath,
                        new StreamDownloadCallback(fileStream));
                }
            }
            catch (Exception e)
            {
                _logger?.LogError(LogHelper.OutputClearness($"下载SCORM资源包失败，失败信息：{e.Message}"));
                return null;
            }

            return pathToFile;
        }
    }
}
