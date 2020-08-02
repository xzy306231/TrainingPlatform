using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ApiUtil;
using ApiUtil.Entities;
using ApiUtil.Log;
using ApiUtil.Mq;
using AutoMapper;
using LinqKit;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;
using PeopleManager.Api.ViewModel;
using PeopleManager.Core.Entity;
using PeopleManager.Infrastructure.Repository;
using PeopleManager.Api.ViewModel.Server;

namespace PeopleManager.Api.Controllers
{
    [Route("peoplemanager/v1")]
    [EnableCors("any")]
    public class PeopleController : Controller
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<PeopleController> _logger;

        private readonly string _uploadFolder;
        private readonly string _downloadFolder;
        private readonly RabbitMqClient _mqClient;
        private readonly ServiceHelper _service;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="mapper"></param>
        /// <param name="hostingEnvironment"></param>
        /// <param name="logger"></param>
        /// <param name="mqClient"></param>
        /// <param name="service"></param>
        public PeopleController(
            UnitOfWork unitOfWork
            ,IMapper mapper
            ,IHostingEnvironment hostingEnvironment
            ,ILogger<PeopleController> logger
            ,RabbitMqClient mqClient
            ,ServiceHelper service)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _mqClient = mqClient;
            _service = service;

            //上传文件夹
            _uploadFolder = Path.Combine(hostingEnvironment.ContentRootPath, $"PersonInfos{Path.DirectorySeparatorChar}UploadFile");
            if (!Directory.Exists(_uploadFolder)) Directory.CreateDirectory(_uploadFolder);

            //下载文件夹
            _downloadFolder = Path.Combine(hostingEnvironment.ContentRootPath, $"PersonInfos{Path.DirectorySeparatorChar}DownloadFile");
            if (!Directory.Exists(_downloadFolder)) Directory.CreateDirectory(_downloadFolder);
        }

        /// <summary>
        /// 根据条件获取人员信息
        /// </summary>
        /// <param name="category">分类标志,teacher/student</param>
        /// <param name="airModelKey">机型id</param>
        /// <param name="skillLevelKey">技能等级id</param>
        /// <param name="flyStatusKey">飞行状态id</param>
        /// <param name="departmentKey">所属部门</param>
        /// <param name="teacherTypeKey"></param>
        /// <param name="keyword">检索关键字</param>
        /// <param name="page">当前页码</param>
        /// <param name="perPage">每页行数</param>
        /// <returns></returns>
        [HttpGet("{category}")]
        public async Task<IActionResult> GetAllInfos(string category, string airModelKey = "", string skillLevelKey = "", 
            string flyStatusKey = "", string departmentKey = "", string teacherTypeKey = "", string keyword = "", int page = 1, int perPage = 10)
        {
            var predicate = PredicateBuilder.New<PersonInfoEntity>(true).And(entity => entity.DeleteFlag == 0);
            
            if (category.Equals("teacher")) predicate = predicate.And(entity => entity.TeacherFlag.Equals(1));
            else if (category.Equals("student")) predicate = predicate.And(entity => entity.StudentFlag.Equals(1));
            else return Ok(new ResponseError("分类标志信息错误"));
            
            if (!string.IsNullOrEmpty(airModelKey))
            {
                predicate = predicate.And(entity =>
                    entity.WorkInfos.Any(infoEntity => infoEntity.AirplaneModelKey.Equals(airModelKey)));
            }

            if (!string.IsNullOrEmpty(skillLevelKey))
            {
                predicate = predicate.And(entity =>
                    entity.WorkInfos.Any(infoEntity => infoEntity.SkillLevelKey.Equals(skillLevelKey)));
            }

            if (!string.IsNullOrEmpty(flyStatusKey))
            {
                predicate = predicate.And(entity =>
                    entity.WorkInfos.Any(infoEntity => infoEntity.FlyStatusKey.Equals(flyStatusKey)));
            }

            if (!string.IsNullOrEmpty(departmentKey))
            {
                predicate = predicate.And(entity =>
                    entity.WorkInfos.Any(infoEntity => infoEntity.DepartmentKey.Equals(departmentKey)));
            }

            if (!string.IsNullOrEmpty(teacherTypeKey))
            {
                predicate = predicate.And(entity =>
                    entity.WorkInfos.Any(infoEntity => infoEntity.TeacherTypeKey.Equals(teacherTypeKey)));
            }

            if (!string.IsNullOrEmpty(keyword))
            {
                predicate = predicate.And(entity => 
                    EF.Functions.Like(entity.UserNumber, $"%{keyword}%") ||
                    EF.Functions.Like(entity.UserName, $"%{keyword}%"));
            }

            var tempCollection = await _unitOfWork.PersonInfoRepository.GetPageAsync(predicate, entity => entity.Id, page, perPage);
            
            #region WorkInfo转换

            var pageData = new PageData<PersonInfoDto> {Totals = tempCollection.Totals};
            foreach (var row in tempCollection.Rows)
            {
                var dto = _mapper.Map<PersonInfoDto>(row);
                pageData.Rows.Add(dto);
                if(row.WorkInfos == null || row.WorkInfos.Count == 0) continue;
                dto.WorkInfo = _mapper.Map<WorkOfPersonDto>(row.WorkInfos[0]);
            }

            #endregion

            return Ok(new ResponseInfo{Result = pageData});
        }

        /// <summary>
        /// 其他服务筛选展示
        /// </summary>
        /// <param name="category">分类标志,teacher/student</param>
        /// <param name="eductionKey">学历</param>
        /// <param name="airModelKey">机型</param>
        /// <param name="skillLevelKey">技术等级</param>
        /// <param name="flyStatusKey">飞行状态</param>
        /// <param name="cardId">排除对象的工号集合</param>
        /// <param name="durationStart">飞行时长起始值</param>
        /// <param name="durationEnd">飞行时长结束值</param>
        /// <param name="page">页码</param>
        /// <param name="perPage">每页长度</param>
        /// <returns></returns>
        [HttpGet("otherServer/filterShow/{category}")]
        public async Task<IActionResult> TrainingPlanFilterShow(string category, string eductionKey, string airModelKey,
            string skillLevelKey, string flyStatusKey, List<string> cardId, double durationStart = -1, double durationEnd =-1, int page = 1, int perPage = 10)
        {
            _logger.LogDebug(LogHelper.OutputClearness("其他服务筛选展示"));
            var predicate = PredicateBuilder.New<PersonInfoEntity>(true).And(entity => entity.DeleteFlag == 0);

            if (category.Equals("teacher")) predicate = predicate.And(entity => entity.TeacherFlag.Equals(1));
            else if (category.Equals("student")) predicate = predicate.And(entity => entity.StudentFlag.Equals(1));
            else return Ok(new ResponseError("分类标志信息错误"));

            if (!string.IsNullOrEmpty(eductionKey)) predicate = predicate.And(entity => entity.EducationKey.Equals(eductionKey));

            if (!string.IsNullOrEmpty(airModelKey))
                predicate = predicate.And(entity => entity.WorkInfos.Any(infoEntity => infoEntity.AirplaneModelKey.Equals(airModelKey)));

            if (!string.IsNullOrEmpty(skillLevelKey))
                predicate = predicate.And(entity => entity.WorkInfos.Any(infoEntity => infoEntity.SkillLevelKey.Equals(skillLevelKey)));

            if (!string.IsNullOrEmpty(flyStatusKey))
                predicate = predicate.And(entity => entity.WorkInfos.Any(infoEntity => infoEntity.FlyStatusKey.Equals(flyStatusKey)));

            if (durationStart >=0) predicate = predicate.And(entity => entity.WorkInfos.Any(infoEntity => infoEntity.TotalDuration >= durationStart));

            if (durationEnd >= 0) predicate = predicate.And(entity => entity.WorkInfos.Any(infoEntity => infoEntity.TotalDuration < durationEnd));
            
            if (cardId != null && cardId.Count != 0)
            {
                foreach (var card in cardId)
                    predicate = predicate.And(entity => !entity.UserNumber.Equals(card));
            }
            var tempCollection = await _unitOfWork.PersonInfoRepository.GetPageAsync(predicate, entity => entity.Id, page, perPage);
            var result = _mapper.Map<PageData<TrainingPlanShowDto>>(tempCollection);
            return Ok(new ResponseInfo{Result = result });
        }

        /// <summary>
        /// 培训计划筛选人员完毕
        /// </summary>
        /// <param name="category">分类标志,teacher/student</param>
        /// <param name="eductionKey">学历</param>
        /// <param name="airModelKey">机型</param>
        /// <param name="skillLevelKey">技术等级</param>
        /// <param name="flyStatusKey">飞行状态</param>
        /// <param name="cardId">排除对象的工号集合</param>
        /// <param name="selectList">单选人员集合</param>
        /// <param name="selectAll">是否全选</param>
        /// <param name="durationStart">飞行时长起始值</param>
        /// <param name="durationEnd">飞行时长结束值</param>
        /// <returns></returns>
        [HttpGet("otherServer/selectPersons/{category}")]
        public async Task<IActionResult> TrainingPlanSelectPersons(string category, string eductionKey, string airModelKey,
            string skillLevelKey, string flyStatusKey, List<string> cardId, List<string> selectList, bool selectAll, 
            double durationStart = -1, double durationEnd = -1)
        {
            var predicate = PredicateBuilder.New<PersonInfoEntity>(true);

            List<PersonInfoEntity> tempCollection;
            if (!selectAll)
            {
                _logger.LogDebug(LogHelper.OutputClearness("没有选择全部"));
                if (selectList != null && selectList.Count != 0)
                {
                    foreach (var item in selectList)
                    {
                        predicate = predicate.Or(entity => entity.DeleteFlag == 0 && entity.UserNumber == item);
                        _logger.LogDebug(LogHelper.OutputClearness($"选择工号{item}"));
                    }
                    tempCollection = await _unitOfWork.PersonInfoRepository.GetListIncludeFullAsync(predicate);
                }
                else
                {
                    return NoContent();
                }
            }
            else
            {
                _logger.LogDebug(LogHelper.OutputClearness("选择全部人员"));
                if (category.Equals("teacher")) predicate = predicate.And(entity => entity.TeacherFlag.Equals(1));
                else if (category.Equals("student")) predicate = predicate.And(entity => entity.StudentFlag.Equals(1));
                else return Ok(new ResponseError("分类标志信息错误"));

                predicate = predicate.And(entity => entity.DeleteFlag == 0);

                if (!string.IsNullOrEmpty(eductionKey))
                    predicate = predicate.And(entity => entity.EducationKey.Equals(eductionKey));

                if (!string.IsNullOrEmpty(airModelKey))
                    predicate = predicate.And(entity => entity.WorkInfos.Any(infoEntity => infoEntity.AirplaneModelKey.Equals(airModelKey)));

                if (!string.IsNullOrEmpty(skillLevelKey))
                    predicate = predicate.And(entity => entity.WorkInfos.Any(infoEntity => infoEntity.SkillLevelKey.Equals(skillLevelKey)));

                if (!string.IsNullOrEmpty(flyStatusKey))
                    predicate = predicate.And(entity => entity.WorkInfos.Any(infoEntity => infoEntity.FlyStatusKey.Equals(flyStatusKey)));

                if (durationStart >= 0 && durationEnd >= 0)
                    predicate = predicate.And(entity =>
                        entity.WorkInfos.Any(infoEntity => infoEntity.TotalDuration >= durationStart && infoEntity.TotalDuration < durationEnd));

                if (cardId != null && cardId.Count != 0)
                {
                    foreach (var card in cardId)
                    {
                        predicate = predicate.And(entity => entity.UserNumber != card);
                    }
                }
                //tempCollection = await _unitOfWork.PersonInfoRepository.GetListAsync(predicate);
                tempCollection = await _unitOfWork.PersonInfoRepository.GetListIncludeFullAsync(predicate);
            }

            var result = _mapper.Map<List<TrainingPlanSelectDto>>(tempCollection);

            return Ok(result);
        }

        /// <summary>
        /// 获取单个人员信息
        /// </summary>
        /// <param name="category">分类标志,teacher/student</param>
        /// <param name="id">id</param>
        /// <returns></returns>
        [HttpGet("{category}/personalInfo")]
        public async Task<IActionResult> GetInfo(string category, long id)
        {
            var predicate = PredicateBuilder.New<PersonInfoEntity>(true).And(entity => entity.DeleteFlag == 0);

            if (category.Equals("teacher")) predicate = predicate.And(entity => entity.TeacherFlag.Equals(1));
            else if (category.Equals("student")) predicate = predicate.And(entity => entity.StudentFlag.Equals(1));
            else return Ok(new ResponseError("分类标志信息错误"));

            predicate = predicate.And(entity => entity.OriginalId.Equals(id));

            var tempInfoEntity = await _unitOfWork.PersonInfoRepository.GetAsync(predicate);
            var result = _mapper.Map<SinglePersonInfoDto>(tempInfoEntity);

            return Ok(result == null ? new ResponseError("用户不存在") : new ResponseInfo { Result = result });
        }

        /// <summary>
        /// 获取单个人员编辑信息
        /// </summary>
        /// <param name="id">id</param>
        /// <returns></returns>
        [HttpGet("personalEditInfo")]
        public async Task<IActionResult> GetInfo(long id)
        {
            var predicate = PredicateBuilder.New<PersonInfoEntity>(true).And(entity => entity.DeleteFlag == 0);
            
            predicate = predicate.And(entity => entity.OriginalId.Equals(id));

            var tempInfoEntity = await _unitOfWork.PersonInfoRepository.GetAsync(predicate);
            var result = _mapper.Map<PersonEditDto>(tempInfoEntity);

            return Ok(result == null ? new ResponseError("用户不存在") : new ResponseInfo { Result = result });
        }

        /// <summary>
        /// 编辑人员信息
        /// </summary>
        /// <param name="info">更新信息</param>
        /// <param name="id">用户id</param>
        /// <returns></returns>
        [HttpPut("editInfo")]
        public async Task<IActionResult> UpdateInfo([FromBody]PersonEditDto info, long id)
        {
            if (info == null || !ModelState.IsValid) return Ok(new ResponseError("参数提交出错"));

            var temp = await _unitOfWork.PersonInfoRepository.GetAsync(entity => entity.OriginalId.Equals(id));
            var tempWorkInfo = temp?.WorkInfos?.FirstOrDefault();
            if (temp == null) return Ok(new ResponseError("用户不存在"));

            _mapper.Map(info, temp);

            var resultWorkInfo = true;
            if (info.WorkInfos!= null && info.WorkInfos.Count != 0)
            {
                if (tempWorkInfo != null)
                {
                    _mapper.Map(info.WorkInfos[0], tempWorkInfo);
                    resultWorkInfo = await _unitOfWork.WorkInfoRepository.UpdateAsync(tempWorkInfo);
                }
                else
                {
                    var tempWork = _mapper.Map<WorkInfoEntity>(info.WorkInfos[0]);
                    temp.WorkInfos = new List<WorkInfoEntity> {tempWork};
                }
                
            }
            
            var result = await _unitOfWork.PersonInfoRepository.UpdateAsync(temp);

            _mqClient.PushLogMessage(new SystemLogEntity(await _service.GetTokenInfo(), result){LogDesc = $"[{temp.UserName}]的人员信息更新完成"});

            return Ok(result && resultWorkInfo ? new ResponseInfo() : new ResponseError("更新失败!"));
        }

        /// <summary>
        /// 上传个人头像
        /// </summary>
        /// <param name="info"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost("personPhoto")]
        public async Task<IActionResult> PersonPhoto([FromBody] UpdatePhotoDto info, long id)
        {
            if (info == null || !ModelState.IsValid) return Ok(new ResponseError("参数提交出错"));

            var temp = await _unitOfWork.PersonInfoRepository.GetAsync(entity => entity.OriginalId.Equals(id));
            if (temp == null) return Ok(new ResponseError("用户不存在"));
            temp.PhotoPath = info.FilePath;
            var result = await _unitOfWork.PersonInfoRepository.UpdateAsync(temp);

            _mqClient.PushLogMessage(new SystemLogEntity(await _service.GetTokenInfo()){ LogDesc = $"上传[{temp.UserName}]的个人头像" });

            return Ok(result ? new ResponseInfo() : new ResponseError {Message = "上传失败"});
        }

        /// <summary>
        /// 模板上传
        /// </summary>
        /// <param name="file"></param>
        /// <param name="roleType"></param>
        /// <returns></returns>
        [HttpPost("fileUpload")]
        public async Task<IActionResult> FileUpload(IFormFile file, string roleType = "student")
        {
            if (file == null || file.Length == 0) return Ok(new ResponseError("未提交文件"));

            foreach (var folder in Directory.GetDirectories(_uploadFolder))
            {
                Directory.Delete(folder,true);
            }

            var fileExtension = Path.GetExtension(file.FileName);
            bool result;
            if (fileExtension == ".xls" || fileExtension == ".xlsx")
            {
                var fileName = file.FileName;
                var filePath = Path.Combine(_uploadFolder, fileName);
                var fileLocation = new FileInfo(filePath);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }

                if (file.Length <= 0) return Ok(new ResponseError("文件未找到"));

                result = await ReadExcelDataAsync(fileLocation, roleType.Trim().Equals("student"));

                _mqClient.PushLogMessage(new SystemLogEntity(await _service.GetTokenInfo()) {LogDesc = $"上传个人详细信息"});
            }
            else
            {
                return Ok(new ResponseError("文件格式错误!"));
            }
            return Ok(result ? new ResponseInfo() : new ResponseError("文件上传失败"));
        }

        /// <summary>
        /// 模板下载
        /// </summary>
        /// <returns></returns>
        [HttpGet("download")]
        public FileStreamResult FileDownload()
        {
            var fileStream = new FileStream($"{_downloadFolder}{Path.DirectorySeparatorChar}person_info_detail.xlsx",FileMode.Open,FileAccess.Read);
            return File(fileStream, "application/octet-stream", "person_info_detail.xlsx");
        }

        private async Task<bool> ReadExcelDataAsync(FileInfo path, bool isStudent = true)
        {
            bool result = false;
            await Task.Run(async() =>
            {
                using (var package = new ExcelPackage(path))
                {
                    #region ::::: 人员信息 :::::

                    var dataList = new List<PersonInfoEntity>();
                    var worksheet = package.Workbook.Worksheets["人员信息"];
                    int totalRows = worksheet.Dimension.Rows;

                    try
                    {
                        //人员信息读取
                        for (int i = 2; i <= totalRows; i++)
                        {
                            var userNumber = worksheet.Cells[i, 2].Value.ToString();
                            if(string.IsNullOrEmpty(userNumber)) continue;//TODO:数据项错误加载
                            var dataTemp =
                                _unitOfWork.PersonInfoRepository.Get(e => e.UserNumber.Equals(userNumber));
                            if(dataTemp == null) continue;//TODO:数据项错误加载
                            dataTemp.Birthday = GetDateTime(worksheet.Cells[i, 4].Value.ToString());
                            dataTemp.EducationValue = worksheet.Cells[i, 5].Value.ToString(); //TODO:key
                            dataTemp.SchoolTag = worksheet.Cells[i, 6].Value.ToString();
                            dataTemp.HouseAddress = worksheet.Cells[i, 7].Value.ToString();
                            dataTemp.RegularAddress = worksheet.Cells[i, 8].Value.ToString();
                            dataTemp.UserPhone = worksheet.Cells[i, 9].Value.ToString();
                            dataTemp.Nationality = worksheet.Cells[i, 10].Value.ToString();
                            dataTemp.Nation = worksheet.Cells[i, 11].Value.ToString();
                            dataTemp.BloodType = worksheet.Cells[i, 12].Value.ToString();
                            dataTemp.NativePlace = worksheet.Cells[i, 13].Value.ToString();
                            dataTemp.MarriageStatus = worksheet.Cells[i, 14].Value.ToString(); //TODO:key
                            dataTemp.StateOfHealth = worksheet.Cells[i, 15].Value.ToString(); //TODO:key
                            dataTemp.UserEmail = worksheet.Cells[i, 16].Value.ToString();
                            dataTemp.EmploymentDate = GetDateTime(worksheet.Cells[i, 17].Value.ToString());
                            dataTemp.QualificationName = worksheet.Cells[i, 18].Value.ToString();
                            dataTemp.QualificationTypeValue = worksheet.Cells[i, 19].Value.ToString(); //TODO:key
                            dataTemp.QualificationGetDate = GetDateTime(worksheet.Cells[i, 20].Value.ToString());
                            dataTemp.QualificationExpirationDate = GetDateTime(worksheet.Cells[i, 21].Value.ToString());
                            dataList.Add(dataTemp);
                        }

                        #endregion

                        if (dataList.Count != 0)
                        {
                            worksheet = package.Workbook.Worksheets["工作信息"];
                            totalRows = worksheet.Dimension.Rows;
                            for (int i = 2; i <= totalRows; i++)
                            {
                                var userNumber = worksheet.Cells[i, 1].Value.ToString();
                                var tempData = dataList.FirstOrDefault(e => e.UserNumber.Equals(userNumber));
                                if (tempData == null) continue; //TODO:数据项错误加载
                                tempData.StudentFlag = 1;
                                tempData.TeacherFlag = isStudent ? 0 : 1;
                                //新增
                                if (tempData.WorkInfos == null || tempData.WorkInfos.Count == 0)
                                {
                                    tempData.WorkInfos.Add(new WorkInfoEntity
                                    {
                                        DepartmentValue = worksheet.Cells[i, 2].Value.ToString(),
                                        TeacherTypeValue = worksheet.Cells[i, 3].Value.ToString(), //TODO:key
                                        AirplaneModelValue = worksheet.Cells[i, 4].Value.ToString(), //TODO:key
                                        BaseValue = worksheet.Cells[i, 5].Value.ToString(), //TODO:key
                                        HireDate = GetDateTime(worksheet.Cells[i, 6].Value.ToString()),
                                        FlyStatusValue = worksheet.Cells[i, 7].Value.ToString(), //TODO:
                                        TotalDuration = GetDoubleValue(worksheet.Cells[i, 8].Value.ToString()),
                                        TrainingDuration = GetDoubleValue(worksheet.Cells[i, 9].Value.ToString()),
                                        ActualFlightNumber =
                                            (int) GetDoubleValue(worksheet.Cells[i, 10].Value.ToString()),
                                        ActualDuration = GetDoubleValue(worksheet.Cells[i, 11].Value.ToString()),
                                        CurrentActualDuration =
                                            (int) GetDoubleValue(worksheet.Cells[i, 12].Value.ToString()),
                                        CurrentFlightNumber =
                                            (int) GetDoubleValue(worksheet.Cells[i, 13].Value.ToString()),
                                    });
                                }
                                else//修改
                                {
                                    tempData.WorkInfos[0].DepartmentValue = worksheet.Cells[i, 2].Value.ToString();
                                    tempData.WorkInfos[0].TeacherTypeValue =
                                        worksheet.Cells[i, 3].Value.ToString(); //TODO:key
                                    tempData.WorkInfos[0].AirplaneModelValue =
                                        worksheet.Cells[i, 4].Value.ToString(); //TODO:key
                                    tempData.WorkInfos[0].BaseValue = worksheet.Cells[i, 5].Value.ToString(); //TODO:key
                                    tempData.WorkInfos[0].HireDate =
                                        GetDateTime(worksheet.Cells[i, 6].Value.ToString());
                                    tempData.WorkInfos[0].FlyStatusValue =
                                        worksheet.Cells[i, 7].Value.ToString(); //TODO:
                                    tempData.WorkInfos[0].TotalDuration =
                                        GetDoubleValue(worksheet.Cells[i, 8].Value.ToString());
                                    tempData.WorkInfos[0].TrainingDuration =
                                        GetDoubleValue(worksheet.Cells[i, 9].Value.ToString());
                                    tempData.WorkInfos[0].ActualFlightNumber =
                                        (int) GetDoubleValue(worksheet.Cells[i, 10].Value.ToString());
                                    tempData.WorkInfos[0].ActualDuration =
                                        GetDoubleValue(worksheet.Cells[i, 11].Value.ToString());
                                    tempData.WorkInfos[0].CurrentActualDuration =
                                        (int) GetDoubleValue(worksheet.Cells[i, 12].Value.ToString());
                                    tempData.WorkInfos[0].CurrentFlightNumber =
                                        (int) GetDoubleValue(worksheet.Cells[i, 13].Value.ToString());

                                }
                            }

                            worksheet = package.Workbook.Worksheets["执照信息"];
                            totalRows = worksheet.Dimension.Rows;
                            for (int i = 2; i <= totalRows; i++)
                            {
                                var userNumber = worksheet.Cells[i, 1].Value.ToString();
                                var tempData = dataList.FirstOrDefault(e => e.UserNumber.Equals(userNumber));
                                if (tempData == null || tempData.CertificateInfos.Count > 0) continue; //TODO:数据项错误加载
                                tempData.CertificateInfos.Add(new CertificateInfoEntity
                                {
                                    Name = worksheet.Cells[i, 2].Value.ToString(),
                                    Code = worksheet.Cells[i, 3].Value.ToString(),
                                    TypeValue = worksheet.Cells[i, 4].Value.ToString(), //TODO:key
                                    AirplaneModelValue = worksheet.Cells[i, 5].Value.ToString(), //TODO:适用机型
                                    GetDate = GetDateTime(worksheet.Cells[i, 6].Value.ToString()),
                                    ExpirationDate = GetDateTime(worksheet.Cells[i, 7].Value.ToString()),
                                    LastEndorseDate = GetDateTime(worksheet.Cells[i, 8].Value.ToString()),
                                    ValidValue = worksheet.Cells[i, 9].Value.ToString(), //TODO:key
                                });
                            }

                            worksheet = package.Workbook.Worksheets["证照信息"];
                            totalRows = worksheet.Dimension.Rows;
                            for (int i = 2; i <= totalRows; i++)
                            {
                                var userNumber = worksheet.Cells[i, 1].Value.ToString();
                                var tempData = dataList.FirstOrDefault(e => e.UserNumber.Equals(userNumber));
                                if (tempData == null || tempData.LicenseInfos.Count > 0) continue; //TODO:数据项错误加载
                                tempData.LicenseInfos.Add(new LicenseInfoEntity
                                {
                                    LicenseName = worksheet.Cells[i, 2].Value.ToString(),
                                    ValidValue = worksheet.Cells[i, 3].Value.ToString(), //TODO:key
                                    StartDate = GetDateTime(worksheet.Cells[i, 4].Value.ToString()),
                                    EndDate = GetDateTime(worksheet.Cells[i, 5].Value.ToString()),
                                });
                            }

                            worksheet = package.Workbook.Worksheets["奖惩记录"];
                            totalRows = worksheet.Dimension.Rows;
                            for (int i = 2; i <= totalRows; i++)
                            {
                                var userNumber = worksheet.Cells[i, 1].Value.ToString();
                                var tempData = dataList.FirstOrDefault(e => e.UserNumber.Equals(userNumber));
                                if (tempData == null || tempData.RewardsAndPunishments.Count > 0) continue; //TODO:数据项错误加载
                                tempData.RewardsAndPunishments.Add(new RewardsAndPunishmentEntity
                                {
                                    EventName = worksheet.Cells[i, 2].Value.ToString(),
                                    EventTypeValue = worksheet.Cells[i, 3].Value.ToString(),
                                    EventDate = GetDateTime(worksheet.Cells[i, 4].Value.ToString()),
                                    EventResult = worksheet.Cells[i, 5].Value.ToString()
                                });
                            }
                        }

                        if (dataList.Count != 0)
                        {
                            //保存
                            await _unitOfWork.PersonInfoRepository.UpdateAsync(dataList);
                        }

                        result = true;
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e,LogHelper.OutputClearness("转换出错"));
                    }
                }
            });
            return result;
        }

        private static DateTime? GetDateTime(string dateTime)
        {
            if (DateTime.TryParse(dateTime, out var result))
            {
                return result;
            }
            return null;
        }

        private static double GetDoubleValue(string data)
        {
            if (double.TryParse(data, out var temp))
            {
                return temp;
            }
            return 0;
        }
    }
}
