using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiUtil;
using ApiUtil.Entities;
using ApiUtil.Log;
using ApiUtil.Mq;
using AutoMapper;
using LinqKit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TrainingTask.Api.Common;
using TrainingTask.Api.ViewModel.Task;
using TrainingTask.Core.Entity;
using TrainingTask.Infrastructure.Repository;

namespace TrainingTask.Api.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Route("taskManage/v1")]
    public partial class TaskManageController : Controller
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<TaskManageController> _logger;
        private readonly RabbitMqClient _mqClient;
        private readonly ServiceHelper _service;
        private readonly RedisUtil _redis;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="mapper"></param>
        /// <param name="logger"></param>
        /// <param name="rabbitMq"></param>
        /// <param name="service"></param>
        /// <param name="redis"></param>
        public TaskManageController(
            UnitOfWork unitOfWork
            , IMapper mapper
            , ILogger<TaskManageController> logger
            , RabbitMqClient rabbitMq
            , ServiceHelper service
            , RedisUtil redis)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _logger.LogInformation(LogHelper.OutputClearness("测试构造函数"));
            _mqClient = rabbitMq;
            _service = service;
            _redis = redis;
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
        [HttpGet("allTask")]
        public async Task<IActionResult> AllTask(string taskTypeKey, string typeLevelKey, string levelKey, string keyword, 
            int tagId = 0, int page = 1, int perPage = 10, string sortBy = "createtime", string order = "desc")
        {
            var sortByTemp = FieldCheck.SortByCreateTime_hour(sortBy);
            var orderTemp = FieldCheck.Order(order);

            if (sortByTemp == null || orderTemp == null)
            {
                _logger.LogDebug($"sortBy:{sortBy},order:{order},填写有误，自己对比");
                return Ok(new ResponseError("请求参数有误，请验证"));
            }

            var ordering = orderTemp + sortByTemp;

            //未删除
            var predicate = PredicateBuilder.New<TaskEntity>(true).
                And(entity => entity.DeleteFlag == 0 && entity.Copy == 0);//未删除且非副本

            //
            if (!string.IsNullOrEmpty(taskTypeKey))
                predicate = predicate.And(entity => entity.TaskTypeKey.Equals(taskTypeKey));

            if (!string.IsNullOrEmpty(typeLevelKey))
                predicate = predicate.And(entity => entity.TypeLevelKey.Equals(typeLevelKey));

            if (!string.IsNullOrEmpty(levelKey))
                predicate = predicate.And(entity => entity.LevelKey.Equals(levelKey));

            if (!string.IsNullOrEmpty(keyword))
                predicate = predicate.And(entity => EF.Functions.Like(entity.TaskName, $"%{keyword}%"));

            if (tagId != 0)
                predicate = predicate.And(task => task.SubjectRefEntities.Any(x =>
                    x.Subject.TagRefEntities.FirstOrDefault(tag => tag.Tag.OriginalId.Equals(tagId)) != null));

            var tempCollection =
                await _unitOfWork.TaskRep.GetPageAsync(predicate, ordering, page, perPage, false);
            var pageData = _mapper.Map<PageData<TaskSmipleDto>>(tempCollection);

            return Ok(new ResponseInfo {Result = pageData});
        }

        /// <summary>
        /// 获取单个任务
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("singleTask")]
        public async Task<IActionResult> SingleTask(long id)
        {
            var result = await _unitOfWork.TaskRep.GetFullAsync(entity => entity.Id.Equals(id));
            if (result != null) return Ok(new ResponseInfo {Result = _mapper.Map<TaskFullDto>(result)});

            _logger.LogInformation(LogHelper.OutputClearness($"获取id{id}的任务失败,该任务不存在"));
            return Ok(new ResponseError($"获取id{id}的任务失败,该任务不存在"));
        }

        /// <summary>
        /// 创建任务
        /// </summary>
        /// <param name="taskInfo"></param>
        /// <returns></returns>
        [HttpPost("newTask")]
        public async Task<IActionResult> CreateTask([FromBody] TaskNewDto taskInfo)
        {
            if (taskInfo == null)
            {
                _logger.LogInformation(LogHelper.OutputClearness($"没有新建任务,请重新提交"));
                return Ok(new ResponseError($"没有新建任务,请重新提交"));
            }

            var newTask = _mapper.Map<TaskEntity>(taskInfo);
            if (taskInfo.SubjectRefEntities.Count != 0)
            {
                newTask.TagDisplay = GetTaskTagsDisplay(taskInfo.SubjectRefEntities).TrimEnd(',');
            }
            var result = await _unitOfWork.TaskRep.InsertAsync(newTask);

            #region ::::: 日志 :::::

            _mqClient.PushLogMessage(new SystemLogEntity(await _service.GetTokenInfo(), result, OptionType.Create)
                {LogDesc = $"创建训练任务[{newTask.TaskName}]{(result ? "成功" : "失败")}"});

            #endregion
            return Ok(result ? new ResponseInfo() : new ResponseError("新增任务失败"));
        }

        /// <summary>
        /// 编辑任务
        /// </summary>
        /// <param name="taskInfo"></param>
        /// <returns></returns>
        [HttpPut("taskEdit")]
        public async Task<IActionResult> EditTaskTest([FromBody] TaskUpdateDto taskInfo)
        {
            if (taskInfo == null)
            {
                _logger.LogInformation(LogHelper.OutputClearness($"没有提交任务,请重新提交"));
                return Ok(new ResponseError($"没有提交任务,请重新提交"));
            }

            var taskTemp = await _unitOfWork.TaskRep.GetFullAsync(x => x.Id.Equals(taskInfo.Id));
            if (taskTemp == null)
            {
                return Ok(new ResponseError("任务不存在"));
            }

            _mapper.Map(taskInfo, taskTemp);//更新任务字段

            #region ::::: 有删除项 :::::

            if (taskInfo.RemoveSubjects != null)
            {
                var tagList = new List<long>();
                foreach (var subjectId in taskInfo.RemoveSubjects)//删除科目
                {
                    var deleteTemp = await _unitOfWork.SubjectRep.GetAsync(x => x.Id.Equals(subjectId));
                    if (deleteTemp == null) continue;
                    tagList.AddRange(deleteTemp.TagRefEntities.Select(refEntity => refEntity.TagId));
                    await _unitOfWork.SubjectRep.DeleteAsync(deleteTemp);
                }

                foreach (var tagId in tagList)
                {
                    var tagTemp = await _unitOfWork.TagRep.GetAsync(x => x.Id.Equals(tagId));
                    if(tagTemp == null) continue;

                    var tagDeleteResult = await _unitOfWork.TagRep.DeleteAsync(tagTemp);
                    if (!tagDeleteResult) _logger.LogInformation(LogHelper.OutputClearness($"删除id为{tagId}的标签失败"));
                }
            }

            #endregion

            #region ::::: 有新增项 :::::

            if (taskInfo.NewSubjects != null && taskInfo.NewSubjects.Count != 0)
            {
                var subjectEntities = _mapper.Map<IList<SubjectEntity>>(taskInfo.NewSubjects);
                foreach (var subjectEntity in subjectEntities)
                {
                    await _unitOfWork.SubjectRep.InsertAsync(subjectEntity);
                    taskTemp.SubjectRefEntities.Add(new TaskSubjectRefEntity { Task = taskTemp, Subject = subjectEntity });
                }
            }

            #endregion

            var result = await _unitOfWork.TaskRep.UpdateAsync(taskTemp);

            #region ::::: 日志 :::::

            _mqClient.PushLogMessage(new SystemLogEntity(await _service.GetTokenInfo(), result)
                {LogDesc = $"编辑id[{taskTemp.Id}]的训练任务{(result ? "成功" : "失败")}"});

            #endregion
            return result ? Ok(new ResponseInfo()) : Ok(new ResponseError("更新失败！"));
        }

        /// <summary>
        /// 删除一个任务
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("singleTask")]
        public async Task<IActionResult> DeleteSingleTask(long id = 0)
        {
            if (id == 0) return Ok(new ResponseError("任务id不能为0"));

            var taskTemp = await _unitOfWork.TaskRep.GetAsync(entity => entity.Id.Equals(id));
            if (taskTemp == null) return Ok(new ResponseError($"id为{id}的训练任务不存在"));

            taskTemp.DeleteFlag = 1;
            var result = await _unitOfWork.TaskRep.UpdateAsync(taskTemp);

            #region ::::: 日志 :::::

            _mqClient.PushLogMessage(new SystemLogEntity(await _service.GetTokenInfo(), result, OptionType.Delete)
                {LogDesc = $"删除id为[{id}]训练任务{(result ? "成功" : "失败")}"});

            #endregion
            return Ok(result?new ResponseInfo() : new ResponseError($"id为{id}的训练任务删除失败"));
        }

        /// <summary>
        /// 删除多个任务
        /// </summary>
        /// <param name="idList"></param>
        /// <returns></returns>
        [HttpDelete("multiTasks")]
        public async Task<IActionResult> DeleteMultiTasks([FromBody] List<int> idList)
        {
            if (idList == null || idList.Count == 0) return Ok(new ResponseError("参数列表为空"));

            foreach (var id in idList)
            {
                var tempTask = await _unitOfWork.TaskRep.GetAsync(entity => entity.Id.Equals(id));
                if (tempTask == null) continue;
                tempTask.DeleteFlag = 1;
                await _unitOfWork.TaskRep.UpdateAsync(tempTask,false);
            }
            var result = await _unitOfWork.TaskRep.SaveChangesAsync();

            #region ::::: 日志 :::::

            _mqClient.PushLogMessage(new SystemLogEntity(await _service.GetTokenInfo(), result > 0, OptionType.Delete)
                {LogDesc = $"批量删除训练任务{(result > 0 ? "成功" : "失败")}"});

            #endregion
            return Ok(result > 0 ? new ResponseInfo() : new ResponseError("删除失败"));
        }

        /// <summary>
        /// 新增任务单
        /// 任务副本
        /// </summary>
        /// <param name="taskIds"></param>
        /// <returns></returns>
        [HttpPost("addMyTask")]
        public async Task<IActionResult> AddMyTask([FromBody]List<long> taskIds)
        {
            if (taskIds == null || taskIds.Count == 0) return Ok(new ResponseError("新增任务失败！请选择至少一条训练任务！"));
            List<TaskEntity> taskEntities = new List<TaskEntity>();
            string ids = string.Empty;
            foreach (var id in taskIds)
            {
                var taskEntity = await _unitOfWork.TaskRep.GetFullAsync(x => x.Id.Equals(id));
                if (taskEntity == null) continue;//任务不存在
                var copyTempDto = _mapper.Map<TaskNewDto>(taskEntity);
                var copyTempEntity = _mapper.Map<TaskEntity>(copyTempDto);
                copyTempEntity.Copy = Convert.ToInt32(taskEntity.Id);//设为副本,副本值为原始任务id
                if (copyTempEntity.SubjectRefEntities != null && copyTempEntity.SubjectRefEntities.Count != 0)
                {
                    foreach (var taskSubjectRefEntity in copyTempEntity.SubjectRefEntities)
                    {
                        var originalTemp = taskEntity.SubjectRefEntities.FirstOrDefault(e =>
                            e.Subject.OriginalId.Equals(taskSubjectRefEntity.Subject.OriginalId));
                        taskSubjectRefEntity.Subject.Copy =
                            originalTemp == null ? -1 : Convert.ToInt32(originalTemp.Subject.Id);//设为副本，副本值为原始科目id
                    }
                }
                var result = await _unitOfWork.TaskRep.InsertAsync(copyTempEntity);

                #region ::::: 日志 :::::

                _mqClient.PushLogMessage(new SystemLogEntity(await _service.GetTokenInfo(), result, OptionType.Create)
                    {LogDesc = $"新增训练任务副本{(result ? "成功" : "失败")}"});

                #endregion
                if (result){taskEntities.Add(copyTempEntity);}
                else{ids += taskEntity.Id.ToString(); }//复制失败
            }

            var results = _mapper.Map<List<TaskSmipleDto>>(taskEntities);
            return Ok(new ResponseInfo {Result = results });
        }
    }
}
