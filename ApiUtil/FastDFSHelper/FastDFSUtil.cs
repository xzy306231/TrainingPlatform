using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using FastDFS.Client;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace ApiUtil.FastDFSHelper
{
    /// <summary>
    /// 
    /// </summary>
    public class FastDFSUtil
    {
        private readonly ILogger<FastDFSUtil> _logger;
        /// <summary>
        /// 
        /// </summary>
        public FastDFSUtil(ILogger<FastDFSUtil> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// 
        /// </summary>
        public FastDFSConfig Config { get; set; }

        /// <summary>
        /// 上传图片到文件服务器
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public async Task<string> ImageFastDfsAsync(IFormFile file)
        {
            if (!_isInitialize) return null;
            var storageNode = await FastDFSClient.GetStorageNodeAsync(_storageGroup);
            if (storageNode == null) return null;
            var fileExt = Path.GetExtension(file.FileName.TrimStart('"').TrimEnd('"').Trim());//扩展名
            var fileStream = file.OpenReadStream();//文件流
            using (var reader = new BinaryReader(fileStream))
            {
                var content = reader.ReadBytes((int)fileStream.Length);
                var fileName = await FastDFSClient.UploadFileAsync(storageNode, content, fileExt);
                return $"/group1/{fileName}";
            }
        }

        /// <summary>
        /// 上传pdf到文件服务器
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public async Task<string> PdfFastDfsAsync(FileInfo file)
        {
            if (!_isInitialize) return null;
            var storageNode = await FastDFSClient.GetStorageNodeAsync(_storageGroup);

            if (storageNode == null) return null;
            var fileExt = file.Extension;//扩展名
            Stream fileStream = file.Open(FileMode.Open);//文件流
            using (var reader = new BinaryReader(fileStream))
            {
                var content = reader.ReadBytes((int)fileStream.Length);
                var fileName = await FastDFSClient.UploadFileAsync(storageNode, content, fileExt);
                return $"/group1/{fileName}";
            }
        }

        /// <summary>
        /// 上传文本到文件服务器
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public async Task<string> TextFastDfsAsync(FileInfo file)
        {
            //连接服务器
            try
            {
                await Task.Delay(1);
                return "";
                //EnsureConnectionInitialize();
                //var storageNode = await FastDFSClient.GetStorageNodeAsync(_storageGroup);
                //if (storageNode != null)
                //{
                //    var fileExt = file.Extension;//扩展名
                //    Stream fileStream = file.Open(FileMode.Open);//文件流
                //    using (BinaryReader reader = new BinaryReader(fileStream))
                //    {
                //        var content = reader.ReadBytes((int)fileStream.Length);
                //        var paths = await FastDFSClient.UploadFileAsync(storageNode, content, fileExt);
                //        return string.Format("/group1/{0}", paths);
                //    }
                //}
            }
            catch (Exception e)
            {
                _logger.LogError("文件服务器炸了：" + e.Message);
            }
            return null;
        }

        /// <summary>
        /// 查看文件服务器文件
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public byte[] GetImageStreamByUrl(string url)
        {
            var ms = new MemoryStream();
            if (string.IsNullOrEmpty(url))
                return ms.ToArray();
            var myRequest = WebRequest.Create(url);
            using (WebResponse myResponse = myRequest.GetResponse())
            {
                using (Stream content = myResponse.GetResponseStream())
                {
                    content?.CopyTo(ms);
                }
            }
            return ms.ToArray();
        }

        /// <summary>
        /// 获取绝对地址
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public string GetImageAbsolutePath(string path)
        {
            if (string.IsNullOrEmpty(path)) return null;
            //else if (path.Contains("upload"))//本服务器下的文件
            //{
            //    var webHelper = EngineContext.Current.Resolve<IWebHelper>();
            //    var address = webHelper.GetUrlReferrer();
            //    if (path.Contains("|"))
            //    {
            //        path = path.Split('|')[0];
            //    }
            //    return address + path;
            //}
            if (path.Contains("http")) return path;//文件服务器保存绝对路径的文件
            var address = Config.FastDFSWebAddress;
            return address + path;
        }

        /// <summary>
        /// 获取图片路径
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public string GetApiImgPath(string path)
        {
            if (string.IsNullOrEmpty(path)) return "";
            //else if (path.Contains("upload") && !path.Contains("http"))//旧文件保存在本地
            //{
            //    string address = EngineContext.Current.Resolve<ConnectionsConfig>().WebUrl;
            //    return address + path;
            //}
            if (path.Contains("upload") || path.Contains("http"))return path;//旧文件保存网络路径
            var address = Config.FastDFSWebAddress;
            return address + path;
        }

        #region 封装方法
        /// <summary>
        /// 是否已经初始化
        /// </summary>
        private static bool _isInitialize;

        private static string _storageGroup = "group1";
        /// <summary>
        /// 静态锁
        /// </summary>
        private static readonly object InitializeLock = new object();

        /// <summary>
        /// 初始Connection
        /// </summary>
        private void EnsureConnectionInitialize()
        {
            if (_isInitialize) return;
            lock (InitializeLock)
            {
                if (_isInitialize) return;
                var trackerIp = Config.FastDFSIPAddress; ;
                var trackerPort = Convert.ToInt32("22122");
                var trackerIPs = new List<IPEndPoint>();
                var endPoint = new IPEndPoint(IPAddress.Parse(trackerIp), trackerPort);
                trackerIPs.Add(endPoint);
                ConnectionManager.Initialize(trackerIPs);
                _isInitialize = true;
            }

        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public byte[] DownFastDFS(string filename)
        {
            var address = Config.FastDFSWebAddress;

            var url = address + filename;
            var request = WebRequest.Create(url);
            request.Method = WebRequestMethods.File.DownloadFile;
            var ms = new MemoryStream();
            try
            {
                using (var response = request.GetResponse())
                {
                    using (var stream = response.GetResponseStream())
                    {
                        stream?.CopyTo(ms);
                    }
                }
                return ms.ToArray();
            }
            catch
            {
                //如果文件服务器还不存在改文件返回空结果
                return new byte[0];
            }
        }
    }
}
