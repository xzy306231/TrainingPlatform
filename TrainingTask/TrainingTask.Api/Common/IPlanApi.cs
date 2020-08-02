using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TrainingTask.Api.ViewModel;
using WebApiClient;
using WebApiClient.Attributes;

namespace TrainingTask.Api.Common
{
    [HttpHost("http://0.0.0.0:80")]
    public interface IPlanApi : IHttpApi
    {
        //[Microsoft.AspNetCore.Mvc.HttpGet("/course/v1/GetMyTrainingTask")]
        //ITask<List<TaskUseCaseDto>> GetPlanTaskInfoAsync(string userID, long planID, 
        //    DateTime? startTime = null,DateTime? endTime = null);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [WebApiClient.Attributes.HttpGet("/course/v1/GetCourseInfoByID")]
        ITask<object> GetCourseInfoById(int id);
    }

    public class PlanHelper
    {
        private readonly IPlanApi _planApi;

        public PlanHelper([FromServices] IPlanApi httpApi)
        {
            _planApi = httpApi;
        }

        public async Task<object> GetCourseInfoById(int id)
        {
            try
            {
                return await _planApi.GetCourseInfoById(id);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
