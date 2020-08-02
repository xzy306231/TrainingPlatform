using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using ApiUtil.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrainingTask.Api.ViewModel.Ref;
using TrainingTask.Api.ViewModel.Report;
using TrainingTask.Api.ViewModel.SubjectScore;
using TrainingTask.Api.ViewModel.TaskScore;
using TrainingTask.Core.Entity;

namespace TrainingTask.Api.Controllers
{
    public partial class TaskManageController
    {

        /// <summary>
        /// 服务间调度
        /// </summary>
        /// <param name="personInfos"></param>
        /// <returns></returns>
        [HttpPut("updatePersonOfTask")]
        public async Task<IActionResult> UpdatePersonOfTask([FromBody] MyTaskPersonUpdateDto personInfos)
        {
            foreach (var taskId in personInfos.TaskId)
            {
                //1.获取任务副本
                var taskEntityTemp = await _unitOfWork.TaskRep.GetIncludeSubjectsAsync(x => x.Id.Equals(taskId));
                if (taskEntityTemp == null)
                {
                    //TODO:日志
                    continue;
                }

                //TODO:日志

                //3.删除人员信息
                if (personInfos.RemoveUsers != null && personInfos.RemoveUsers.Count != 0)
                {
                    foreach (var user in personInfos.RemoveUsers)
                    {
                        var deleteTemp = await _unitOfWork.TaskScoreRep.GetAsync(entity =>
                            entity.UserId.Equals(user.UserId) &&
                            entity.PlanId.Equals(personInfos.PlanId) &&
                            entity.TaskId.Equals(taskId));
                        if (deleteTemp != null)
                            await _unitOfWork.TaskScoreRep.DeleteAsync(deleteTemp);
                    }
                }

                //5.新增关系
                if (personInfos.NewUsers == null || personInfos.NewUsers.Count == 0) continue;

                foreach (var user in personInfos.NewUsers)
                {
                    var taskScoreDto = _mapper.Map<TaskScoreCreateDto>(taskEntityTemp);
                    taskScoreDto.UserId = user.UserId;
                    taskScoreDto.UserName = user.UserName;
                    taskScoreDto.Department = user.Department;
                    taskScoreDto.PlanId = personInfos.PlanId;
                    taskScoreDto.TaskId = taskId;

                    foreach (var refEntity in taskEntityTemp.SubjectRefEntities)
                    {
                        var subjectDtoTemp = _mapper.Map<SubjectScoreCreateDto>(refEntity.Subject);
                        taskScoreDto.SubjectScores.Add(subjectDtoTemp);
                    }
                    var taskScoreEntity = _mapper.Map<TaskScoreEntity>(taskScoreDto);
                    var result = await _unitOfWork.TaskScoreRep.InsertAsync(taskScoreEntity);
                    //TODO:日志
                }

            }
            return Ok(new ResponseInfo());
        }

        /// <summary>
        /// 人员任务完成结果
        /// </summary>
        /// <param name="planId"></param>
        /// <param name="taskId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet("PersonTaskResult")]
        public async Task<IActionResult> PersonTaskResult(long planId, long taskId, long userId)
        {
            var resultTemp = await _unitOfWork.TaskScoreRep.GetAsync(entity =>
                entity.PlanId.Equals(planId) && entity.TaskId.Equals(taskId) && entity.UserId.Equals(userId));
            return resultTemp == null ? Ok("1") : (resultTemp.Result == 0 ? Ok("0") : Ok("1"));
        }

        /// <summary>
        /// 个人训练任务报告信息
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="planId"></param>
        /// <returns></returns>
        [HttpGet("PersonalReportInfo")]
        public async Task<IActionResult> PersonalReportInfo(long userId, long planId)
        {
            #region ::::: 当前用户在这个计划下的信息 :::::

            var temp = await _unitOfWork.TaskScoreRep.GetListIncludeSubjectAsync(e =>
                e.DeleteFlag == 0 && e.PlanId.Equals(planId) && e.UserId.Equals(userId));
            if (temp == null || temp.Count == 0)
                //return Ok(new ResponseError($"不存在planId为{planId}，userId为{userId}的训练内容"));
                return Ok(new ResponseInfo());

            #endregion

            #region ::::: 所有与这个计划相关的训练任务信息 :::::

            var allTemps = await _unitOfWork.TaskScoreRep.GetListIncludeSubjectAsync(e => e.DeleteFlag == 0 && e.PlanId.Equals(planId));
            if (allTemps == null || allTemps.Count == 0) return Ok(new ResponseError($"不存在planId为{planId}的训练内容"));

            var userGroupTemps = allTemps.GroupBy(entity => entity.UserId);

            #endregion

            var personalResult = new PersonalResult(); //最终返回给请求方的对象
            var personalInfos = new List<PersonalInfo>();

            //var taskScores = await _unitOfWork.TaskScoreRep.GetListIncludeSubjectAsync(e => e.DeleteFlag == 0 && e.PlanId.Equals(planId));
            var totalSubjectNumb = allTemps.Sum(t => t.SubjectScores.Count);
            personalResult.TotalSubjectNumb = totalSubjectNumb.ToString();//本次培训科目总数
            var finishSubjectNumb = allTemps.Sum(t => t.SubjectScores.Count(s => s.Status == 0));
            personalResult.FinishSubjectNumb = finishSubjectNumb.ToString();//本次培训学员完成科目数

            #region ::::: 每个用户的练习时长、完成率、通过率 :::::

            foreach (var userGroupTemp in userGroupTemps)
            {
                var personalInfo = new PersonalInfo
                {
                    UserId = userGroupTemp.Key, Duration = userGroupTemp.Sum(e => e.Duration)
                };
                var subjectCount = userGroupTemp.Sum(e => e.SubjectScores.Count);
                personalInfo.FinishPercent =
                    subjectCount == 0
                        ? 0
                        : userGroupTemp.Sum(e => e.SubjectScores.Count(s => s.Status == 0)) * 1.0f / subjectCount * 100;
                personalInfo.PassPercent =
                    subjectCount == 0
                        ? 0
                        : userGroupTemp.Sum(e => e.SubjectScores.Count(s => s.Result == 0)) * 1.0f / subjectCount * 100;
                personalInfos.Add(personalInfo);
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
                    if(ownInfo == null) ownInfo = personalInfo;
                    //学习时长
                    personalResult.TaskTrainSumTime = ownInfo.Duration.ToString("0.00");
                    //排名
                    personalResult.TaskTrainTimeRank = rank.ToString();
                    break;
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
                personalResult.TaskTrainAvgLearningTime = avgDuration.ToString(CultureInfo.InvariantCulture);
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

            return Ok(new ResponseInfo{Result = personalResult});
        }

        /// <summary>
        /// 单个任务的报告信息
        /// </summary>
        /// <returns></returns>
        [HttpGet("CollectiveReportInfo")]
        public async Task<IActionResult> CollectiveReportInfo(long planId)
        {
            var result = new CollectiveResult();
            var taskScores = await _unitOfWork.TaskScoreRep.GetListIncludeSubjectAsync(e => e.DeleteFlag == 0 && e.PlanId.Equals(planId));
            var taskScoresFinish = taskScores.Where(e=>e.Status == 0).ToList();
            var userNumb = taskScoresFinish.GroupBy(t => t.UserId).Count();
            if (taskScoresFinish.Count == 0) return Ok(new ResponseInfo());
            var attendNumb = taskScoresFinish.GroupBy(e => e.UserId).Count();//本次培训参加模拟练习学员人数
            result.AttendNumb = attendNumb.ToString();

            var allPlanNumb = await _unitOfWork.TaskScoreRep.Entities.Where(e=>e.DeleteFlag == 0).GroupBy(e => e.PlanId).CountAsync();
            var allAttendNumb = await _unitOfWork.TaskScoreRep.Entities.Where(e => e.DeleteFlag == 0 && e.Status == 0).GroupBy(e => new {e.PlanId, e.UserId}).CountAsync();
            result.AttendFlag = attendNumb - allAttendNumb / allPlanNumb >= 0 ? "超出" : "低于";//相比所有培训平均人次超出/低于
            result.AttendNumbDif = Math.Abs(attendNumb - allAttendNumb / allPlanNumb).ToString();//相比所有培训平均人次超出/低于的人数
            var totalDuration = taskScoresFinish.Sum(t => t.Duration);
            result.TotalDuration = totalDuration.ToString(CultureInfo.InvariantCulture);//本次培训模拟练习总时长
            result.AvgDuration = (totalDuration / userNumb).ToString("0.00");//本次培训的所有学员平均时长
            var tempDuration = await _unitOfWork.TaskScoreRep.Entities.Where(e => e.DeleteFlag == 0 && e.Status == 0).SumAsync(e => e.Duration);
            var avgDuration = totalDuration / userNumb - tempDuration / allAttendNumb;
            result.TotalAvgDuration = (tempDuration / allAttendNumb).ToString("0.00");//所有培训平均练习时长
            result.AvgDurationFlag = avgDuration >= 0 ? "超出" : "低于";//超出/低于平均的时长标志
            result.AvgDurationDif = Math.Abs(avgDuration).ToString("0.00");//超出/低于平均的时长数

            var totalSubjectNumb = taskScores.Sum(t => t.SubjectScores.Count);
            result.TotalSubjectNumb = totalSubjectNumb.ToString();//本次培训科目总数
            var finishSubjectNumb = taskScores.Sum(t => t.SubjectScores.Count(s => s.Status == 0));
            result.FinishSubjectNumb = finishSubjectNumb.ToString();//本次培训学员完成科目数
            var avgFinishSubject = finishSubjectNumb * 1.0f / totalSubjectNumb * 100;
            result.AvgFinishSubject = avgFinishSubject.ToString("0.00");//本次培训科目平均完成率
            var avgAllFinishSubject = await _unitOfWork.SubjectRep.Entities.Where(e => e.DeleteFlag == 0).AverageAsync(e => e.FinishPercent);
            var avgFinishSubjectDif = avgFinishSubject - avgAllFinishSubject;
            result.TotalAvgFinishSubject = avgAllFinishSubject.ToString("0.00");//所有培训平均通过率
            result.AvgFinishSubjectFlag = avgFinishSubjectDif >= 0 ? "超出" : "低于";//相比所有培训平均完成率
            result.AvgFinishSubjectDif = Math.Abs(avgFinishSubjectDif).ToString("0.00");//超出/低于数

            var passSubjectNumb = taskScores.Sum(t => t.SubjectScores.Count(s => s.Result == 0));
            result.PassSubjectNumb = passSubjectNumb.ToString();//本次培训学员通过科目数
            var avgPassSubject = passSubjectNumb * 1.0f / totalSubjectNumb * 100;
            result.AvgPassSubject = avgPassSubject.ToString("0.00");//本次培训学员平均通过率
            var avgAllPassSubject = await _unitOfWork.SubjectRep.Entities.Where(e => e.DeleteFlag == 0).AverageAsync(e => e.PassPercent);
            var avgPassSubjectDif = avgPassSubject - avgAllPassSubject;
            result.TotalAvgPassSubject = avgAllPassSubject.ToString("0.00");//所有培训平均通过率
            result.AvgPassSubjectFlag = avgPassSubjectDif >= 0 ? "超出" : "低于";//相比所有培训平均通过率
            result.AvgPassSubjectDif = Math.Abs(avgPassSubjectDif).ToString("0.00");//超出/低于数

            result.TaskGlobalResult = "1";//TODO:先写死，后面再改

            return Ok(new ResponseInfo{Result = result});
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="subjectRefs"></param>
        /// <returns></returns>
        private string GetTaskTagsDisplay(IList<TaskSubjectRefNewDto> subjectRefs)
        {
            var result = string.Empty;
            var tagDict = new Dictionary<long, string>();
            foreach (var taskSubjectRef in subjectRefs)
            {
                foreach (var subjectTagRef in taskSubjectRef.Subject.TagRefEntities)
                {
                    if (tagDict.ContainsKey(subjectTagRef.Tag.OriginalId)) continue;
                    tagDict.Add(subjectTagRef.Tag.OriginalId, subjectTagRef.Tag.TagName);
                }
            }
            var tagList = tagDict.Values.ToList().Distinct();
            return tagList.Aggregate(result, (current, temp) => current + $"{temp},");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="taskId"></param>
        /// <returns></returns>
        private async Task<bool> UpdateTaskStatistics(long taskId)
        {
            var taskScoreTemp = await _unitOfWork.TaskRep.GetAsync(taskId);
            if (taskScoreTemp == null) return false;

            var originalTaskTemp = await _unitOfWork.TaskRep.GetAsync(entity => entity.Id.Equals(taskScoreTemp.Copy));
            if (originalTaskTemp == null) return false;

            var taskScoreNumb = await _unitOfWork.TaskScoreRep.CountAsync(
                entity => entity.DeleteFlag == 0 && entity.TaskId.Equals(taskId));
            if (taskScoreNumb == 0) return true;
            var finishTaskNumb = await _unitOfWork.TaskScoreRep.CountAsync(
                entity => entity.DeleteFlag == 0 && entity.TaskId.Equals(taskId) && entity.Status == 0);
            var passTaskNumb = await _unitOfWork.TaskScoreRep.CountAsync(
                entity => entity.DeleteFlag == 0 && entity.TaskId.Equals(taskId) && entity.Result == 0);
            taskScoreTemp.FinishPercent = finishTaskNumb * 1.0f / taskScoreNumb * 100;
            taskScoreTemp.PassPercent = passTaskNumb * 1.0f / taskScoreNumb * 100;
            var result = await _unitOfWork.TaskRep.UpdateAsync(taskScoreTemp);
            return result;
        }

        private async Task<bool> UpdateSubjectStatistics(long subjectScoreId)
        {
            var subjectScoreTemp = await _unitOfWork.SubjectScoreRep.GetAsync(subjectScoreId);//科目成绩表中找
            if (subjectScoreTemp == null) return false;//科目成绩表中不存在
            var subjectTemp = await _unitOfWork.SubjectRep.GetAsync(entity=>entity.Id.Equals(subjectScoreTemp.SubjectId));//科目表中找副本
            if (subjectTemp == null) return true;//科目资源表中不存在
            var subjectScoreNumb = await _unitOfWork.SubjectScoreRep.CountAsync(
                entity => entity.DeleteFlag == 0 && entity.SubjectId.Equals(subjectScoreTemp.SubjectId));//科目成绩表中统计同类科目的数量
            if (subjectScoreNumb == 0) return true;//当前这类科目总数为0就不用更新了
            var finishSubjectNumb = await _unitOfWork.SubjectScoreRep.CountAsync(entity => entity.DeleteFlag == 0 && 
                                                                                  entity.SubjectId.Equals(subjectScoreTemp.SubjectId) &&
                                                                                  entity.Status == 0);
            var passSubjectNumb = await _unitOfWork.SubjectScoreRep.CountAsync(entity => entity.DeleteFlag == 0 &&
                                                                                  entity.SubjectId.Equals(subjectScoreTemp.SubjectId) &&
                                                                                  entity.Result == 0);
            subjectTemp.FinishPercent = finishSubjectNumb * 1.0f / subjectScoreNumb * 100;
            subjectTemp.PassPercent = passSubjectNumb * 1.0f / subjectScoreNumb * 100;
            var result = await _unitOfWork.SubjectRep.UpdateAsync(subjectTemp);
            return result;
        }

        ///// <summary>
        ///// 建立任务与科目的关系
        ///// </summary>
        ///// <param name="taskId">任务id</param>
        ///// <param name="subjectIds">科目id集合</param>
        ///// <returns></returns>
        //private async Task<bool> CreateTaskSubjectRef(long taskId, IList<long> subjectIds)
        //{
        //    if (subjectIds == null || subjectIds.Count == 0) return false;
        //    var taskEntity = await _unitOfWork.TaskRep.GetAsync(entity => entity.Id.Equals(taskId));
        //    if (taskEntity == null) return false;

        //    var relation = new TaskToSubject { Task = taskEntity, SubjectIds = subjectIds };
        //    var model = await _unitOfWork.TaskRep.Entities.Include(x => x.SubjectRefEntities)
        //        .FirstOrDefaultAsync(x => x.Id == relation.Task.Id);
        //    //多对多关系更新
        //    await _unitOfWork.TaskSubjectRefRep.TryUpdateManyToMany(model.SubjectRefEntities,
        //        relation.SubjectIds.Select(x => new TaskSubjectRefEntity { TaskId = x, TaskId = relation.Task.Id }),
        //        x => x.TaskId);

        //    var result = await _unitOfWork.TaskRep.SaveChangesAsync();
        //    return result > 0;
        //}

        ///// <summary>
        ///// 建立科目与知识点的关联关系
        ///// </summary>
        ///// <param name="subjectId">科目id</param>
        ///// <param name="tagIds">知识点id集合</param>
        ///// <returns></returns>
        //private async Task<bool> CreateSubjectTagRef(long subjectId, IList<long> tagIds)
        //{
        //    var subjectEntity = await _unitOfWork.SubjectRep.GetAsync(entity => entity.Id.Equals(subjectId));
        //    if (subjectEntity == null) return false;

        //    var relation = new SubjectToTag { Subject = subjectEntity, Tags = tagIds };
        //    var model = await _unitOfWork.SubjectRep.Entities.Include(x => x.TagRefEntities)
        //        .FirstOrDefaultAsync(x => x.Id == relation.Subject.Id);
        //    //多对多关系更新
        //    await _unitOfWork.SubjectTagRefRep.TryUpdateManyToMany(model.TagRefEntities,
        //        relation.Tags.Select(x => new SubjectTagRefEntity { TagId = x, TaskId = relation.Subject.Id }),
        //        x => x.TagId);
        //    //
        //    var result = await _unitOfWork.SubjectRep.SaveChangesAsync();
        //    return result > 0;
        //}

        ///// <summary>
        ///// 创建科目集合
        ///// </summary>
        ///// <param name="refs">科目信息dto</param>
        ///// <returns>科目id集合</returns>
        //private async Task<IList<long>> NewSubjectIds(IList<TaskSubjectRefNewDto> refs)
        //{
        //    var subjectIds = new List<long>();
        //    if (refs == null || refs.Count == 0) return subjectIds;//不存在返回空列表

        //    foreach (var subjectRefDto in refs)
        //    {
        //        var tempSubjectDto = subjectRefDto.Subject;
        //        if (tempSubjectDto == null) continue;
        //        var tempTagIds = await GetTagIds(tempSubjectDto.TagRefEntities);//获取关联知识点id集合
        //        tempSubjectDto.TagRefEntities.Clear();//dto中的知识点信息清空，避免科目插表失败

        //        //新增科目
        //        var tempSubjectEntity = _mapper.Map<SubjectEntity>(tempSubjectDto);
        //        var tempInsert = await _unitOfWork.SubjectRep.InsertAsync(tempSubjectEntity);//插数据库//

        //        var tempResult = false;
        //        if (tempInsert) tempResult = await CreateSubjectTagRef(tempSubjectEntity.Id, tempTagIds);//建立关系
        //        subjectIds.Add(tempSubjectEntity.Id);
        //        _logger.LogInformation(LogHelper.OutputClearness($"科目id为{tempSubjectEntity.Id}添加知识点【{(tempResult ? "成功" : "失败")}】"));
        //    }

        //    return subjectIds;
        //}

        ///// <summary>
        ///// 新增知识点
        ///// </summary>
        ///// <param name="refs">知识点dto集合</param>
        ///// <returns>返回知识点id集合</returns>
        //private async Task<IList<long>> GetTagIds(IList<SubjectTagRefDto> refs)
        //{
        //    var tagIds = new List<long>();
        //    if (refs == null || refs.Count == 0) return tagIds;//不存在返回空列表

        //    foreach (var tagRefDto in refs)
        //    {
        //        var tempTagDto = tagRefDto.Tag;
        //        if (tempTagDto == null) continue;
        //        //知识点表中是否已经存在同原始id和名字的知识点
        //        var temp = await _unitOfWork.TagRep.GetAsync(
        //            entity => entity.OriginalId.Equals(tempTagDto.OriginalId) &&
        //                      entity.TagName.Equals(tempTagDto.TagName));
        //        if (temp != null)
        //        {
        //            //存在知识点，将知识点存起来
        //            tagIds.Add(temp.Id);
        //            continue;
        //        }
        //        //不存在，新增知识点
        //        var tempTagEntity = _mapper.Map<TagEntity>(tempTagDto);
        //        var tempResult = await _unitOfWork.TagRep.InsertAsync(tempTagEntity);
        //        if (tempResult)
        //        {
        //            //添加成功
        //            tagIds.Add(tempTagEntity.Id);
        //        }
        //    }

        //    return tagIds;
        //}

        ///// <summary>
        ///// 删除任务
        ///// </summary>
        ///// <param name="id"></param>
        ///// <returns></returns>
        //public async Task<bool> DeleteTaskById(long id)
        //{
        //    bool result = false;
        //    var taskTemp = await _unitOfWork.TaskRep.GetFullAsync(x => x.Id.Equals(id));
        //    if (taskTemp != null)
        //    {
        //        result = await _unitOfWork.TaskRep.DeleteAsync(taskTemp);
        //    }

        //    return result;
        //}

        ///// <summary>
        ///// 删除科目
        ///// </summary>
        ///// <param name="id"></param>
        ///// <returns></returns>
        //public async Task<bool> DeleteSubjectById(long id)
        //{
        //    bool result = false;
        //    var subjectTemp = await _unitOfWork.SubjectRep.GetAsync(x => x.Id.Equals(id));
        //    if (subjectTemp != null)
        //    {
        //        result = await _unitOfWork.SubjectRep.DeleteAsync(subjectTemp);
        //    }

        //    return result;
        //}

        ///// <summary>
        ///// 删除标签
        ///// </summary>
        ///// <param name="id"></param>
        ///// <returns></returns>
        //public async Task<bool> DeleteTagById(long id)
        //{
        //    bool result = false;
        //    var tagTemp = await _unitOfWork.TagRep.GetAsync(x => x.Id.Equals(id));
        //    if (tagTemp != null)
        //    {
        //        result = await _unitOfWork.TagRep.DeleteAsync(tagTemp);
        //    }

        //    return result;
        //}
    }
}
