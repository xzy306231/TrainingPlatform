using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiUtil;
using ApiUtil.Entities;
using ApiUtil.Log;
using ApiUtil.Mq;
using AutoMapper;
using Courseware.Api.Common;
using Courseware.Api.ViewModel.Manage;
using Courseware.Api.ViewModel.Manage.Resource;
using Courseware.Core.Entities;
using Courseware.Infrastructure.Repository;
using LinqKit;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Courseware.Api.Controllers
{
    /// <summary>
    /// 课件管理服务
    /// </summary>
    [Route("coursesource/v1/")]
    [EnableCors("any")]
    public class ManageController : Controller
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<ManageController> _logger;
        private readonly RabbitMqClient _mqClient;
        private readonly string _urlPrefix;
        private readonly ServiceHelper _service;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="unitOfWork">仓储</param>
        /// <param name="mapper"></param>
        /// <param name="logger"></param>
        /// <param name="mqClient"></param>
        /// <param name="service"></param>
        public ManageController(
            UnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<ManageController> logger,
            RabbitMqClient mqClient,
            ServiceHelper service)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _mqClient = mqClient;
            _urlPrefix = ConfigUtil.FastWebAddress;
            _service = service;
        }

        /// <summary>
        /// 默认返回所有未删除已审核通过课件资源
        /// </summary>
        /// <param name="tagId">知识点id</param>
        /// <param name="resourceType">video/flash/document/picture/zip</param>
        /// <param name="keyword">关键字</param>
        /// <param name="page">当前页码</param>
        /// <param name="perPage">每页行数</param>
        /// <param name="sortBy">createtime/filesize</param>
        /// <param name="order">asc/desc</param>
        /// <returns></returns>
        [HttpGet("all")]
        public async Task<IActionResult> GetCourseResource(string resourceType, string keyword, long tagId = -1, int page = 1,
            int perPage = 10, string sortBy = "createtime", string order = "asc")
        {
            var sortByTemp = FieldCheck.CheckFieldSortByCreateTimeSize(sortBy);
            var orderTemp = FieldCheck.CheckFieldOrder(order);
            if (sortByTemp == null || orderTemp == null)
            {
                _logger.LogDebug($"sortBy:{sortBy},order:{order},填写有误，自己对比");
                return Ok(new ResponseError("请求参数有误，请验证"));
            }

            var ordering = orderTemp + sortByTemp;
            var predicate = PredicateBuilder.New<ResourceEntity>(true).And(entity => entity.DeleteFlag == 0);//未删除

            predicate = predicate.And(entity => entity.CheckStatus.Equals(FieldCheck.PassStatus));//已审核

            if (tagId != -1)
            {
                var tagTemp = await _unitOfWork.TagRepository.GetAsync(tagEntity => tagEntity.OriginalId.Equals(tagId));
                if (tagTemp != null)
                {
                    predicate = predicate.And(entity => entity.ResourceTags.Any(tagEntity => tagEntity.TagId.Equals(tagTemp.Id)));
                }
                else
                {
                    _logger.LogInformation(LogHelper.OutputClearness($"找不到id为{tagId}的知识点"));
                    return Ok(new ResponseInfo());
                }
            }

            if (!string.IsNullOrEmpty(resourceType))
            {
                var resourceKey = FieldCheck.GetSourceKey(resourceType.Trim().ToLower());
                predicate = predicate.And(entity => entity.ResourceType.Equals(resourceKey));
            }

            if (!string.IsNullOrEmpty(keyword))//关键字检索
            {
                predicate = predicate.And(entity =>
                    EF.Functions.Like(entity.ResourceName, $"%{keyword}%") ||
                    EF.Functions.Like(entity.ResourceTagsDisplay, $"{keyword}"));
            }

            var tempCollection = await _unitOfWork.ResourceRepository.GetPageAsync(predicate, ordering, page, perPage);
            var pageData = _mapper.Map<PageData<ResourceAllDto>>(tempCollection);
            return Ok(new ResponseInfo{ Result = pageData });
        }

        /// <summary>
        /// 默认返回相关用户的所有未删除的课件资源
        /// </summary>
        /// <param name="userId">用户id</param>
        /// <param name="tagId">知识点id</param>
        /// <param name="resourceType">video/flash/document/picture/zip</param>
        /// <param name="keyword">关键字</param>
        /// <param name="page">当前页码</param>
        /// <param name="perPage">每页行数</param>
        /// <param name="sortBy">createtime/filesize</param>
        /// <param name="order">asc/desc</param>
        /// <returns></returns>
        [HttpGet("mine")]
        public async Task<IActionResult> GetUserCourseResource(long userId, string resourceType, string keyword, long tagId = -1,
            int page = 1, int perPage = 10, string sortBy = "createtime", string order = "asc")
        {
            if (userId == 0) return Ok(new ResponseError("userId不存在"));
            var sortByTemp = FieldCheck.CheckFieldSortByCreateTimeSize(sortBy);
            var orderTemp = FieldCheck.CheckFieldOrder(order);
            if (sortByTemp == null || orderTemp == null)
            {
                _logger.LogInformation(LogHelper.OutputClearness($"sortBy:{sortBy},order:{order},填写有误，自己对比"));
                return Ok(new ResponseError("请求参数有误，请验证"));
            }
            var ordering = orderTemp + sortByTemp;

            var predicate = PredicateBuilder.New<ResourceEntity>(true).And(entity => entity.DeleteFlag == 0);//未删除

            predicate = predicate.And(entity => entity.CreatorId.Equals(userId));

            if (tagId != -1)
            {
                var tagTemp = await _unitOfWork.TagRepository.GetAsync(tagEntity => tagEntity.OriginalId.Equals(tagId));
                if (tagTemp != null)
                {
                    predicate = predicate.And(entity => entity.ResourceTags.Any(tagEntity => tagEntity.TagId.Equals(tagTemp.Id)));
                }
                else
                {
                    _logger.LogInformation(LogHelper.OutputClearness($"找不到id为{tagId}的知识点"));
                    return Ok(new ResponseInfo());
                }
            }

            if (!string.IsNullOrEmpty(resourceType))
            {
                var resourceKey = FieldCheck.GetSourceKey(resourceType.Trim().ToLower());
                predicate = predicate.And(entity => entity.ResourceType.Equals(resourceKey));
            }

            if (!string.IsNullOrEmpty(keyword))//关键字检索
            {
                predicate = predicate.And(entity =>
                    EF.Functions.Like(entity.ResourceName, $"%{keyword}%") ||
                    EF.Functions.Like(entity.ResourceTagsDisplay, $"{keyword}"));
            }

            var tempCollection = await _unitOfWork.ResourceRepository.GetPageAsync(predicate, ordering, page, perPage);
            var pageData = _mapper.Map<PageData<ResourceMineDto>>(tempCollection);
            return Ok(new ResponseInfo{Result = pageData});
        }

        /// <summary>
        /// 课件预览
        /// </summary>
        /// <param name="id">资源id</param>
        /// <returns></returns>
        [HttpGet("preview")]
        public async Task<IActionResult> PreviewCourseware(long id = 0)
        {
            if (id == 0) return Ok(new ResponseError("资源id不能为0"));
            var tempResource = await _unitOfWork.ResourceRepository.GetAsync(entity => entity.Id.Equals(id));
            return Ok(tempResource == null
                ? new ResponseError($"资源id为{id}的课件不存在或已被删除")
                : new ResponseInfo
                {
                    Result = new
                    {
                        transfType = tempResource.TransfType,
                        resourceUrl = tempResource.ResourceType == "5"
                            ? FileSystemHelper.GetScoUrl(tempResource.PathToFolder, tempResource.PathToIndex)
                            : $"{_urlPrefix}{tempResource.GroupName}/{tempResource.TransformUrl}"
                    }
                });
        }

        /// <summary>
        /// 单条课件详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("singleDetail")]
        public async Task<IActionResult> SingleDetail(long id = 0)
        {
            if (id == 0) return Ok(new ResponseError("资源id不能为0"));
            var tempResource = await _unitOfWork.ResourceRepository.GetAsync(entity => entity.Id.Equals(id));
            if (tempResource == null) return Ok(new ResponseError($"资源id为{id}的课件不存在或已被删除"));
            var result = _mapper.Map<ResourceAllDto>(tempResource);
            return Ok(new ResponseInfo {Result = result});
        }

        /// <summary>
        /// 设置知识点标签
        /// </summary>
        /// <param name="relations"></param>
        /// <returns></returns>
        [HttpPut("mine/tag")]
        public async Task<IActionResult> ResourceSetTags([FromBody] IList<SetTagDto> relations)
        {
            _logger.LogDebug(LogHelper.OutputClearness($"Start"));
            if (!ModelState.IsValid) return Ok(new ResponseError("参数验证失败"));
            var errorStr = string.Empty;
            if (relations == null || relations.Count == 0) return Ok(new ResponseError("参数为空"));

            #region ::::: 参数中的知识点去重 :::::

            var newTagDic = new Dictionary<long, KnowledgeTagDto>();
            foreach (var dto in relations)
            {
                if(dto.Tags == null || dto.Tags.Count == 0) continue;
                foreach (var tag in dto.Tags)
                {
                    if(newTagDic.ContainsKey(tag.OriginalId)) continue;
                    newTagDic.Add(tag.OriginalId, tag);

                    _logger.LogDebug(LogHelper.OutputClearness($"参数中的知识点去重:key{tag.OriginalId}--value{tag.TagName}"));
                }
            }
            
            #endregion

            #region ::::: 新增知识点 :::::

            foreach (var tag in newTagDic.Values)
            {
                var tempTag = await _unitOfWork.TagRepository.GetAsync(entity => entity.OriginalId.Equals(tag.OriginalId));
                if (tempTag != null)
                {
                    continue;
                }
                var insertTag = _mapper.Map<KnowledgeTagEntity>(tag);
                _logger.LogDebug(LogHelper.OutputClearness($"新增一个知识点，开始"));
                await _unitOfWork.TagRepository.InsertAsync(insertTag);
                _logger.LogDebug(LogHelper.OutputClearness($"新增一个id为{insertTag.Id}的知识点"));
            }

            #endregion

            #region ::::: 创建关系 :::::

            foreach (var tagDto in relations)
            {
                _logger.LogDebug(LogHelper.OutputClearness($"设置资源id为{tagDto.Id}的知识点"));
                var result = await SingleSourceSetTags(tagDto);
                errorStr += result ? string.Empty : $"{tagDto.Id}," ;
            }

            #endregion

            _mqClient.PushLogMessage(new SystemLogEntity(await _service.GetTokenInfo()) {LogDesc = "知识点设置完成"});

            return Ok(string.IsNullOrEmpty(errorStr) ? new ResponseInfo() : new ResponseError($"id为{errorStr.Trim().TrimEnd(',')}的课件知识点设置失败"));
        }

        private async Task<bool> SingleSourceSetTags(SetTagDto tagDto)
        {
            #region ::::: 组合更新关系对象 :::::

            var resource = await _unitOfWork.ResourceRepository.
                GetResourceAsync(entity => entity.Id.Equals(tagDto.Id));
            
            var tags = new List<long>();
            var display = string.Empty;
            foreach (var tag in tagDto.Tags)
            {
                var temp = _unitOfWork.TagRepository.Get(entity => entity.OriginalId.Equals(tag.OriginalId));
                if (temp == null) continue;
                tags.Add(temp.Id);
                display += $"{tag.TagName}  |  ";
            }
            var tagDistinct = tags.Distinct().ToList();


            #endregion

            var relation = new ResToTag { Resource = resource, Tags = tagDistinct };

            var model = await _unitOfWork.ResourceRepository.Entities.Include(x => x.ResourceTags)
                .FirstOrDefaultAsync(x => x.Id == relation.Resource.Id);
            //多对多关系更新
            await _unitOfWork.ResourceTagRepository.TryUpdateManyToMany(model.ResourceTags,
                relation.Tags.Select(x => new ResourceTagEntity {TagId = x, ResourceId = relation.Resource.Id}),
                x => x.TagId);
            //更新
            var result = await _unitOfWork.ResourceRepository.SaveChangesAsync();
            _logger.LogDebug(LogHelper.OutputClearness($"id为{resource.Id}的资源更新知识点数量{result}"));
            if (result > 0)
            {
                model.ResourceTagsDisplay = display;
                var changeResult = await _unitOfWork.ResourceRepository.UpdateAsync(model);
                return changeResult;
            }

            return true;
        }

        /// <summary>
        /// 资源名修改
        /// </summary>
        /// <param name="resource"></param>
        /// <returns></returns>
        [HttpPatch("mine/rename")]
        public async Task<IActionResult> ResourceRename([FromBody] RenameDto resource)
        {
            if (!ModelState.IsValid) return Ok(new ResponseError("查询条件有误！"));

            var tempRec = _unitOfWork.ResourceRepository.Entities.FirstOrDefault(entity => entity.Id.Equals(resource.Id));
            if (tempRec == null) return Ok(new ResponseError("资源不存在！"));
            tempRec.ResourceName = resource.ResourceName;
            var result = await _unitOfWork.ResourceRepository.UpdateAsync(tempRec);

            _mqClient.PushLogMessage(new SystemLogEntity(await _service.GetTokenInfo(), result)
                {LogDesc = $"id为{resource.Id}的课件名称修改为[{resource.ResourceName}]{(result ? "成功" : "失败")}"});
            return Ok(result ? new ResponseInfo() : new ResponseError("资源名修改失败"));
        }

        #region ::::: 审核 :::::

        /// <summary>
        /// 已审核资源
        /// </summary>
        /// <param name="keyword">关键字</param>
        /// <param name="page">页码</param>
        /// <param name="perPage">每页行数</param>
        /// <param name="sortBy">排序字段,createtime/</param>
        /// <param name="order">正序反序</param>
        /// <returns></returns>
        [HttpGet("checked")]
        public async Task<IActionResult> SourceChecked(string keyword, int page = 1, int perPage = 20, string sortBy = "checktime", string order = "desc")
        {
            var sortByTemp = FieldCheck.CheckFieldSortByCheckTimeSize(sortBy);
            var orderTemp = FieldCheck.CheckFieldOrder(order);
            if (sortByTemp == null || orderTemp == null) return Ok(new ResponseError("请求参数有误，请验证"));

            var ordering = orderTemp + sortByTemp;
            var predicate = PredicateBuilder.New<ResourceEntity>(true).And(entity => entity.DeleteFlag == 0);//未删除
            //已审核，包括通过和未通过
            predicate = predicate.And(entity => entity.CheckStatus.Equals(FieldCheck.PassStatus) || entity.CheckStatus.Equals(FieldCheck.FailStatus));

            if (!string.IsNullOrEmpty(keyword))//关键字检索
                predicate = predicate.And(entity => EF.Functions.Like(entity.ResourceName, $"%{keyword}%") || EF.Functions.Like(entity.ResourceTagsDisplay, $"{keyword}"));

            var tempCollection = await _unitOfWork.ResourceRepository.GetPageAsync(predicate, ordering, page, perPage);
            var pageData = _mapper.Map<PageData<ResourceCheckedDto>>(tempCollection);
            return Ok(new ResponseInfo{ Result = pageData});
        }

        /// <summary>
        /// 未审核资源
        /// </summary>
        /// <param name="keyword">关键字</param>
        /// <param name="page">页码</param>
        /// <param name="perPage">每页行数</param>
        /// <param name="sortBy">排序字段,createtime/</param>
        /// <param name="order">正序反序</param>
        /// <returns></returns>
        [HttpGet("unchecked")]
        public async Task<IActionResult> SourceUnChecked(string keyword, int page = 1, int perPage = 20, string sortBy = "createtime", string order = "desc")
        {
            var sortByTemp = FieldCheck.CheckFieldSortByCreateTimeSize(sortBy);
            var orderTemp = FieldCheck.CheckFieldOrder(order);
            if (sortByTemp == null || orderTemp == null) return Ok(new ResponseError("请求参数有误，请验证"));

            var ordering = orderTemp + sortByTemp;
            var predicate = PredicateBuilder.New<ResourceEntity>(true).And(entity => entity.DeleteFlag == 0);//未删除
            predicate = predicate.And(entity => entity.TransfType.Equals("0"));//转换完成
            predicate = predicate.And(entity => entity.CheckStatus.Equals(FieldCheck.CheckingStatus));//审核中
            if (!string.IsNullOrEmpty(keyword))//关键字检索
                predicate = predicate.And(entity => EF.Functions.Like(entity.ResourceName, $"%{keyword}%") || EF.Functions.Like(entity.ResourceTagsDisplay, $"{keyword}"));

            var tempCollection = await _unitOfWork.ResourceRepository.GetPageAsync(predicate, ordering, page, perPage);

            var pageData = _mapper.Map<PageData<ResourceUnCheckedDto>>(tempCollection);
            return Ok(new ResponseInfo{ Result = pageData});
        }

        /// <summary>
        /// 批量同意或拒绝
        /// </summary>
        /// <param name="option">pass/fail</param>
        /// <param name="multiResourceOption"></param>
        /// <returns></returns>
        [HttpPatch("unchecked/multi/{option}")]
        public async Task<IActionResult> MultiResourceOption(string option, [FromBody] MultiCheckOption multiResourceOption)
        {
            #region ::::: 校验 :::::

            if (!option.Equals("pass") && !option.Equals("fail")) return Ok(new ResponseError($"option请求参数有误，请验证{option}"));
            if (!ModelState.IsValid) return Ok(new ResponseError("multiResourceOption有误，请验证"));

            #endregion

            var checkResultStr = option.Equals("pass") ? "通过" : "拒绝";

            var valueTemp = await _service.GetTokenInfo();//获取Token

            string errorStr = string.Empty;
            foreach (var checkId in multiResourceOption.CheckIdList)
            {
                var resultTemp = _unitOfWork.ResourceRepository.Get(entity => entity.Id.Equals(checkId));
                if (resultTemp == null || resultTemp.CheckStatus != FieldCheck.CheckingStatus)
                {
                    errorStr += $"{checkId},";
                    continue;
                }

                resultTemp.CheckDate = DateTime.Now;
                resultTemp.CheckStatus = option.Equals("pass") ? FieldCheck.PassStatus : FieldCheck.FailStatus;
                resultTemp.CheckerId = multiResourceOption.CheckerId;
                resultTemp.CheckerName = multiResourceOption.CheckerName;

                _logger.LogDebug(LogHelper.OutputClearness(
                    $"批量审核：Id[{resultTemp.CheckerId}],name[{resultTemp.CheckerName}],result[{resultTemp.CheckStatus}]"));

                var result = await _unitOfWork.ResourceRepository.UpdateAsync(resultTemp);

                if (!result) errorStr += $"{checkId},";//数据库执行失败

                _mqClient.PushLogMessage(new SystemLogEntity(valueTemp, result)
                {
                    LogDesc = $"课件[{resultTemp.ResourceName}]审核" + (result
                                  ? $"[{checkResultStr}]，审核人为{resultTemp.CheckerName}"
                                  : "[操作失败]，原因：资源已审核或找不到相应的资源")
                });

                if (result)
                {
                    _mqClient.PushTodoMessage(new TodoEntity(true, resultTemp.Id)
                    { Body = $"名称为[{resultTemp.ResourceName}]的课件已审核[{checkResultStr}]，审核人为{resultTemp.CheckerName}" });
                }
            }

            if (!string.IsNullOrEmpty(errorStr))
            {
                _logger.LogInformation(LogHelper.OutputClearness($"id为{errorStr}的资源审核失败，原因：资源已审核或找不到相应的资源"));
                return Ok(new ResponseError($"id为{errorStr}的资源审核失败，原因：资源已审核或找不到相应的资源"));
            }

            return Ok(new ResponseInfo());
        }

        /// <summary>
        /// 审核单条资源
        /// </summary>
        /// <param name="option">pass/fail</param>
        /// <param name="checkOption">json</param>
        /// <returns></returns>
        [HttpPatch("unchecked/{option}")]
        public async Task<IActionResult> SingleResourceOption(string option,[FromBody] CheckOption checkOption)
        {
            if (!option.Equals("pass") && !option.Equals("fail")) return Ok(new ResponseError($"option请求参数有误，请验证{option}"));
            if (!ModelState.IsValid) return Ok(new ResponseError("resourceOption参数不合规，请验证"));

            var checkResultStr = option.Equals("pass") ? "通过" : "拒绝";

            var resultResource = _unitOfWork.ResourceRepository.Get(entity => entity.Id.Equals(checkOption.Id));
            if (resultResource == null || resultResource.CheckStatus != FieldCheck.CheckingStatus) return Ok(new ResponseError("资源不存在或者资源已审核"));

            resultResource.CheckDate = DateTime.Now;
            resultResource.CheckerId = checkOption.CheckerId;
            resultResource.CheckerName = checkOption.CheckerName;
            resultResource.CheckRemark = checkOption.CheckRemark;
            resultResource.CheckStatus = option.Equals("pass") ? FieldCheck.PassStatus : FieldCheck.FailStatus;

            _logger.LogDebug(LogHelper.OutputClearness(
                $"单课件审核：Id[{resultResource.CheckerId}],name[{resultResource.CheckerName}],result[{resultResource.CheckStatus}]"));

            var result = await _unitOfWork.ResourceRepository.UpdateAsync(resultResource);

            _mqClient.PushLogMessage(new SystemLogEntity(await _service.GetTokenInfo(), result)
            {
                LogDesc = $"课件[{resultResource.ResourceName}]审核" + (result
                              ? $"[{checkResultStr}]，审核人为{resultResource.CheckerName}"
                              : "[操作失败]，原因：资源已审核或找不到相应的资源")
            });

            if (result)
            {
                _mqClient.PushTodoMessage(new TodoEntity(true, resultResource.Id)
                { Body = $"名称为[{resultResource.ResourceName}]的课件已审核[{checkResultStr}]，审核人为{resultResource.CheckerName}"});
            }

            return Ok(!result ? new ResponseError("审核失败!失败原因：数据库更新失败！") : new ResponseInfo());
        }

        #endregion

        /// <summary>
        /// 单个删除
        /// </summary>
        /// <param name="id">资源id</param>
        /// <returns></returns>
        [HttpDelete("singleResource")]
        public async Task<IActionResult> DeleteSingleResource(long id = -1)
        {
            if (id == -1) return Ok(new ResponseError("没有收到待删除的资源id"));

            var resultResource = await _unitOfWork.ResourceRepository.GetAsync(entity => entity.Id.Equals(id));

            if (resultResource == null) return Ok(new ResponseError("资源未找到，删除失败！"));

            resultResource.DeleteFlag = 1;
            var result = await _unitOfWork.ResourceRepository.UpdateAsync(resultResource);

            _mqClient.PushLogMessage(new SystemLogEntity(await _service.GetTokenInfo(), result, OptionType.Delete)
                { LogDesc = $"id为[{id}]的课件删除操作{(result ? "成功" : "失败，原因：资源已删除或找不到相应的资源")}"});

            _mqClient.PushTodoMessage(new TodoEntity(true, resultResource.Id)
            { Body = $"名称为[{resultResource.ResourceName}]的课件已处理" });

            return Ok(result ? new ResponseInfo() : new ResponseError("资源删除失败，失败原因：数据库更新失败"));
        }

        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="delObjects">需要删除的对象</param>
        /// <returns></returns>
        [HttpDelete("multiResource")]
        public async Task<IActionResult> DeleteMultiResource([FromBody]IList<long> delObjects)
        {
            if (delObjects == null || delObjects.Count == 0)
                return Ok(new ResponseError("没有收到待删除的资源id"));

            var valueTemp = await _service.GetTokenInfo();

            string errorStr = string.Empty;
            foreach (var deleteId in delObjects)
            {
                var resultTemp = _unitOfWork.ResourceRepository.Get(entity => entity.Id.Equals(deleteId));
                if (resultTemp == null || resultTemp.DeleteFlag == 1)
                {
                    errorStr += $"{deleteId},";
                    continue;
                }

                resultTemp.DeleteFlag = 1;

                var result = await _unitOfWork.ResourceRepository.UpdateAsync(resultTemp);//执行数据库操作

                if (!result) errorStr += $"{deleteId},";//数据库执行失败

                _mqClient.PushLogMessage(new SystemLogEntity(valueTemp, result, OptionType.Delete)
                    {LogDesc = $"名称为[{resultTemp.ResourceName}]的课件{(result ? "已删除" : "删除失败，原因：资源已删除或找不到相应的资源")}"});

                _mqClient.PushTodoMessage(new TodoEntity(true, resultTemp.Id){ Body = $"名称为[{resultTemp.ResourceName}]的课件已处理" });
            }

            if (string.IsNullOrEmpty(errorStr)) return Ok(new ResponseInfo());

            _logger.LogInformation(LogHelper.OutputClearness($"id为{errorStr}的资源删除失败，原因：资源已删除或找不到相应的资源"));
            return Ok(new ResponseError($"id为{errorStr}的资源删除失败，原因：资源已删除或找不到相应的资源"));

        }
    }
}
