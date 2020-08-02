using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using ApiUtil.Entities;
using LinqKit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TrainingTask.Api.Common;
using TrainingTask.Api.ViewModel.Task;
using TrainingTask.Api.ViewModel.TaskScore;
using TrainingTask.Core.Entity;

namespace TrainingTask.Api.Controllers
{
    public partial class TaskManageController
    {
        /// <summary>
        /// 训练任务结果
        /// 培训计划
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("taskResult")]
        public async Task<IActionResult> TaskResult(long id)
        {
            var taskScoreTemp = await _unitOfWork.TaskScoreRep.GetAsync(entity => entity.Id.Equals(id));
            if (taskScoreTemp == null) return Ok(new ResponseError("该训练任务不存在"));
            var taskEntityTemp =
                await _unitOfWork.TaskRep.GetFullAsync(entity => entity.Id.Equals(taskScoreTemp.TaskId));
            var taskDto = _mapper.Map<TaskFullDto>(taskEntityTemp);
            return Ok(new ResponseInfo { Result = taskDto });
        }

        /// <summary>
        /// 任务统计
        /// 培训计划
        /// </summary>
        /// <param name="id">任务id</param>
        /// <returns></returns>
        [HttpGet("taskStatistics")]
        public async Task<IActionResult> TaskStatistics(long id)
        {
            var taskScores = await _unitOfWork.TaskScoreRep.GetListAsync(entity => entity.TaskId.Equals(id));
            if (taskScores?.Count == 0) return Ok(new ResponseInfo { Result = new { result = 1, subjectNumb = 0, passNumb = 0, passPercent = 0, finishNumb = 0, finishPercent = 0 } });

            var pass = await _unitOfWork.TaskScoreRep.GetListAsync(entity => entity.TaskId.Equals(id) && entity.Result.Equals(0));
            var finish = await _unitOfWork.TaskScoreRep.GetListAsync(entity => entity.TaskId.Equals(id) && entity.Status.Equals(0));

            return Ok(new ResponseInfo { Result = new { passNumb = pass.Count, finishNumb = finish.Count } });
        }

        /// <summary>
        /// 训练任务统计
        /// 培训计划
        /// </summary>
        /// <param name="id">任务id</param>
        /// <param name="planId">训练计划id</param>
        /// <param name="keyword">关键字</param>
        /// <param name="result">0已完成|1未完成|-1未筛选</param>
        /// <param name="status">0已完成|1未完成|-1未筛选</param>
        /// <param name="page">页码</param>
        /// <param name="perPage">每页行数</param>
        /// <returns></returns>
        [HttpGet("taskScores")]
        public async Task<IActionResult> TaskScores(long id, long planId, string keyword, int result = -1, int status = -1, int page = 1, int perPage = 10)
        {
            var predicate = PredicateBuilder.New<TaskScoreEntity>(true).And(entity => entity.DeleteFlag == 0);
            predicate = predicate.And(entity => entity.PlanId.Equals(planId));
            predicate = predicate.And(entity => entity.TaskId.Equals(id));
            if (result != -1) predicate = predicate.And(entity => entity.Result.Equals(result));
            if (status != -1) predicate = predicate.And(entity => entity.Status.Equals(status));
            
            if (!string.IsNullOrEmpty(keyword))
                predicate = predicate.And(entity =>
                    EF.Functions.Like(entity.UserName, $"%{keyword}%") ||
                    EF.Functions.Like(entity.Department, $"%{keyword}%"));

            var temp = await _service.GetDictObjectAsync("department_type");
            FieldCheck.SetDepartment(temp);

            var tempCollection =
                await _unitOfWork.TaskScoreRep.GetPageAsync(predicate, entity => entity.Id, page, perPage);
            var pageData = _mapper.Map<PageData<TaskScoreRetrieveDto>>(tempCollection);

            return Ok(new ResponseInfo { Result = pageData });
        }

        /// <summary>
        /// 任务分析统计
        /// </summary>
        /// <returns></returns>
        [HttpGet("taskAnalysisStatistics")]
        public async Task<IActionResult> TaskAnalysisStatistics()
        {
            var taskNumb = await _unitOfWork.TaskRep.CountAsync(entity => entity.DeleteFlag == 0 && entity.Copy == 0);
            var allTaskDuration = await _unitOfWork.TaskRep.Entities
                .Where(entity => entity.DeleteFlag == 0 && entity.Copy == 0).SumAsync(entity => entity.DurationAvg);
            var taskStatus = await _unitOfWork.TaskRep.Entities
                .Where(entity => entity.DeleteFlag == 0 && entity.Copy == 0).SumAsync(entity => entity.FinishPercent);
            var taskResult = await _unitOfWork.TaskRep.Entities
                .Where(entity => entity.DeleteFlag == 0 && entity.Copy == 0).SumAsync(entity => entity.PassPercent);
            
            return Ok(new ResponseInfo
            {
                Result = new
                {
                    finish = taskNumb == 0 ? "0.00" : (taskStatus / taskNumb * 100).ToString("0.00"),
                    pass = taskNumb == 0 ? "0.00" : (taskResult / taskNumb * 100).ToString("0.00"),
                    durationAvg = taskNumb == 0 ? "0.00" : (allTaskDuration / taskNumb).ToString("0.00")
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
        [HttpGet("taskAnalysisDetail")]
        public async Task<IActionResult> TaskAnalysisDetail(int page = 1, int perPage = 10, string sortBy = "status", string order = "desc")
        {
            var sortByTemp = FieldCheck.SortByStatus_Result(sortBy);
            var orderTemp = FieldCheck.Order(order);
            if (sortByTemp == null || orderTemp == null)
            {
                _logger.LogDebug($"sortBy:{sortBy},order:{order},填写有误，自己对比");
                return Ok(new ResponseError("请求参数有误，请验证"));
            }

            var ordering = orderTemp + sortByTemp;

            var predicate = PredicateBuilder.New<TaskEntity>(true)
                .And(entity => entity.DeleteFlag == 0 && entity.Copy == 0);

            var tempCollection = await _unitOfWork.TaskRep.GetPageAsync(predicate, ordering, page, perPage);
            var pageDate = _mapper.Map<PageData<TaskStatistics>>(tempCollection);
            return Ok(new ResponseInfo {Result = pageDate});
        }

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
            if (status == -1 && result == -1) return Ok(new ResponseError("没有设置任务结束标志或者通过标志位"));
            TaskScoreEntity temp = null;
            temp = await _unitOfWork.TaskScoreRep.GetAsync(entity => entity.Id.Equals(id));
            if (temp == null) return Ok(new ResponseError("未找到当前任务"));
            if (status != -1) temp.Status = status;
            if (result != -1) temp.Result = result;
            var tempResult = await _unitOfWork.TaskScoreRep.UpdateAsync(temp);
            if (tempResult) await UpdateTaskStatistics(temp.TaskId);
            return Ok(tempResult ? new ResponseInfo() : new ResponseError("数据更新失败"));
        }

        /// <summary>
        /// 科目分析统计
        /// </summary>
        /// <returns></returns>
        [HttpGet("subjectAnalysisStatistics")]
        public async Task<IActionResult> SubjectAnalysisStatistics()
        {
            var subjectNumb =
                await _unitOfWork.SubjectRep.CountAsync(entity => entity.DeleteFlag == 0 && entity.Copy > 0);
            var subjectStatus = await _unitOfWork.SubjectRep.Entities
                .Where(entity => entity.DeleteFlag == 0 && entity.Copy > 0).SumAsync(entity => entity.FinishPercent);
            var subjectResult = await _unitOfWork.SubjectRep.Entities
                .Where(entity => entity.DeleteFlag == 0 && entity.Copy > 0).SumAsync(entity => entity.PassPercent);

            return Ok(new ResponseInfo
            {
                Result = new
                {
                    finish = subjectNumb == 0 ? "0.00" : (subjectStatus / subjectNumb * 100).ToString("0.00"),
                    pass = subjectNumb == 0 ? "0.00" : (subjectResult / subjectNumb * 100).ToString("0.00")
                }
            });
        }

        /// <summary>
        /// 科目分析详情
        /// </summary>
        /// <param name="page"></param>
        /// <param name="perPage"></param>
        /// <returns></returns>
        [HttpGet("subjectAnalysisDetail")]
        public async Task<IActionResult> SubjectAnalysisDetail(int page = 1, int perPage = 10)
        {
            var temp = _unitOfWork.SubjectRep.Entities.Where(entity => entity.DeleteFlag == 0 && entity.Copy > 0)
                .GroupBy(entity => entity.OriginalId);
            var tempList = new ArrayList();
            foreach (var group in temp.ToList())
            {
                var groupTemp = group.ToList();
                var subjectName = groupTemp.First()?.Name;
                var status = groupTemp.Average(e => e.FinishPercent);
                var result = groupTemp.Average(e => e.PassPercent);
                tempList.Add(new {subjectName = subjectName, finishPercent = status.ToString("0.00"), passPercent = result.ToString("0.00") });
            }
            return Ok(new ResponseInfo{Result = tempList});
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
            var subjectTemp = await _unitOfWork.SubjectScoreRep.GetAsync(id);
            if (subjectTemp == null) return Ok(new ResponseError("该科目不存在"));
            if (status != -1) subjectTemp.Status = status;
            if (result != -1) subjectTemp.Result = result;
            var tempResult = await _unitOfWork.SubjectScoreRep.UpdateAsync(subjectTemp);
            if (tempResult) await UpdateSubjectStatistics(id);
            return Ok(tempResult ? new ResponseInfo() : new ResponseError("科目成绩更新失败"));
        }
    }
}
