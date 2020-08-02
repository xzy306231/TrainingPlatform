using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiUtil.Entities;
using Microsoft.AspNetCore.Mvc;
using TrainingTask.Api.ViewModel.Subject;
using TrainingTask.Api.ViewModel.SubjectScore;
using TrainingTask.Api.ViewModel.TaskScore;

namespace TrainingTask.Api.Controllers
{
    public partial class TaskManageController
    {
        /// <summary>
        /// 我的任务统计
        /// 我的训练
        /// </summary>
        /// <param name="id">任务id</param>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet("myTaskStatistics")]
        public async Task<IActionResult> MyTaskStatistics(long id, long userId)
        {
            var taskScoreEntity = await _unitOfWork.TaskScoreRep.GetAsyncIncludeSubject(x => x.TaskId.Equals(id) && x.UserId.Equals(userId));
            if (taskScoreEntity == null) return Ok(new ResponseError($"id为{id}，user为{userId}的训练任务单不存在"));
            var taskEntity = await _unitOfWork.TaskRep.GetAsync(x => x.Id.Equals(taskScoreEntity.TaskId));
            var myTaskContentDto = _mapper.Map<MyTaskContentDto>(taskEntity);//任务内容
            var subjectNumb = taskScoreEntity.SubjectScores.Count;
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

            var passNumb = taskScoreEntity.SubjectScores.Count(entity => entity.Result == 0);
            var passPercent = Convert.ToInt32(passNumb * 1.0f / subjectNumb * 100);
            var finishNumb = taskScoreEntity.SubjectScores.Count(entity => entity.Status == 0);
            var finishPercent = Convert.ToInt32(finishNumb * 1.0f / subjectNumb * 100);

            var subjectScoreDtos = _mapper.Map<IList<SubjectScoreRetrieveDto>>(taskScoreEntity.SubjectScores);

            return Ok(new ResponseInfo
            {
                Result = new
                {
                    taskContent = myTaskContentDto,
                    trainingResult = taskScoreEntity.Result,
                    subjectNumb = subjectNumb,
                    passNumb = passNumb,
                    passPercent = passPercent,
                    finishNumb = finishNumb,
                    finishPercent = finishPercent,
                    subjectScores = subjectScoreDtos
                }
            });
        }

        /// <summary>
        /// 训练任务完成情况
        /// </summary>
        /// <param name="planId">计划id</param>
        /// <param name="taskId">任务id</param>
        /// <param name="userId">人员id</param>
        /// <returns></returns>
        [HttpGet("singleTaskStatus")]
        public async Task<IActionResult> SingleTaskStatus(long planId, long taskId, long userId)
        {
            var taskScoreEntity = await _unitOfWork.TaskScoreRep.GetAsyncIncludeSubject(
                x=>x.PlanId.Equals(planId) && x.TaskId.Equals(taskId) && x.UserId.Equals(userId));
            if (taskScoreEntity == null)
            {
                return NoContent();
            }

            return Ok(taskScoreEntity.Status);
        }

        /// <summary>
        /// 科目详情
        /// 我的训练
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("subjectInfo")]
        public async Task<IActionResult> SubjectInfo(long id)
        {
            var subjectEntity = await _unitOfWork.SubjectRep.GetAsyncIncludeTags(entity => entity.Id.Equals(id));
            //if (subjectEntity == null) return Ok(new ResponseError($"id为{id}的科目不存在"));
            var subjectFullDto = _mapper.Map<SubjectFullDto>(subjectEntity);
            return Ok(new ResponseInfo { Result = subjectFullDto });
        }
    }
}
