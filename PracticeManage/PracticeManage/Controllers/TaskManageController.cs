using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ApiUtil;
using ApiUtil.Entities;
using ApiUtil.Log;
using ApiUtil.Mq;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PracticeManage.Entity;
using PracticeManage.ViewModel.Subject;
using PracticeManage.ViewModel.Task;

namespace PracticeManage.Controllers
{
    /// <summary>
    /// 训练任务服务
    /// </summary>
    [Route("practice/v1/task")]
    public class TaskManageController : Controller
    {
        private readonly IFreeSql _fSql;
        private readonly IMapper _mapper;
        private readonly ILogger<TaskManageController> _logger;
        private readonly RabbitMqClient _mqClient;
        private readonly ServiceHelper _service;

        public TaskManageController(IFreeSql fSql, IMapper mapper,
            ILogger<TaskManageController> logger, RabbitMqClient rabbitMq, ServiceHelper service)
        {
            _mapper = mapper;
            _logger = logger;
            _mqClient = rabbitMq;
            _service = service;
            _fSql = fSql;
        }

        /// <summary>
        /// 训练任务一览
        /// </summary>
        /// <param name="taskTypeKey">任务类型</param>
        /// <param name="typeLevelKey">类别等级</param>
        /// <param name="levelKey">级别等级</param>
        /// <param name="keyword">关键字</param>
        /// <param name="tagId">知识点id</param>
        /// <param name="page">当前页</param>
        /// <param name="perPage">每页行</param>
        /// <param name="sortBy">createtime/hour</param>
        /// <param name="order">asc/desc</param>
        /// <returns></returns>
        [HttpGet("all")]
        public async Task<IActionResult> AllTask(string taskTypeKey, string typeLevelKey, string levelKey,
            string keyword, int tagId = 0, int page = 1, int perPage = 10, string sortBy = "createtime", string order = "desc")
        {
            var result = await _fSql.Select<TaskEntity>()
                .IncludeMany(t=>t.SubjectBusEntities, then=>then.IncludeMany(sb=>sb.Tags))
                .WhereIf(!string.IsNullOrEmpty(taskTypeKey), t => t.TaskTypeKey.Equals(taskTypeKey))
                .WhereIf(!string.IsNullOrEmpty(typeLevelKey), t => t.TypeLevelKey.Equals(typeLevelKey))
                .WhereIf(!string.IsNullOrEmpty(levelKey), t => t.LevelKey.Equals(levelKey))
                .WhereIf(!string.IsNullOrEmpty(keyword), t => t.TaskName.Contains(keyword))
                .WhereIf(tagId != 0, task=>task.SubjectBusEntities.AsSelect().Any(sb=>sb.Tags.AsSelect().Any(t=>t.OriginalId.Equals(tagId))))
                //.WhereIf(tagId != 0, t => t.SubjectBusEntities.Exists(s => s.Tags.Exists(tag => tag.Id.Equals(tagId))))
                .WhereCascade(t=>t.DeleteFlag==0)
                .OrderBy(sortBy.Equals("createtime") && order.Equals("asc"), t => t.CreateTime)
                .OrderBy(sortBy.Equals("hour") && order.Equals("asc"), t => t.ClassHour)
                .OrderByDescending(sortBy.Equals("createtime") && order.Equals("desc"), t => t.CreateTime)
                .OrderByDescending(sortBy.Equals("hour") && order.Equals("desc"), t => t.ClassHour)
                .Count(out var itemCount)
                .Page(page, perPage).ToListAsync(a => new TaskQueryDto());
            return Ok(new ResponseInfo {Result = new PageData<TaskQueryDto> {Rows = result, Totals = itemCount}});
        }

        /// <summary>
        /// 获取多个训练任务的详细信息
        /// </summary>
        /// <param name="idList"></param>
        /// <returns></returns>
        [HttpPost("multipleDetails")]//[HttpGet("singleTask")]
        public async Task<IActionResult> MultipleDetails([FromBody]List<long> idList)
        {
            var results = new List<TaskFullDto>();
            var errorIds = string.Empty;
            foreach (var id in idList)
            {
                var result = await _fSql.Select<TaskEntity>()
                    .Where(t => t.Id.Equals(id))
                    .IncludeMany(t => t.SubjectBusEntities, then => then.IncludeMany(s => s.Tags))
                    .WhereCascade(t => t.DeleteFlag == 0)
                    .ToOneAsync();
                if (result != null)
                    results.Add(_mapper.Map<TaskFullDto>(result));
                else
                    errorIds += $"{id},";
            } 

            if(string.IsNullOrEmpty(errorIds))
                return Ok(new ResponseInfo {Result = results });

            _logger.LogInformation(LogHelper.OutputClearness($"获取id{idList}的任务失败,该任务不存在"));
            return Ok(new ResponseError($"获取id{idList}的任务失败,该任务不存在"));
        }

        /// <summary>
        /// 创建训练任务
        /// </summary>
        /// <param name="taskInfo"></param>
        /// <returns></returns>
        [HttpPost("new")]
        public async Task<IActionResult> CreateTask([FromBody] TaskNewDto taskInfo)
        {
            if (taskInfo == null || !ModelState.IsValid)
            {
                _logger.LogInformation(LogHelper.OutputClearness($"没有新建任务,请重新提交"));
                return Ok(new ResponseError($"没有新建任务,请重新提交"));
            }
            //先插任务，获取id
            var taskId = await _fSql.Insert<TaskEntity>().AppendData(_mapper.Map<TaskEntity>(taskInfo)).ExecuteIdentityAsync();
            if (taskInfo.SubjectList != null && taskInfo.SubjectList.Count != 0)
            {
                var tagDisplay = await CreateTaskSubjectRef(taskInfo.SubjectList, taskId);
                await _fSql.Update<TaskEntity>(taskId).Set(t => t.TagDisplay, tagDisplay).ExecuteAffrowsAsync();
            }

            #region ::::: 日志 :::::

            _mqClient.PushLogMessage(new SystemLogEntity(await _service.GetTokenInfo(), taskId > 0, OptionType.Create)
            { LogDesc = $"创建训练任务[{taskInfo.TaskName}]成功" });

            #endregion

            return Ok(new ResponseInfo());
        }

        /// <summary>
        /// 编辑训练任务
        /// </summary>
        /// <param name="taskInfo"></param>
        /// <returns></returns>
        [HttpPut("edit")]
        public async Task<IActionResult> EditTaskTest([FromBody] TaskUpdateDto taskInfo)
        {
            if (taskInfo?.SubjectList == null || taskInfo.SubjectList.Count == 0)
            {
                _logger.LogInformation(LogHelper.OutputClearness($"没有提交任务或科目,请重新提交"));
                return Ok(new ResponseError($"没有提交任务或科目,请重新提交"));
            }
            var taskEntity = await _fSql.Select<TaskEntity>()
                .Where(t => t.Id.Equals(taskInfo.Id))
                .IncludeMany(t => t.SubjectBusEntities, then => then.IncludeMany(s => s.Tags))
                .WhereCascade(t => t.DeleteFlag == 0)
                .ToOneAsync();

            if(taskEntity == null) return Ok(new ResponseError("任务不存在"));

            _mapper.Map(taskInfo, taskEntity);
            var updateTaskResult = await _fSql.Update<TaskEntity>().SetSource(taskEntity).ExecuteAffrowsAsync();

            foreach (var subjectEntity in taskEntity.SubjectBusEntities)
            {
                var tempResult = await _fSql.Update<SubjectBusEntity>(subjectEntity.Id).Set(s => s.DeleteFlag, 1).ExecuteAffrowsAsync();
                _logger.LogInformation(LogHelper.OutputClearness($"删除id为{subjectEntity.Id}的科目"));
            }

            if(taskInfo.SubjectList != null && taskInfo.SubjectList.Count != 0)
            {
                var taskDisplay = await CreateTaskSubjectRef(taskInfo.SubjectList, taskEntity.Id);
                await _fSql.Update<TaskEntity>(taskEntity.Id).Set(t => t.TagDisplay, taskDisplay).ExecuteAffrowsAsync();
            }

            #region ::::: 日志 :::::

            _mqClient.PushLogMessage(new SystemLogEntity(await _service.GetTokenInfo(), true)
            { LogDesc = $"编辑id[{taskEntity.Id}]的训练任务成功" });

            #endregion

            return Ok(new ResponseInfo());
        }

        /// <summary>
        /// 删除一个训练任务
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        [HttpDelete("removeSingle")]
        public async Task<IActionResult> DeleteSingleTask(long id, string name)
        {
            var result = await _fSql.Update<TaskEntity>(id).Set(t => t.DeleteFlag, 1).ExecuteAffrowsAsync();

            #region ::::: 日志 :::::

            _mqClient.PushLogMessage(new SystemLogEntity(await _service.GetTokenInfo(), result > 0, OptionType.Delete)
            { LogDesc = $"删除id为[{id}],名称为[{name}]训练任务{(result > 0 ? "成功" : "失败")}" });

            #endregion

            return Ok(result > 0 ? new ResponseInfo() : new ResponseError($"id为{id}的训练任务删除失败"));
        }

        /// <summary>
        /// 删除多个训练任务
        /// </summary>
        /// <param name="idList"></param>
        /// <returns></returns>
        [HttpDelete("removeMulti")]
        public async Task<IActionResult> DeleteMultiTasks([FromBody] List<int> idList)
        {
            if (idList == null || idList.Count == 0) return Ok(new ResponseError("参数列表为空"));
            int count = 0;
            foreach (var id in idList)
            {
                count += await _fSql.Update<TaskEntity>(id).Set(t => t.DeleteFlag, 1).ExecuteAffrowsAsync();
            }

            var result = Math.Abs(count - idList.Count);
            #region ::::: 日志 :::::

            _mqClient.PushLogMessage(new SystemLogEntity(await _service.GetTokenInfo(), result == 0, OptionType.Delete)
            { LogDesc = $"批量删除训练任务{(result == 0 ? "成功" : "失败")}" });

            #endregion

            return Ok(result == 0 ? new ResponseInfo() : new ResponseError($"删除失败,[{result}]条数据删除失败"));
        }

        private async Task<string> CreateTaskSubjectRef(List<long> subjectList, long taskId)
        {
            var tagNames = new List<string>();
            foreach (var subjectId in subjectList)
            {
                //获取科目资源
                var subjectEntity = await _fSql.Select<SubjectEntity>().Where(s => s.Id.Equals(subjectId)).IncludeMany(s => s.Tags).ToOneAsync();
                if (subjectEntity == null)
                {
                    _logger.LogInformation(LogHelper.OutputClearness($"获取id为{subjectId}的科目失败，不存在这个科目"));
                    continue;
                }
                var subjectBusDto = _mapper.Map<SubjectBusNewDto>(subjectEntity);//转成只有科目没有知识点的dto
                subjectBusDto.TaskId = taskId;//添加任务id
                var subjectBusId = await _fSql.Insert<SubjectBusEntity>()
                    .AppendData(_mapper.Map<SubjectBusEntity>(subjectBusDto)).ExecuteIdentityAsync();//先新增科目业务资源，获取id
                _logger.LogInformation(LogHelper.OutputClearness($"新增id为{subjectBusId}的科目"));
                //新增关联，科目业务和原知识点之间的关联关系
                foreach (var tag in subjectEntity.Tags)
                {
                    if (!tagNames.Contains(tag.TagName)) tagNames.Add(tag.TagName);
                    await _fSql.Insert<SubjectBusTagRefEntity>().AppendData(new SubjectBusTagRefEntity
                        { SubjectBusEntity_id = subjectBusId, TagEntity_id = tag.Id }).ExecuteAffrowsAsync();
                    _logger.LogInformation(LogHelper.OutputClearness($"新增科目{subjectBusId}知识点{tag.Id}的关联"));
                }
            }

            var tagDisplay = string.Empty;
            foreach (var tagName in tagNames)
            {
                tagDisplay += $"{tagName},";
            }

            return tagDisplay.Trim(',');
        }

    }
}
