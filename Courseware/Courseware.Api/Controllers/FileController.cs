using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using ApiUtil;
using ApiUtil.Entities;
using ApiUtil.Log;
using ApiUtil.Mq;
using AutoMapper;
using Courseware.Api.Common;
using Courseware.Api.ViewModel.FileUpDown;
using Courseware.Core.Entities;
using Courseware.Infrastructure.Repository;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Courseware.Api.Controllers
{
    [Route("coursesource/v1/file")]
    [EnableCors("any")]
    public class FileController : Controller
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly IHostingEnvironment _env;
        private readonly string _downloadFolder;
        private readonly string _courseFolder;
        private readonly ILogger<FileController> _logger;
        private readonly IMapper _mapper;
        private readonly RabbitMqClient _mqClient;
        private readonly string _localIp;
        private readonly ServiceHelper _service;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="env"></param>
        /// <param name="unitOfWork"></param>
        /// <param name="mapper"></param>
        /// <param name="mqClient"></param>
        /// <param name="logger"></param>
        /// <param name="service"></param>
        public FileController(
            IHostingEnvironment env,
            UnitOfWork unitOfWork,
            IMapper mapper,
            RabbitMqClient mqClient,
            ILogger<FileController> logger,
            ServiceHelper service)
        {
            _env = env;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
            _mqClient = mqClient;
            _localIp = ConfigUtil.LocalIp;
            _service = service;
            if (string.IsNullOrEmpty(_env.WebRootPath))
            {
                _logger.LogInformation(LogHelper.OutputClearness($"_env.WebRootPath为null"));
                _env.WebRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                _logger.LogInformation(LogHelper.OutputClearness($"新建文件夹路径为{_env.WebRootPath}"));
            }
            //scorm zip下载文件夹
            _downloadFolder = Path.Combine(env.WebRootPath, ConfigUtil.ZipFolder);
            if (!Directory.Exists(_downloadFolder))
            {
                _logger.LogInformation(LogHelper.OutputClearness($"scorm下载文件夹路径为{_downloadFolder}"));
                Directory.CreateDirectory(_downloadFolder);
            }
            //scorm 课件包
            _courseFolder = Path.Combine(env.WebRootPath, ConfigUtil.CourseFolder);
            if (!Directory.Exists(_courseFolder)) Directory.CreateDirectory(_courseFolder);
        }

        /// <summary>
        /// 上传课件信息
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        [HttpPost("uploadFileInfo")]
        public async Task<IActionResult> UpdateFileInfo([FromBody]UploadFileInfo info)
        {
            if (!ModelState.IsValid) return Ok(new ResponseError("参数提交不完整，请重新提交"));

            var newResource = _mapper.Map<ResourceEntity>(info);
            var result = await _unitOfWork.ResourceRepository.InsertAsync(newResource);

            if (newResource.TransfType.Equals("1"))
            {
                _mqClient.PushTransformMessage(new FileTransfEntity
                {
                    FileSuffix = newResource.FileSuffix,
                    OriginalUrl = newResource.OriginalUrl,
                    GroupName = newResource.GroupName,
                    ResourceName = newResource.ResourceName,
                    SourceId = newResource.Id
                });
            }
            else
            {
                bool tempResult = true;
                if (newResource.ResourceType.Equals("5"))
                {
                    //TODO:解析
                    tempResult = false;
                    var transfScorm = await ScormResult($"{newResource.ResourceName}.{newResource.FileSuffix}", newResource.OriginalUrl);
                    if (transfScorm != null)
                    {
                        newResource.TitleFromManifest = transfScorm.Title;
                        newResource.PathToIndex = transfScorm.Href;
                        newResource.PathToFolder = transfScorm.PathToPackageFolder;
                        newResource.SCORMVersion = transfScorm.SCORM_Version;
                        newResource.TransfType = "0";
                        tempResult = await _unitOfWork.ResourceRepository.UpdateAsync(newResource);
                    }
                }

                if (tempResult)
                    _mqClient.PushTodoMessage(new TodoEntity(false, newResource.Id) { Name = "您有一个课件待审核" });
            }

            _mqClient.PushLogMessage(new SystemLogEntity(await _service.GetTokenInfo(), result, OptionType.Create)
                {LogDesc = "课件上传成功." +
                           $"名称[{info.ResourceName}]" +
                           $"类型[{FieldCheck.GetSourceName(info.ResourceType)}]" +
                           $"密级[{FieldCheck.CheckResourceLevel(info.ResourceLevel)}]"});
                    //$"上传密级[{FieldCheck.CheckResourceLevel(info.ResourceLevel)}]课件{info.ResourceName}{(result ? "成功" : "失败")}"});

            return Ok(result ? new ResponseInfo() : new ResponseError("资源上传失败！"));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="resourceName"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        private async Task<SCORMUploadHelper> ScormResult(string resourceName, string path)
        {
            var scormZipPath = await FastDfsHelper.GetInstance().ZipFileDownLoad(resourceName, path, _downloadFolder);
            if (scormZipPath == null) return null;

            //确保文件夹名称没有无效字符
            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(scormZipPath);
            fileNameWithoutExtension = Path.GetInvalidFileNameChars().Aggregate(fileNameWithoutExtension, (current, c) => current.Replace(c.ToString(), string.Empty));

            //Unzip the package
            var pathToPackageFolder = Path.Combine(_courseFolder, fileNameWithoutExtension);
            try
            {
                ZipFile.ExtractToDirectory(scormZipPath, pathToPackageFolder, true);
            }
            catch (Exception e)
            {
                _logger.LogError(LogHelper.OutputClearness($"将压缩文件{scormZipPath}解压到文件夹{pathToPackageFolder}失败!失败信息{e.Message}"));
                return null;
            }

            //find the imsmanifest.xml file
            var pathToManifest = FileSystemHelper.FindManifestFile(pathToPackageFolder);
            if (string.IsNullOrEmpty(pathToManifest)) return null;

            //
            var scorm = new SCORMUploadHelper(_logger);
            scorm.ParseManifest(pathToManifest);

            //
            scorm.PathToManifest = pathToManifest;
            scorm.PathToPackageFolder = pathToPackageFolder;
            //databaseHelper.InsertScormCourse(scorm.title, scorm.title, sPathToIndex, sPathToManifest, sPathToPackageFolder, scorm.SCORM_Version, DateTime.Now, UserID);
            return scorm;
        }

        /// <summary>
        /// 文件转换完成通知
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        [HttpPost("convertFile")]
        public async Task<IActionResult> TransformComplete([FromBody] TransformComplete info)
        {
            if (!ModelState.IsValid) return BadRequest();

            _logger.LogDebug(LogHelper.OutputClearness($"收到文件转换消息，Dto内容为{info.ResourceId}|{info.TransFilePath}"));
            var tempResource = await _unitOfWork.ResourceRepository.GetAsync((entity => entity.Id.Equals(info.ResourceId)));
            if (tempResource == null)
            {
                _logger.LogError("文件转换失败，失败原因：没有找到这个课件id");
                return BadRequest();
            }

            tempResource.TransfType = "0";
            tempResource.TransformUrl = info.TransFilePath;
            var result = await _unitOfWork.ResourceRepository.UpdateAsync(tempResource);
            if (!result)
            {
                _logger.LogInformation(LogHelper.OutputClearness($"将转换结果更新到数据库失败,课件id为{info.ResourceId}"));
                return BadRequest();
            }
            _logger.LogDebug(LogHelper.OutputClearness($"转换结果更新到数据库成功"));

            _mqClient.PushTodoMessage(new TodoEntity(false, tempResource.Id) {Name = "您有一个课件待审核"});
            return Ok();
        }

        /*
         * 第一步，握手传递文件大小，多少片，md5值，文件类型
         * 第二步，分片上传文件片段，存储文件
         * 第三步，合并所有文件，做md5校验
         */
        #region ::::: 分片上传 :::::

        /// <summary>
        /// 请求上传文件
        /// </summary>
        /// <param name="requestFile">请求上传参数实体</param>
        /// <returns></returns>
        [HttpPost("requestUpload")]
        public async Task<IActionResult> RequestUploadFile([FromBody]RequestFileUploadEntity requestFile)
        {
            _logger.LogDebug($"RequestUploadFile 接收参数：{JsonConvert.SerializeObject(requestFile)}");
            if (requestFile.Size <= 0 || requestFile.Count <= 0 || string.IsNullOrEmpty(requestFile.FileData))
                return Ok(new ResponseError("参数有误"));

            var temp = await _unitOfWork.ResourceRepository.GetAsync(entity =>
                entity.DeleteFlag == 0 && entity.MD5Code.Equals(requestFile.FileData));
            if (temp != null) return Ok(new ResponseError("该文件已存在！"));

            //这里需要记录文件相关信息，并返回文件guid名，后续请求带上此参数
            var guidName = Guid.NewGuid().ToString("N");

            //前期单台服务器可以记录Cache，多台后需考虑redis或数据库
            CacheUtil.Set(guidName, requestFile, new TimeSpan(0, 10, 0), true);
            return Ok(new ResponseInfo { Result = new { filename = guidName } });
        }

        /// <summary>
        /// 文件上传
        /// </summary>
        /// <returns></returns>
        [HttpPost("upload")]
        public async Task<IActionResult> FileSave()
        {
            var files = Request.Form.Files;
            long size = files.Sum(f => f.Length);
            string fileName = Request.Form["filename"];

            int.TryParse(Request.Form["fileindex"], out var fileIndex);
            _logger.LogDebug($"FileSave开始执行获取数据：{fileIndex}_{size}");

            if (size <= 0 || string.IsNullOrEmpty(fileName)) return Ok(new ResponseError("文件上传失败"));

            if (!CacheUtil.Exists(fileName)) return Ok(new ResponseError("请重新请求上传文件"));

            long fileSize = 0;
            string filePath = $".{ConfigUtil.FilePath}{DateTime.Now:yyyy-MM-dd}/{fileName}";
            string saveFileName = $"{fileName}_{fileIndex}";
            string dirPath = Path.Combine(filePath, saveFileName);

            if (!Directory.Exists(filePath)) Directory.CreateDirectory(filePath);

            foreach (var file in files)
            {
                //如果有文件
                if (file.Length <= 0) continue;
                fileSize = 0;
                fileSize = file.Length;

                using (var stream = new FileStream(dirPath, FileMode.OpenOrCreate))
                {
                    await file.CopyToAsync(stream);
                }
            }
            return Ok(new ResponseInfo());
        }

        /// <summary>
        /// 文件合并
        /// </summary>
        /// <param name="fileInfo">文件参数信息[name]</param>
        /// <returns></returns>
        [HttpPost("merge")]
        public async Task<IActionResult> FileMerge([FromBody]FileMergeDto fileInfo)
        {
            var fileName = fileInfo.name;
            if (string.IsNullOrEmpty(fileName)) return Ok(new ResponseError("文件名不能为空"));

            //最终上传完成后，请求合并返回合并消息
            try
            {
                var requestFile = CacheUtil.Get<RequestFileUploadEntity>(fileName);
                if (requestFile == null) return Ok(new ResponseError("合并失败"));
                var filePath = $".{ConfigUtil.FilePath}{DateTime.Now:yyyy-MM-dd}/{fileName}";
                var fileExt = requestFile.FileExt;
                var fileMd5 = requestFile.FileData;//MD5
                var fileCount = requestFile.Count;
                var fileSize = requestFile.Size;

                _logger.LogDebug($"获取文件路径：{filePath}");
                _logger.LogDebug($"获取文件类型：{fileExt}");

                var savePath = filePath.Replace(fileName, "");
                var saveFileName = $"{fileName}{fileExt}";
                var files = Directory.GetFiles(filePath);
                var fileFinalName = Path.Combine(savePath, saveFileName);
                _logger.LogDebug($"获取文件最终路径：{fileFinalName}");
                var fs = new FileStream(fileFinalName, FileMode.Create);
                _logger.LogDebug($"目录文件下文件总数：{files.Length}");
                _logger.LogDebug($"目录文件排序前：{string.Join(",", files.ToArray())}");
                _logger.LogDebug($"目录文件排序后：{string.Join(",", files.OrderBy(x => x.Length).ThenBy(x => x))}");
                var finalBytes = new byte[fileSize];
                foreach (var part in files.OrderBy(x => x.Length).ThenBy(x => x))
                {
                    var bytes = System.IO.File.ReadAllBytes(part);

                    await fs.WriteAsync(bytes, 0, bytes.Length);
                    bytes = null;
                    System.IO.File.Delete(part);//删除分块
                }
                fs.Close();
                //这个地方会引发文件被占用异常
                fs = new FileStream(fileFinalName, FileMode.Open);
                var strMd5 = GetCryptoString(fs);
                _logger.LogDebug($"文件数据MD5：{strMd5}");
                _logger.LogDebug($"文件上传数据：{JsonConvert.SerializeObject(requestFile)}");
                fs.Close();
                Directory.Delete(filePath);
                //如果MD5与原MD5不匹配，提示重新上传
                if (strMd5 != requestFile.FileData)
                {
                    _logger.LogDebug($"上传文件md5：{requestFile.FileData},服务器保存文件md5：{strMd5}");
                    return Ok(new ResponseError("MD5值不匹配"));
                }

                CacheUtil.Remove(fileInfo.name);
            }
            catch (Exception ex)
            {
                _logger.LogDebug($"合并文件失败，文件名称：{fileName}，错误信息：{ex.Message}");
                return Ok(new ResponseError("合并文件失败,请重新上传"));
            }
            return Ok(new ResponseInfo());
        }

        /// <summary>
        /// 文件流加密
        /// </summary>
        /// <param name="fileStream"></param>
        /// <returns></returns>
        private static string GetCryptoString(Stream fileStream)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            var cryptBytes = md5.ComputeHash(fileStream);
            return GetCryptoString(cryptBytes);
        }

        private static string GetCryptoString(IEnumerable<byte> cryptBytes)
        {
            //加密的二进制转为string类型返回
            var sb = new StringBuilder();
            foreach (var t in cryptBytes)
            {
                sb.Append(t.ToString("x2"));
            }
            return sb.ToString();
        }

        #endregion

        #region ::::: 简单文件上传 :::::

        [HttpPost("sampleFileUpload")]
        public async Task<IActionResult> SampleFileSave()
        {
            var date = Request;
            var files = Request.Form.Files;
            var size = files.Sum(f => f.Length);
            var webRootPath = _env.WebRootPath;
            var contentRootPath = _env.ContentRootPath;
            foreach (var formFile in files)
            {
                if (formFile.Length <= 0) continue;
                var fileExt = GetFileExt(formFile.FileName); //文件扩展名，不含“.”
                var fileSize = formFile.Length; //获得文件大小，以字节为单位
                var newFileName = Guid.NewGuid() + "." + fileExt; //随机生成新的文件名
                var filePath = webRootPath + "/upload/" + newFileName;
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await formFile.CopyToAsync(stream);
                }
            }

            return Ok(new ResponseInfo{Result = new { count = files.Count, size }});
        }

        private string GetFileExt(string formFileFileName)
        {
            if (string.IsNullOrEmpty(formFileFileName)) return string.Empty;
            var tempArr = formFileFileName.Split('.');
            return tempArr[0];
        }

        #endregion
    }

    public class UploadConfig
    {
        public byte[] Buffer { get; set; }
        public string FileName { get; set; }
        public string Chunked { get; set; }
        public string PreviosName { get; set; }

    }

}
