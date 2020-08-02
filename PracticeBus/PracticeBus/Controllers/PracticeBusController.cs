using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using ApiUtil;
using ApiUtil.Entities;
using ApiUtil.Log;
using ApiUtil.Mq;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PracticeBus.Entity;
using PracticeBus.ViewModel;
using PracticeBus.ViewModel.Report;
using PracticeBus.ViewModel.Subject;
using PracticeBus.ViewModel.SubjectScore;
using PracticeBus.ViewModel.SubjectStatistics;
using PracticeBus.ViewModel.Task;
using PracticeBus.ViewModel.TaskScore;
using PracticeBus.ViewModel.TaskStatistics;

namespace PracticeBus.Controllers
{
    [Route("practiceBus/v1")]
    public class PracticeBusController : Controller
    {
        private readonly IFreeSql _fSql;
        private readonly IMapper _mapper;
        private readonly ILogger<PracticeBusController> _logger;
        private readonly RabbitMqClient _mqClient;
        private readonly ServiceHelper _service;

        public PracticeBusController(IFreeSql fSql, IMapper mapper, ServiceHelper service,
            ILogger<PracticeBusController> logger, RabbitMqClient rabbitMq)
        {
            _fSql = fSql;
            _mapper = mapper;
            _logger = logger;
            _mqClient = rabbitMq;
            _service = service;
        }

        /// <summary>
        /// 更新成绩单
        /// </summary>
        /// <param name="planId"></param>
        /// <param name="taskId"></param>
        /// <returns></returns>
        private async Task<bool> UpdateTaskScore(long planId, long taskId)
        {
            #region ::::: 先清一遍数据 :::::

            var taskScoreList = await _fSql.Select<TTaskBusScore>()
                .Where(ts => ts.DeleteFlag == 0 && ts.PlanId.Equals(planId) && ts.TaskBusId.Equals(taskId))
                .ToListAsync();
            if (taskScoreList != null && taskScoreList.Count > 0)
            {
                var deleteResult = await _fSql.Update<TTaskBusScore>().SetSource(taskScoreList).Set(ts => ts.DeleteFlag, 1).ExecuteAffrowsAsync();
                //todo:日志：
                _logger.LogInformation(LogHelper.OutputClearness($"更新任务成绩表，先逻辑删除了{deleteResult}条记录"));
            }

            #endregion

            #region ::::: 如果培训计划中没有学员信息，则返回 :::::

            var studentIdList = await _fSql.Select<TTrainingplanStu>()
                .Where(s => s.DeleteFlag == 0 && s.TrainingplanId.Equals(planId))
                .ToListAsync();

            if (studentIdList == null || studentIdList.Count == 0) return true;

            #endregion

            #region ::::: 训练任务信息查找出来，找不到返回错误 :::::

            var taskEntity = await _fSql.Select<TTaskBus>()
                .Where(t => t.Id.Equals(taskId) && t.DeleteFlag == 0)
                .IncludeMany(t => t.Subjects).ToOneAsync();

            if (taskEntity == null)
            {
                _logger.LogInformation(LogHelper.OutputClearness($"id为{taskId}的任务不存在"));
                return false;
            }

            #endregion

            foreach (var stu in studentIdList)
            {
                //先插任务成绩表
                var tsId = await _fSql.Insert<TTaskBusScore>()
                    .AppendData(new TTaskBusScore { PlanId = planId, TaskBusId = taskId, UserId = stu.UserId??0, UserName = stu.UserName, Status = 1, Result = 1 })
                    .ExecuteIdentityAsync();
                //todo:日志：任务成绩表插数据
                _logger.LogInformation(LogHelper.OutputClearness($"新增id为{tsId}的任务成绩表"));
                //再插科目成绩表
                var subjectScoreList = new List<TSubjectBusScore>();
                foreach (var subject in taskEntity.Subjects)
                {
                    subjectScoreList.Add(new TSubjectBusScore
                    {
                        SubjectBusId = subject.Id,
                        OriginalId = subject.OriginalId,
                        TaskBusId = taskId,
                        TaskScoreId = tsId,
                        UserId = stu.Id,
                        Status = 1,
                        Result = 1
                    });
                }
                if (subjectScoreList.Count == 0) continue;
                var insertSubjectScoreResult =
                    await _fSql.Insert<TSubjectBusScore>().AppendData(subjectScoreList).ExecuteAffrowsAsync();
                //todo:日志：科目成绩表插数据
                _logger.LogInformation(LogHelper.OutputClearness($"id为{tsId}的任务成绩表下新增了{insertSubjectScoreResult}条关联的科目成绩表"));
            }

            return true;
        }

        #region ::::: 培训管理 :::::

        /// <summary>
        /// 培训计划添加任务
        /// </summary>
        /// <param name="taskInfo"></param>
        /// <returns></returns>
        [HttpPost("addTask")]
        public async Task<IActionResult> AddTask([FromBody] AddMyTaskParams taskInfo)
        {
            //todo:日志
            foreach (var dto in taskInfo.Tasks)
            {
                var taskEntity = _mapper.Map<TTaskBus>(dto);
                if (taskEntity == null) continue;
                //1.插task，存id
                var taskId = await _fSql.Insert<TTaskBus>().AppendData(taskEntity).ExecuteIdentityAsync();

                #region ::::: 插subject，存id :::::

                if (taskEntity.Subjects == null) continue;
                foreach (var subjectBus in taskEntity.Subjects)
                {
                    subjectBus.TaskBusId = taskId;
                    var subjectId = await _fSql.Insert<TSubjectBus>().AppendData(subjectBus).ExecuteIdentityAsync();
                    if(subjectBus.Tags == null) continue;
                    foreach (var tag in subjectBus.Tags)
                    {
                        //4.判断tag的id是否存在，id存在再判断名字是否一样
                        long tagId = 0;
                        var originalTag = await _fSql.Select<TKnowledgeTag>()
                            .Where(t => t.SrcId.Equals(tag.SrcId) && t.DeleteFlag == 0)
                            .FirstAsync();
                        if (originalTag == null || !originalTag.Tag.Equals(tag.Tag))
                            tagId = await _fSql.Insert<TKnowledgeTag>().AppendData(tag).ExecuteIdentityAsync();
                        else tagId = originalTag.Id;

                        //5.关联subject和tag
                        var refTempResult = await _fSql.Insert<TSubjectBusTagRef>()
                            .AppendData(new TSubjectBusTagRef{SubjectBus_id = subjectId,KnowledgeTag_id = tagId})
                            .ExecuteAffrowsAsync();
                    }
                }

                #endregion

                #region ::::: 插计划表 :::::

                var planCount = await _fSql.Select<TPlanCourseTaskExamRef>()
                    .Where(p => p.DeleteFlag == 0 && p.PlanId.Equals(taskInfo.PlanId)).CountAsync();
                var planInsertResult = await _fSql.Insert<TPlanCourseTaskExamRef>()
                    .AppendData(new TPlanCourseTaskExamRef
                    {
                        PlanId = taskInfo.PlanId,
                        ContentId = taskId,
                        ContentSort = (int)planCount + 1,
                        TeacherName = taskInfo.TeacherName,
                        TeacherNum = taskInfo.TeacherNum,
                        Dif = "2",
                        CreateBy = taskInfo.CreatorId,
                        DeleteFlag = 0,
                        FinishRate = 0,
                    }).ExecuteAffrowsAsync();
                _logger.LogInformation(LogHelper.OutputClearness("计划任务中间表插表"));

                #endregion

            }
            return Ok(new ResponseInfo());
        }

        /// <summary>
        /// 创建训练任务成绩表
        /// 培训计划开始后调用
        /// </summary>
        /// <param name="planId"></param>
        /// <returns></returns>
        [HttpGet("addTaskScore")]
        public async Task<IActionResult> AddTaskScore(long planId)
        {
            var taskIds = await _fSql.Select<TPlanCourseTaskExamRef>()
                .Where(e => e.PlanId.Equals(planId) && e.DeleteFlag == 0 && e.Dif.Equals("2"))
                .ToListAsync(e => e.ContentId);
            foreach (var taskId in taskIds)
            {
                if(taskId == null) continue;
                var tempResult = await UpdateTaskScore(planId, taskId.Value);//todo:日志：更新任务关联表
                if (tempResult)
                {
                    var taskTemp = await _fSql.Select<TTaskBus>(taskId.Value).ToOneAsync();
                    if(taskTemp == null) continue;
                    //todo:日志：任务统计表创建数据
                    await _fSql.Insert<TTaskBusStatistics>().AppendData(new TTaskBusStatistics
                    {
                        TaskOriginalId = taskTemp.OriginalId,
                        TaskBusId = taskTemp.Id,
                        TaskName = taskTemp.TaskName
                    }).ExecuteIdentityAsync();

                    var subjects = await _fSql.Select<TSubjectBus>().Where(s => s.TaskBusId.Equals(taskId.Value)).ToListAsync();
                    //todo:日志：科目统计表创建数据
                    foreach (var subject in subjects)
                    {
                        await _fSql.Insert<TSubjectBusStatistics>().AppendData(new TSubjectBusStatistics
                        {
                            SubjectOriginalId = subject.OriginalId,
                            SubjectBusId = subject.Id,
                            SubjectName = subject.Name
                        }).ExecuteIdentityAsync();
                    }
                }
            }
            return Ok(new ResponseInfo());
        }

        /// <summary>
        /// 删除训练任务
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("removeTask")]
        public async Task<IActionResult> RemoveTask(long id)
        {
            var result = await _fSql.Update<TTaskBus>(id).Set(tb => tb.DeleteFlag, 1).ExecuteAffrowsAsync();
            //TODO:日志
            return Ok(result > 0 ? new ResponseInfo() : new ResponseError("删除失败"));
        }

        /// <summary>
        /// 训练任务结果
        /// </summary>
        /// <param name="id">任务id</param>
        /// <returns></returns>
        [HttpGet("taskBus")]
        public async Task<IActionResult> TaskBus(long id)
        {
            var taskBus = await _fSql.Select<TTaskBus>()
                .Where(t => t.Id.Equals(id))
                .IncludeMany(t => t.Subjects)
                .WhereCascade(t => t.DeleteFlag == 0)
                .ToOneAsync();
            return Ok(taskBus != null ? 
                new ResponseInfo{Result = _mapper.Map<TaskQueryDto>(taskBus)} :
                new ResponseError("任务不存在"));
        }

        /// <summary>
        /// 训练统计--通过与完成结果
        /// </summary>
        /// <param name="id">任务id</param>
        /// <returns></returns>
        [HttpGet("taskScoreStatistics")]
        public async Task<IActionResult> TaskStatistics(long id)
        {
            var taskScores = await _fSql.Select<TTaskBusScore>()
                .Where(ts => ts.TaskBusId.Equals(id) && ts.DeleteFlag == 0)
                .ToListAsync();
            return Ok(new ResponseInfo
            {
                Result = new
                {
                    passNumb = taskScores?.Count > 0 ? taskScores.Count(ts=>ts.Result == 0) : 0,
                    finishNumb = taskScores?.Count > 0 ? taskScores.Count(ts=>ts.Status == 0) : 0
                }
            });
        }

        /// <summary>
        /// 训练统计--任务成绩单列表
        /// </summary>
        /// <param name="id">任务id</param>
        /// <param name="planId">训练计划id</param>
        /// <param name="keyword">关键字</param>
        /// <param name="result">0已完成|1未完成|-1未筛选</param>
        /// <param name="status">0已完成|1未完成|-1未筛选</param>
        /// <param name="page">页码</param>
        /// <param name="perPage">每页行数</param>
        /// <returns></returns>
        [HttpGet("taskScoreList")]
        public async Task<IActionResult> TaskScores(long id, long planId, string keyword, int result = -1,
            int status = -1, int page = 1, int perPage = 10)
        {
            var taskScores = await _fSql.Select<TTaskBusScore>()
                .Where(ts => ts.PlanId.Equals(planId) && ts.TaskBusId.Equals(id))
                .WhereIf(result != -1, ts => ts.Result.Equals((sbyte) result))
                .WhereIf(status != -1, ts => ts.Status.Equals((sbyte) status))
                .WhereIf(!string.IsNullOrEmpty(keyword),
                    ts => ts.UserName.Contains(keyword) || ts.Department.Contains(keyword))
                .WhereCascade(ts=>ts.DeleteFlag == 0)
                .Count(out var itemCount)
                .Page(page, perPage)
                .ToListAsync(ts => new TaskScoreQueryDto());

            return Ok(new ResponseInfo
                {Result = new PageData<TaskScoreQueryDto> {Rows = taskScores, Totals = itemCount}});

        }

        /// <summary>
        /// 单个学员任务统计
        /// </summary>
        /// <param name="id">任务成绩id</param>
        /// <param name="userId">用户id</param>
        /// <returns></returns>
        [HttpGet("singleTaskStatistics")]
        public async Task<IActionResult> MyTaskStatistics(long id, long userId)
        {
            var taskScore = await _fSql.Select<TTaskBusScore>()
                .Where(ts => ts.TaskBusId.Equals(id) && ts.UserId.Equals(userId))
                .Include(ts => ts.TaskBus)
                .IncludeMany(ts => ts.SubjectBusScores, then => then.Include(ss => ss.SubjectBus))
                .ToOneAsync();
            if (taskScore == null) return Ok(new ResponseError($"id为{id}，user为{userId}的训练任务单不存在"));
            var myTaskContentDto = _mapper.Map<BaseTaskDto>(taskScore.TaskBus);
            var subjectNumb = taskScore.SubjectBusScores.Count;

            #region ::::: 任务中没有科目 :::::

            if (subjectNumb == 0)
                return Ok(new ResponseInfo
                {
                    Result = new
                    {
                        taskContent = myTaskContentDto,
                        trainingResult = 1,
                        subjectNumb = 0,
                        passNumb = 0,
                        passPercent = 0,
                        finishNumb = 0,
                        finishPercent = 0
                    }
                });

            #endregion

            var passNumb = taskScore.SubjectBusScores.Count(s => s.Result == 0);
            var passPercent = Convert.ToInt32(passNumb * 1.0f / subjectNumb * 100);
            var finishNumb = taskScore.SubjectBusScores.Count(s => s.Status == 0);
            var finishPercent = Convert.ToInt32(finishNumb * 1.0f / subjectNumb * 100);

            return Ok(new ResponseInfo
            {
                Result = new
                {
                    taskContent = myTaskContentDto,
                    trainingResult = taskScore.Result,
                    subjectNumb = subjectNumb,
                    passNumb = passNumb,
                    passPercent = passPercent,
                    finishNumb = finishNumb,
                    finishPercent = finishPercent,
                    subjectScores = _mapper.Map<List<SubjectScoreQueryDto>>(taskScore.SubjectBusScores)
                }
            });
        }

        /// <summary>
        /// 科目详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("subjectInfo")]
        public async Task<IActionResult> SubjectInfo(long id)
        {
            var result = await _fSql.Select<TSubjectBus>()
                .Where(sb => sb.Id.Equals(id))
                .IncludeMany(sb=>sb.Tags)
                .ToOneAsync();
            return Ok(result == null ? new ResponseError("科目不存在") : new ResponseInfo {Result = _mapper.Map<SubjectQueryDto>(result)});
        }

        #endregion

        #region ::::: 设置结果 :::::

        /// <summary>
        /// 任务设置状态或结果值
        /// </summary>
        /// <param name="status"></param>
        /// <param name="result"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("taskScoreSetValue")]
        public async Task<IActionResult> TaskScoreSetValue(long id, int status = -1, int result = -1)
        {
            if (status == -1 && result == -1) return Ok(new ResponseError("任务状态或结果参数未填写"));
            var taskScore = await _fSql.Select<TTaskBusScore>()
                .Where(ts => ts.DeleteFlag == 0 && ts.Id.Equals(id)).ToOneAsync();
            if (taskScore == null) return Ok(new ResponseError("成绩单不存在"));
            var count = await _fSql.Update<TTaskBusScore>(id)
                .Set(ts => new TTaskBusScore {Status = (sbyte) status, Result = (sbyte) result}).ExecuteAffrowsAsync();
            return Ok(count>0?new ResponseInfo() : new ResponseError("更新失败"));
        }

        /// <summary>
        /// 科目设置状态或结果值
        /// </summary>
        /// <param name="id"></param>
        /// <param name="status"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        [HttpGet("subjectScoreSetValue")]
        public async Task<IActionResult> SubjectScoreSetValue(long id, int status = -1, int result = -1)
        {
            if (status == -1 && result == -1) return Ok(new ResponseError("科目状态或结果参数未填写"));
            var subjectScore = await _fSql.Select<TSubjectBusScore>()
                .Where(ss => ss.DeleteFlag == 0 && ss.Id.Equals(id)).ToOneAsync();
            if (subjectScore == null) return Ok(new ResponseError("成绩单不存在"));
            var count = await _fSql.Update<TSubjectBusScore>(id)
                .Set(ss => new TSubjectBusScore { Status = (sbyte) status, Result = (sbyte) result}).ExecuteAffrowsAsync();
            return Ok(count > 0 ? new ResponseInfo() : new ResponseError("更新失败"));
        }

        #endregion

        #region ::::: 任务统计 :::::

        /// <summary>
        /// 任务分析统计
        /// </summary>
        /// <returns></returns>
        [HttpGet("appraisal/taskStatistics")]
        public async Task<IActionResult> TaskAnalysisStatistics()
        {
            var taskNumb = await _fSql.Select<TTaskBusStatistics>().Where(ts => ts.DeleteFlag == 0).CountAsync();
            if (taskNumb == 0) return Ok(new ResponseInfo {Result = new { finish = 0.0, pass = 0.0, durationAvg = 0.0 } });

            var finish = await _fSql.Select<TTaskBusStatistics>().Where(t => t.DeleteFlag == 0).SumAsync(t => t.FinishPercent);
            var pass = await _fSql.Select<TTaskBusStatistics>().Where(t => t.DeleteFlag == 0).SumAsync(t => t.PassPercent);
            var duration = await _fSql.Select<TTaskBusStatistics>().Where(t => t.DeleteFlag == 0).SumAsync(t => t.DurationAvg);

            return Ok(new ResponseInfo
            {
                Result = new
                {
                    finish = finish / taskNumb,
                    pass = pass / taskNumb,
                    durationAvg = duration / taskNumb
                }
            });
        }

        /// <summary>
        /// 任务分析详情
        /// </summary>
        /// <param name="page"></param>
        /// <param name="perPage"></param>
        /// <param name="sortBy">status/result</param>
        /// <param name="order">asc/desc</param>
        /// <returns></returns>
        [HttpGet("appraisal/taskDetail")]//taskAnalysisDetail
        public async Task<IActionResult> TaskAnalysisDetail(int page = 1, int perPage = 10, string sortBy = "status",
            string order = "desc")
        {
            var result = await _fSql.Select<TTaskBusStatistics>()
                .Where(tbs => tbs.DeleteFlag == 0)
                .OrderByDescending(sortBy.Equals("status") && order.Equals("desc"), e => e.FinishPercent)
                .OrderByDescending(sortBy.Equals("result") && order.Equals("desc"), e => e.PassPercent)
                .OrderBy(sortBy.Equals("status") && order.Equals("asc"), e => e.FinishPercent)
                .OrderBy(sortBy.Equals("result") && order.Equals("asc"), e => e.PassPercent)
                .Count(out var itemCount)
                .Page(page, perPage)
                .ToListAsync<TaskBusStatisticsQueryDto>();

            return Ok(new ResponseInfo
                {Result = new PageData<TaskBusStatisticsQueryDto> {Rows = result, Totals = itemCount}});
        }

        /// <summary>
        /// 科目分析统计
        /// </summary>
        /// <returns></returns>
        [HttpGet("appraisal/subjectStatistics")]
        public async Task<IActionResult> SubjectAnalysisStatistics()
        {
            var subjectNumb = await _fSql.Select<TSubjectBusStatistics>().Where(ss => ss.DeleteFlag == 0).CountAsync();
            if (subjectNumb == 0) return Ok(new ResponseInfo {Result = new {finish = 0.0, pass = 0.0}});

            var finish = await _fSql.Select<TSubjectBusStatistics>().Where(t => t.DeleteFlag == 0).SumAsync(s => s.FinishPercent);
            var pass = await _fSql.Select<TSubjectBusStatistics>().Where(t => t.DeleteFlag == 0).SumAsync(s => s.PassPercent);

            return Ok(new ResponseInfo
            {
                Result = new
                {
                    finish = finish / subjectNumb,
                    pass = pass / subjectNumb
                }
            });
        }

        /// <summary>
        /// 科目分析详情
        /// </summary>
        /// <param name="page"></param>
        /// <param name="perPage"></param>
        /// <returns></returns>
        [HttpGet("appraisal/subjectDetail")]//subjectAnalysisDetail
        public async Task<IActionResult> SubjectAnalysisDetail(int page = 1, int perPage = 10)
        {
            var result = await _fSql.Select<TSubjectBusStatistics>()
                .Where(sbs => sbs.DeleteFlag == 0)
                .Count(out var itemCount)
                .ToListAsync<SubjectBusStatisticsQueryDto>();
            return Ok(new ResponseInfo{Result = new PageData<SubjectBusStatisticsQueryDto>{Rows = result, Totals = itemCount}});
        }

        #endregion

        #region ::::: 报告 :::::

        /// <summary>
        /// 个人训练任务报告信息
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="planId"></param>
        /// <returns></returns>
        [HttpGet("report/personal")]
        public async Task<IActionResult> PersonalReportInfo(long userId, long planId)
        {
            #region ::::: 当前用户在这个计划下的信息 :::::
            //用户id在planId培训计划下的所有成绩单
            var temp = await _fSql.Select<TTaskBusScore>()
                .Where(ts => ts.PlanId.Equals(planId) && ts.UserId.Equals(userId))
                .IncludeMany(ts => ts.SubjectBusScores)
                .WhereCascade(t=>t.DeleteFlag == 0)
                .ToListAsync();
            if (temp == null || temp.Count == 0) return Ok(new ResponseInfo());

            #endregion

            #region ::::: 所有与这个计划相关的训练任务信息 :::::
            //planId培训计划下所有的成绩单
            var allTemps = await _fSql.Select<TTaskBusScore>()
                .Where(ts => ts.PlanId.Equals(planId) && ts.DeleteFlag == 0)
                .IncludeMany(ts => ts.SubjectBusScores)
                .ToListAsync();
            if(allTemps == null || allTemps.Count == 0) return Ok(new ResponseError($"不存在planId为{planId}的训练内容"));
            var userGroupTemps = allTemps.GroupBy(e => e.UserId);//按学员分组

            #endregion

            var personalResult = new PersonalResult();//最终返回给请求方的对象
            var personalInfos = new List<PersonalInfo>();

            var totalSubjectNumb = allTemps.Sum(t => t.SubjectBusScores.Count);
            personalResult.TotalSubjectNumb = totalSubjectNumb.ToString();//本次培训科目总数
            var finishSubjectNumb = allTemps.Sum(t => t.SubjectBusScores.Count(s => s.Status == 0));
            personalResult.FinishSubjectNumb = finishSubjectNumb.ToString();//本次培训学员完成科目数量

            #region ::::: 每个用户的练习时长、完成率、通过率 :::::

            foreach (var userGroupTemp in userGroupTemps)
            {
                var personInfo = new PersonalInfo{UserId = userGroupTemp.Key, Duration = userGroupTemp.Sum(e => e.Duration)};
                var subjectCount = userGroupTemp.Sum(e => e.SubjectBusScores.Count);
                personInfo.FinishPercent = subjectCount == 0
                    ? 0
                    : userGroupTemp.Sum(e => e.SubjectBusScores.Count(s => s.Status == 0)) * 1.0f / subjectCount * 100;

                personInfo.PassPercent = subjectCount == 0
                    ? 0
                    : userGroupTemp.Sum(e => e.SubjectBusScores.Count(s => s.Result == 0)) * 1.0f / subjectCount * 100;
                personalInfos.Add(personInfo);
            }

            #endregion

            PersonalInfo ownInfo = null;//请求中userId的用户信息
            int rank = 0;//个人在不同条件下的排名
            int rankResultTemp = 0;//达不达标的临时变量
            float rankResult = 0;//最终的值大于0即达标
            int behindItem = 0;//不同条件下，在本人后面的人数

            #region ::::: 训练时长排名 :::::

            //按学习时长排序，考虑并列
            var infoSort = personalInfos.AsEnumerable().GroupBy(info => info.Duration).OrderBy(infos => infos.Key).ToList();
            foreach (IGrouping<float, PersonalInfo> group in infoSort)
            {
                rank++;//按组排名
                foreach (var personalInfo in group)
                {
                    if (!personalInfo.UserId.Equals(userId)) continue;
                    if (ownInfo == null) ownInfo = personalInfo;
                    //学习时长
                    personalResult.TaskTrainSumTime = ownInfo.Duration.ToString("0.00");
                    //排名
                    personalResult.TaskTrainTimeRank = rank.ToString();
                }
            }

            if (infoSort.Count <= 1)//就一个排名，说明大家时长是一样的，那就没有先后了
            {
                personalResult.TaskTrainTimeExceedPercent = "0";
                personalResult.TaskTrainAvgLearningTime = ownInfo.Duration.ToString(CultureInfo.InvariantCulture);
                personalResult.TaskTrainLevelFlag = "超出";
                personalResult.TaskTrainDifHours = "0";
                rankResult = 0;
            }
            else
            {
                for (int index = rank; index < infoSort.Count; index++)
                {
                    behindItem += infoSort[index].Count();
                }
                //超过%
                personalResult.TaskTrainTimeExceedPercent = (behindItem * 1.0f / personalInfos.Count * 100).ToString(CultureInfo.InvariantCulture);
                //平均学习时长
                var avgDuration = personalInfos.Average(info => info.Duration);
                //超出/低于
                personalResult.TaskTrainLevelFlag = ownInfo.Duration - avgDuration >= 0 ? "超出" : "低于";
                //超出多少
                personalResult.TaskTrainDifHours = (ownInfo.Duration - avgDuration).ToString(CultureInfo.InvariantCulture);

                foreach (var group in infoSort)
                {
                    rankResultTemp++;
                    if (Math.Abs(avgDuration - @group.Key) < float.Epsilon) rankResult = rankResultTemp - rank;
                    else if (avgDuration > group.Key && avgDuration < infoSort[rankResultTemp].Key)
                    {
                        rankResult = rankResultTemp + 0.5f - rank;
                    }
                }
            }

            #endregion


            #region ::::: 训练完成率排名 :::::
            //初始化变量
            rank = 0;
            rankResultTemp = 0;
            behindItem = 0;

            //按完成率排序，考虑并列
            infoSort = personalInfos.AsEnumerable().GroupBy(info => info.FinishPercent).OrderBy(infos => infos.Key).ToList();
            foreach (IGrouping<float, PersonalInfo> group in infoSort)
            {
                rank++;
                foreach (var personalInfo in group)
                {
                    if (!personalInfo.UserId.Equals(userId)) continue;
                    //完成率
                    personalResult.TaskFinishRate = ownInfo.FinishPercent.ToString("0.00");
                    //排名
                    personalResult.TaskFinishRateRank = rank.ToString();
                    break;
                }
            }
            if (infoSort.Count <= 1)//就一个排名，说明大家时长是一样的，那就没有先后了
            {
                personalResult.TaskFinishRateExceedPercent = "0";
                personalResult.TaskFinishAvgRate = ownInfo.FinishPercent.ToString(CultureInfo.InvariantCulture);
                personalResult.TaskFinishRateFlag = "超出";
                personalResult.TaskFinishDifRate = "0";
                rankResult += 0;
            }
            else
            {
                for (int index = rank; index < infoSort.Count; index++)
                {
                    behindItem += infoSort[index].Count();
                }
                //超过%
                personalResult.TaskFinishRateExceedPercent = (behindItem * 1.0f / personalInfos.Count * 100).ToString(CultureInfo.InvariantCulture);
                //平均完成率
                var avgFinish = personalInfos.Average(info => info.FinishPercent);
                personalResult.TaskFinishAvgRate = avgFinish.ToString(CultureInfo.InvariantCulture);
                //超出/低于
                personalResult.TaskFinishRateFlag = ownInfo.FinishPercent - avgFinish >= 0 ? "超出" : "低于";
                //超出多少
                personalResult.TaskFinishDifRate = (ownInfo.FinishPercent - avgFinish).ToString(CultureInfo.InvariantCulture);

                foreach (var group in infoSort)
                {
                    rankResultTemp++;
                    if (Math.Abs(avgFinish - @group.Key) < float.Epsilon) rankResult += rankResultTemp - rank;
                    else if (avgFinish > group.Key && avgFinish < infoSort[rankResultTemp].Key)
                    {
                        rankResult += rankResultTemp + 0.5f - rank;
                    }
                }
            }
            #endregion

            #region ::::: 训练通过率排名 :::::
            //初始化变量
            rank = 0;
            rankResultTemp = 0;
            behindItem = 0;

            //按通过率排序，考虑并列
            infoSort = personalInfos.AsEnumerable().GroupBy(info => info.PassPercent).OrderBy(infos => infos.Key).ToList();
            foreach (IGrouping<float, PersonalInfo> group in infoSort)
            {
                rank++;
                foreach (var personalInfo in group)
                {
                    if (!personalInfo.UserId.Equals(userId)) continue;
                    //通过率
                    personalResult.TaskPassRate = ownInfo.FinishPercent.ToString("0.00");
                    //排名
                    personalResult.TaskPassRateRank = rank.ToString();
                    break;
                }
            }
            if (infoSort.Count <= 1)//就一个排名，说明大家时长是一样的，那就没有先后了
            {
                personalResult.TaskPassRatePercent = "0";
                personalResult.TaskPassAvgRate = ownInfo.PassPercent.ToString(CultureInfo.InvariantCulture);
                personalResult.TaskPassRateFlag = "超出";
                personalResult.TaskPassDifRate = "0";
                rankResult += 0;
            }
            else
            {
                for (int index = rank; index < infoSort.Count; index++)
                {
                    behindItem += infoSort[index].Count();
                }
                //超过%
                personalResult.TaskPassRatePercent = (behindItem * 1.0f / personalInfos.Count * 100).ToString(CultureInfo.InvariantCulture);
                //平均通过率
                var avgPass = personalInfos.Average(info => info.PassPercent);
                personalResult.TaskPassAvgRate = avgPass.ToString(CultureInfo.InvariantCulture);
                //超出/低于
                personalResult.TaskPassRateFlag = ownInfo.PassPercent - avgPass >= 0 ? "超出" : "低于";
                //超出多少
                personalResult.TaskPassDifRate = (ownInfo.PassPercent - avgPass).ToString(CultureInfo.InvariantCulture);

                foreach (var group in infoSort)
                {
                    rankResultTemp++;
                    if (Math.Abs(avgPass - @group.Key) < float.Epsilon) rankResult += rankResultTemp - rank;
                    else if (avgPass > group.Key && avgPass < infoSort[rankResultTemp].Key)
                    {
                        rankResult += rankResultTemp + 0.5f - rank;
                    }
                }
            }
            #endregion

            personalResult.TaskGlobalResult = rankResult >= 0 ? "0" : "1";

            return Ok(new ResponseInfo {Result = personalInfos});
        }

        /// <summary>
        /// 单个任务的报告信息
        /// </summary>
        /// <param name="planId"></param>
        /// <returns></returns>
        [HttpGet("report/collective")]
        public async Task<IActionResult> CollectiveReportInfo(long planId)
        {
            var result = new CollectiveResult();

            var anyData = await _fSql.Select<TTaskBusStatistics>().Where(tbs => tbs.PlanId.Equals(planId) && tbs.DeleteFlag == 0).CountAsync();
            if (anyData == 0) return Ok(new ResponseInfo());//没有本次培训计划的统计数据，培训计划可能还没有结束

            //本次培训计划参与人数
            var userNumb = await _fSql.Select<TTrainingplanStu>()
                .Where(t => t.TrainingplanId.Equals(planId) && t.DeleteFlag == 0).CountAsync();

            #region ::::: 练习时长 :::::

            //----------本次培训参加模拟练习学员共__人
            result.AttendNumb = userNumb.ToString();

            var allUserOfPlanNumb = await _fSql.Select<TTrainingplanStu>().Where(t => t.DeleteFlag == 0).CountAsync();
            var allPlanNumb = await _fSql.Select<TTrainingplanStu>().Where(t => t.DeleteFlag == 0)
                .GroupBy(t => t.TrainingplanId).CountAsync();
            var avgUserOfPlan = allUserOfPlanNumb / allPlanNumb;
            //----------相比所有培训平均人次__
            result.AttendFlag = userNumb >= avgUserOfPlan ? "超出" : "低于";
            //----------__人
            result.AttendNumbDif = Math.Abs(userNumb - avgUserOfPlan).ToString();

            var taskStats = await _fSql.Select<TTaskBusStatistics>().Where(t => t.DeleteFlag == 0 && t.PlanId.Equals(planId)).ToListAsync();
            //----------所有学员在本次培训中，模拟练习时长共有__小时
            var durationTotal = taskStats.Sum(t => t.DurationTotal);
            result.TotalDuration = durationTotal.ToString("0.0");
            //----------本次培训的所有学员平均练习时长是__小时
            var avgDuration = taskStats.Average(t => t.DurationAvg);
            result.AvgDuration = avgDuration.ToString("0.0");

            var allAvgDuration = await _fSql.Select<TTaskBusStatistics>().Where(t => t.DeleteFlag == 0).AvgAsync(t => t.DurationAvg);
            //----------所有培训平均练习时长
            result.TotalAvgDuration = allAvgDuration.ToString("0.0");
            //----------相比所有培训平均练习时长__
            result.AvgDurationFlag = (double) avgDuration - allAvgDuration >= 0 ? "超出" : "低于";
            //----------__小时
            result.AvgDurationDif = Math.Abs((double) avgDuration - allAvgDuration).ToString("0.0");

            #endregion

            #region ::::: 完成率 :::::

            //----------本次培训模拟练习共有__门课程
            var subjectStats = await _fSql.Select<TSubjectBusStatistics>()
                .Where(s => s.DeleteFlag == 0 && s.PlanId.Equals(planId)).ToListAsync();
            result.TotalSubjectNumb = subjectStats.Count.ToString();
            //----------学员们完成了__门课程
            var subFinish = await _fSql.Select<TSubjectBusScore>()
                .Where(s => s.DeleteFlag == 0 && s.PlanId.Equals(planId) && s.Status == 0).CountAsync();
            result.FinishSubjectNumb = subFinish.ToString();
            //----------平均完成率为__%
            var subAvgFinish = subFinish * 1.0d / subjectStats.Count * 100;
            result.AvgFinishSubject = subAvgFinish.ToString("0.0");
            //----------所有培训平均完成率
            var allSubAvgFinish = await _fSql.Select<TSubjectBusStatistics>()
                .Where(s => s.DeleteFlag == 0).AvgAsync(s => s.FinishPercent);
            result.TotalAvgFinishSubject = allSubAvgFinish.ToString("0.00");
            //----------相比所有培训平均完成率__
            result.AvgFinishSubjectFlag = subAvgFinish - allSubAvgFinish >= 0 ? "超出" : "低于";
            //----------__%
            result.AvgFinishSubjectDif = Math.Abs(subAvgFinish - allSubAvgFinish).ToString("0.0");

            #endregion

            #region ::::: 通过率 :::::

            //学员们完成了__门科目
            var subPass = await _fSql.Select<TSubjectBusScore>()
                .Where(s => s.DeleteFlag == 0 && s.PlanId.Equals(planId) && s.Result == 0).CountAsync();
            result.PassSubjectNumb = subPass.ToString();
            //平均通过率为__%
            var subAvgPass = subPass * 1.0d / subjectStats.Count * 100;
            result.AvgPassSubject = subAvgPass.ToString("0.0");
            //所有培训平均通过率
            var allSubAvgPass = await _fSql.Select<TSubjectBusStatistics>()
                .Where(s => s.DeleteFlag == 0).AvgAsync(s => s.PassPercent);
            result.TotalAvgPassSubject = allSubAvgPass.ToString("0.0");
            //相比所有培训平均通过率__
            result.AvgPassSubjectFlag = subAvgPass - allSubAvgPass >= 0 ? "超出" : "低于";
            //__%
            result.AvgPassSubjectDif = Math.Abs(subAvgPass - allSubAvgPass).ToString("0.0");

            #endregion

            return Ok(new ResponseInfo {Result = result});
        }

            #endregion

        #region ::::: 服务间调度 :::::

        /// <summary>
        /// 服务间调度
        /// </summary>
        /// <returns></returns>
        [HttpPut("service/updatePersonInfo")]//updatePersonOfTask
        public async Task<IActionResult> UpdatePersonOfTask([FromBody] MyTaskPersonUpdateDto personInfos)
        {
            if (personInfos?.TaskId == null) return BadRequest();
            foreach (var taskId in personInfos.TaskId)
            {
                var result = await UpdateTaskScore(personInfos.PlanId, taskId);
                if(!result)_logger.LogInformation(LogHelper.OutputClearness($"更新PlanId[{personInfos.PlanId}],TaskBusId[{taskId}]失败"));
            }
            return NoContent();
        }


        /// <summary>
        /// 训练任务完成情况
        /// </summary>
        /// <param name="planId">计划id</param>
        /// <param name="taskId">任务id</param>
        /// <param name="userId">人员id</param>
        /// <returns></returns>
        [HttpGet("service/singleTaskStatus")]//singleTaskStatus
        public async Task<IActionResult> SingleTaskStatus(long planId, long taskId, long userId)
        {
            var taskScore = await _fSql.Select<TTaskBusScore>()
                .Where(ts => ts.PlanId.Equals(planId))
                .Where(ts => ts.UserId.Equals(userId))
                .Where(ts => ts.TaskBusId.Equals(taskId))
                .ToOneAsync();
            if (taskScore == null) return NoContent();

            return Ok(taskScore.Status);
        }

        #endregion
    }
}
