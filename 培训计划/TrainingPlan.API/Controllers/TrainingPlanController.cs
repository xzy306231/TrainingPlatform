using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

/// <summary>
/// 培训计划
/// </summary>
[Route("trainingplan/v1")]
[ApiController]
public class TrainingPlanController : ControllerBase
{
    private readonly IHttpClientHelper client;
    private readonly TrainingPlan trainingPlan;
    private readonly IConfiguration configuration;
    private readonly RabbitMQClient rabbit;
    private readonly pf_training_plan_v1Context db;
       /// <summary>
       /// 
       /// </summary>
       /// <param name="client"></param>
       /// <param name="trainingPlan"></param>
    public TrainingPlanController(IHttpClientHelper client, TrainingPlan trainingPlan, IConfiguration configuration, RabbitMQClient rabbit, pf_training_plan_v1Context db)
    {
        this.client = client;
        this.trainingPlan = trainingPlan;
        this.configuration = configuration;
        this.rabbit = rabbit;
        this.db = db;
    }
    #region 培训计划
    /// <summary>
    /// 获取培训计划
    /// </summary>
    /// <param name="strStatus">状态</param>
    /// <param name="planName">计划名称</param>
    /// <param name="startTime">开始时间</param>
    /// <param name="endTime">结束时间</param>
    /// <param name="pageIndex">页码</param>
    /// <param name="pageSize">记录数</param>
    /// <returns></returns>
    [HttpGet("GetTrainingPlan")]
    public JsonResult GetTrainingPlan(string strStatus, string planName, string startTime, string endTime, int pageIndex = 1, int pageSize = 10)
    {
        var requestJWT = Request.Headers["Authorization"];
        TokenModel obj = Newtonsoft.Json.JsonConvert.DeserializeObject<TokenModel>(client.GetTokenJson(requestJWT).Result);
        return new JsonResult(trainingPlan.GetTrainingPlan(db,strStatus, planName, startTime, endTime, pageIndex, pageSize, obj));
    }

    /// <summary>
    /// 创建培训计划
    /// </summary>
    /// <param name="model">计划模型</param>
    /// <returns></returns>
    [HttpPost("Add_TrainingPlan")]
    public JsonResult Add_TrainingPlan(TrainingPlanModel model)
    {
        var requestJWT = Request.Headers["Authorization"];
        TokenModel obj = Newtonsoft.Json.JsonConvert.DeserializeObject<TokenModel>(client.GetTokenJson(requestJWT).Result);
        return new JsonResult(trainingPlan.Add_TrainingPlan(db,rabbit,model, obj));
    }

    /// <summary>
    ///修改培训计划 
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPut("Update_TrainingPlan")]
    public JsonResult Update_TrainingPlan(TrainingPlanModel model)
    {
        var requestJWT = Request.Headers["Authorization"];
        TokenModel obj = Newtonsoft.Json.JsonConvert.DeserializeObject<TokenModel>(client.GetTokenJson(requestJWT).Result);
        return new JsonResult(trainingPlan.Update_TrainingPlan(db,rabbit,model, obj, client));
    }

    /// <summary>
    /// 中止培训计划
    /// </summary>
    /// <param name="TrainingPlanID">培训计划ID</param>
    /// <returns></returns>
    [HttpPut("Update_QuitTrainingPlan")]
    public JsonResult Update_QuitTrainingPlan(long TrainingPlanID)
    {
        var requestJWT = Request.Headers["Authorization"];
        TokenModel obj = Newtonsoft.Json.JsonConvert.DeserializeObject<TokenModel>(client.GetTokenJson(requestJWT).Result);
        return new JsonResult(trainingPlan.Update_QuitTrainingPlan(db,rabbit,TrainingPlanID, obj, client));
    }

    /// <summary>
    /// 发布培训计划
    /// </summary>
    /// <param name="id"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    [HttpPut("Update_PublishTrainingPlan")]
    public JsonResult Update_PublishTrainingPlan(long id)
    {
        var requestJWT = Request.Headers["Authorization"];
        TokenModel obj = Newtonsoft.Json.JsonConvert.DeserializeObject<TokenModel>(client.GetTokenJson(requestJWT).Result);
        return new JsonResult(trainingPlan.Update_PublishTrainingPlan(db,rabbit,id, obj, client));
    }

    /// <summary>
    /// 删除培训计划
    /// </summary>
    /// <param name="TrainingPlanID">培训计划ID</param>
    /// <returns></returns>
    [HttpDelete("Delete_TrainingPlan")]
    public JsonResult Delete_TrainingPlan(long TrainingPlanID)
    {
        var requestJWT = Request.Headers["Authorization"];
        TokenModel obj = Newtonsoft.Json.JsonConvert.DeserializeObject<TokenModel>(client.GetTokenJson(requestJWT).Result);
        return new JsonResult(trainingPlan.Delete_TrainingPlan(db,rabbit,TrainingPlanID, obj, client));
    }

    /// <summary>
    /// 培训计划发布
    /// </summary>
    /// <param name="PlanID"></param>
    /// <returns></returns>
    [HttpPut("PubLish_TrainingPlan")]
    public JsonResult PubLish_TrainingPlan(long PlanID)
    {
        return new JsonResult(trainingPlan.PubLish_TrainingPlan(db,PlanID));
    }

    #endregion

    #region 计划内容
    /// <summary>
    /// 获取培训计划内容
    /// </summary>
    /// <param name="PlanID">培训计划ID</param>
    /// <returns></returns>
    [HttpGet("GetTrainingPlanContent")]
    public JsonResult GetTrainingPlanContent(long PlanID)
    {
        return new JsonResult(trainingPlan.GetTrainingPlanContent(db,PlanID));
    }

    /// <summary>
    /// 计划内容排序变更
    /// </summary>
    /// <param name="sortList"></param>
    /// <returns></returns>
    [HttpPut("Update_ContentSort")]
    public JsonResult Update_ContentSort(List<ContentSort> sortList, string planName)
    {
        var requestJWT = Request.Headers["Authorization"];
        TokenModel obj = Newtonsoft.Json.JsonConvert.DeserializeObject<TokenModel>(client.GetTokenJson(requestJWT).Result);
        return new JsonResult(trainingPlan.Update_ContentSort(db,rabbit,sortList, planName, obj));
    }
    /// <summary>
    /// 课程添加到培训计划
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPost("Add_CourseToTrainingPlan")]
    public async Task<JsonResult> Add_CourseToTrainingPlan(CourseListID model)
    {
        var requestJWT = Request.Headers["Authorization"];
        TokenModel obj = Newtonsoft.Json.JsonConvert.DeserializeObject<TokenModel>(client.GetTokenJson(requestJWT).Result);
        return new JsonResult(await trainingPlan.Add_CourseToTrainingPlan(db,rabbit,model, obj, client));
    }

    /// <summary>
    /// 训练任务添加到培训计划
    /// </summary>
    /// <param name="trainingTask"></param>
    /// <returns></returns>
    [HttpPost("Add_TrainingTaskToTrainingPlan")]
    public JsonResult Add_TrainingTaskToTrainingPlan(TrainingTaskList trainingTask)
    {
        var requestJWT = Request.Headers["Authorization"];
        TokenModel obj = Newtonsoft.Json.JsonConvert.DeserializeObject<TokenModel>(client.GetTokenJson(requestJWT).Result);
        List<long?> taskList = trainingPlan.Add_TrainingTaskToTrainingPlan(db,rabbit,trainingTask, obj);
        if (taskList != null && taskList.Count > 0)
        {
            List<t_trainingplan_stu> stuList = trainingPlan.GetStuByPlanID(db,trainingTask.PlanID);
            //存在学员
            if (stuList != null && stuList.Count > 0)
            {
                List<UserInfo> userInfoList = new List<UserInfo>();
                for (int i = 0; i < stuList.Count; i++)
                {
                    userInfoList.Add(new UserInfo()
                    {
                        UserId = stuList[i].user_id,
                        UserName = stuList[i].user_name,
                        Department = stuList[i].department
                    });
                }

                UserModel userModel = new UserModel();
                userModel.PlanId = trainingTask.PlanID;
                userModel.TaskId = taskList;
                userModel.NewUsers = userInfoList;

                //调用远程服务
                trainingPlan.Add_TaskStuToRemoteService(userModel, client);
                return new JsonResult(new { code = 200, msg = "OK" });
            }
            else
                return new JsonResult(new { code = 200, msg = "OK" });
        }
        else
        {
            return new JsonResult(new { code = 400, msg = "Error" });
        }

    }

    /// <summary>
    /// 考试添加到培训计划
    /// </summary>
    /// <param name="examinationList"></param>
    /// <returns></returns>
    [HttpPost("Add_ExamToTrainingPlan")]
    public JsonResult Add_ExamToTrainingPlan(ExaminationList examinationList)
    {
        var requestJWT = Request.Headers["Authorization"];
        TokenModel obj = Newtonsoft.Json.JsonConvert.DeserializeObject<TokenModel>(client.GetTokenJson(requestJWT).Result);
        return new JsonResult(trainingPlan.Add_ExamToTrainingPlan(db,rabbit,examinationList, obj, client));
    }

    /// <summary>
    /// 为训练计划内容指定教员
    /// </summary>
    /// <param name="ContentID">内容ID</param>
    /// <param name="TeacherID">教员ID</param>
    /// <param name="TeacherName">教师名字</param>
    /// <returns></returns>
    [HttpPut("Add_TeacherToTrainingPlan")]
    public JsonResult Add_TeacherToTrainingPlan(long ContentID, string TeacherID, string TeacherName, string planName, string contentName)
    {
        var requestJWT = Request.Headers["Authorization"];
        TokenModel obj = Newtonsoft.Json.JsonConvert.DeserializeObject<TokenModel>(client.GetTokenJson(requestJWT).Result);
        return new JsonResult(trainingPlan.Add_TeacherToTrainingPlan(db,rabbit,ContentID, TeacherID, TeacherName, planName, contentName, obj));
    }

    /// <summary>
    /// 将内容从培训计划中移除
    /// </summary>
    /// <param name="id">内容ID</param>
    /// <returns></returns>
    [HttpDelete("Delete_ContentFromTrainingPlan")]
    public JsonResult Delete_ContentFromTrainingPlan(long id, string planName, string contentName)
    {
        var requestJWT = Request.Headers["Authorization"];
        TokenModel obj = Newtonsoft.Json.JsonConvert.DeserializeObject<TokenModel>(client.GetTokenJson(requestJWT).Result);
        //计划ID
        long? planid = trainingPlan.GetPlanIDByContentID(db,id);
        //考试ID
        long? examid = trainingPlan.GetExamIDByContentID(db,id);
        if (examid != null)
        {
            List<long?> list = new List<long?>();
            list.Add(examid);
            ExamUserModel examUser = new ExamUserModel();
            examUser.PlanID = planid;
            examUser.ExaminationListID = list;
            examUser.userInfos = trainingPlan.GetStuByPlanID(db,planid);
            //调用远程服务
            trainingPlan.Delete_ExamStusToRemoteService(examUser, client);
        }
        return new JsonResult(trainingPlan.Delete_ContentFromTrainingPlan(db,rabbit,id, planName, contentName, obj));
    }
    #endregion

    #region 课程学习参与条件

    /// <summary>
    /// 为课程添加学习条件
    /// </summary>
    /// <param name="ModelList">条件模型</param>
    /// <returns></returns>
    [HttpPost("Add_CourseLearningCondition")]
    public JsonResult Add_CourseLearningCondition(ConfigLearningConditionList ModelList)
    {
        var requestJWT = Request.Headers["Authorization"];
        TokenModel obj = Newtonsoft.Json.JsonConvert.DeserializeObject<TokenModel>(client.GetTokenJson(requestJWT).Result);
        return new JsonResult(trainingPlan.Add_CourseLearningCondition(db,rabbit,ModelList, obj));
    }

    /// <summary>
    /// 修改课程学习条件
    /// </summary>
    /// <param name="ModelList">条件模型</param>
    /// <returns></returns>
    [HttpPut("Update_CourseLearningCondition")]
    public JsonResult Update_CourseLearningCondition(ConfigLearningConditionList ModelList)
    {
        var requestJWT = Request.Headers["Authorization"];
        TokenModel obj = Newtonsoft.Json.JsonConvert.DeserializeObject<TokenModel>(client.GetTokenJson(requestJWT).Result);
        return new JsonResult(trainingPlan.Update_CourseLearningCondition(db,rabbit,ModelList, obj));
    }
    #endregion

    #region 培训计划学员管理

    /// <summary>
    /// 获取学员
    /// </summary>
    /// <param name="PlanID">计划ID</param>
    /// <param name="eductionKey">学历</param>
    /// <param name="airModelKey">机型</param>
    /// <param name="skillLevelKey">技术等级</param>
    /// <param name="flyStatusKey">飞行状态</param>
    /// <param name="page">页码</param>
    /// <param name="perPage">每页记录数</param>
    /// <param name="durationStart">飞行时长起始值</param>
    /// <param name="durationEnd">飞行时长结束值</param>
    /// <returns></returns>
    [HttpGet("GetStuToPlan")]
    public object GetStuToPlan(long PlanID, string eductionKey, string airModelKey, string skillLevelKey, string flyStatusKey, int page = 1, int perPage = 10, double durationStart = -1, double durationEnd = -1)
    {
        //排除登录的那个人
        var requestJWT = Request.Headers["Authorization"];
        TokenModel obj = Newtonsoft.Json.JsonConvert.DeserializeObject<TokenModel>(client.GetTokenJson(requestJWT).Result);
        string strQuery = "cardId=" + obj.userNumber + "&";

        //查找评分的教员
        List<long> examinationIDList = trainingPlan.GetExaminationIDList(db,PlanID);
        if (examinationIDList != null && examinationIDList.Count > 0)//调用远程服务
        {
            string examUrl = @"http://EXAMINATION-SERVICE/examination/v1/GetExamGradeTech";
            string strResult = client.PostRequestResult(examUrl, examinationIDList);
            List<string> strList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(strResult);
            for (int i = 0; i < strList.Count; i++)
            {
                strQuery = strQuery + "cardId=" + strList[i] + "&";
            }
        }

        //排除创建的那个人
        string str = trainingPlan.GetPlanCreateNum(db,PlanID);
        if (!string.IsNullOrEmpty(str))
            strQuery = strQuery + "cardId=" + str + "&";

        //查找已存在的学员
        List<string> cardId = trainingPlan.GetStuToPlan(db,PlanID);
        if (cardId != null && cardId.Count > 0)
        {
            for (int i = 0; i < cardId.Count; i++)
            {
                strQuery = strQuery + "cardId=" + cardId[i] + "&";
            }
        }
        //查找已存在的老师
        List<string> TeacherList = trainingPlan.GetTeacherFromPlan(db,PlanID);
        if (TeacherList != null && TeacherList.Count > 0)
        {
            for (int i = 0; i < TeacherList.Count; i++)
            {
                strQuery = strQuery + "cardId=" + TeacherList[i] + "&";
            }
        }

        if (!string.IsNullOrEmpty(eductionKey))
            strQuery = strQuery + "eductionKey=" + eductionKey + "&";
        if (!string.IsNullOrEmpty(airModelKey))
            strQuery = strQuery + "airModelKey=" + airModelKey + "&";
        if (!string.IsNullOrEmpty(skillLevelKey))
            strQuery = strQuery + "skillLevelKey=" + skillLevelKey + "&";
        if (!string.IsNullOrEmpty(flyStatusKey))
            strQuery = strQuery + "flyStatusKey=" + flyStatusKey + "&";
        strQuery += "durationStart=" + durationStart + "&durationEnd=" + durationEnd + "&page=" + page + "&perPage=" + perPage;

        string strUrl = "http://PEOPLEMANAGER-SERVICE/peoplemanager/v1/otherServer/filterShow/student?" + strQuery;
        string str1 = client.GetRequest(strUrl).Result;
        return str1;
    }

    /// <summary>
    /// 为培训计划添加学员
    /// </summary>
    /// <param name="trainingPlanStuList">学员模型</param>
    /// <returns></returns>
    [HttpPost("Add_StuToTrainingPlan")]
    public JsonResult Add_StuToTrainingPlan(TrainingPlanStuList trainingPlanStuList)
    {
        var requestJWT = Request.Headers["Authorization"];
        TokenModel obj = Newtonsoft.Json.JsonConvert.DeserializeObject<TokenModel>(client.GetTokenJson(requestJWT).Result);
        return new JsonResult(trainingPlan.Add_StuToTrainingPlan(db,rabbit,trainingPlanStuList, obj));
    }

    /// <summary>
    /// 为培训计划添加学员
    /// </summary>
    /// <param name="planStu"></param>
    /// <returns></returns>
    [HttpPost("Add_StuFromRemoteService")]
    public JsonResult Add_StuFromRemoteService(PlanStu planStu)
    {
        var requestJWT = Request.Headers["Authorization"];
        TokenModel obj = Newtonsoft.Json.JsonConvert.DeserializeObject<TokenModel>(client.GetTokenJson(requestJWT).Result);

        //排除登录人
        string strQuery = "cardId=" + obj.userNumber + "&";

        //排除创建的那个人
        string str = new TrainingPlan().GetPlanCreateNum(db,planStu.PlanID);
        if (!string.IsNullOrEmpty(str))
            strQuery = strQuery + "cardId=" + str + "&";

        //计划中已存在的人员
        List<string> cardId = trainingPlan.GetStuToPlan(db,planStu.PlanID);
        if (cardId != null && cardId.Count > 0)
        {
            for (int i = 0; i < cardId.Count; i++)
            {
                strQuery = strQuery + "cardId=" + cardId[i] + "&";
            }
        }
        if (planStu.selectList != null && planStu.selectList.Count > 0)
        {
            for (int i = 0; i < planStu.selectList.Count; i++)
            {
                strQuery = strQuery + "selectList=" + planStu.selectList[i] + "&";
            }
        }
        if (!string.IsNullOrEmpty(planStu.eductionKey))
            strQuery = strQuery + "eductionKey=" + planStu.eductionKey + "&";
        if (!string.IsNullOrEmpty(planStu.airModelKey))
            strQuery = strQuery + "airModelKey=" + planStu.airModelKey + "&";
        if (!string.IsNullOrEmpty(planStu.skillLevelKey))
            strQuery = strQuery + "skillLevelKey=" + planStu.skillLevelKey + "&";
        if (!string.IsNullOrEmpty(planStu.flyStatusKey))
            strQuery = strQuery + "flyStatusKey=" + planStu.flyStatusKey + "&";
        if (planStu.selectAll)
            strQuery = strQuery + "selectAll=" + planStu.selectAll + "&";
        if (planStu.selectAll == false)
            strQuery = strQuery + "selectAll=" + planStu.selectAll + "&";

        strQuery += "durationStart=" + planStu.durationStart + "&durationEnd=" + planStu.durationEnd;
        string strUrl = "http://PEOPLEMANAGER-SERVICE/peoplemanager/v1/otherServer/selectPersons/student?" + strQuery;
        string str1 = client.GetRequest(strUrl).Result;
        //筛选之后的学员
        List<TrainingPlanSelectDto> list = Newtonsoft.Json.JsonConvert.DeserializeObject<List<TrainingPlanSelectDto>>(str1);

        //将学员写入到本地库
        trainingPlan.Add_StuFromRemoteService(db,rabbit,planStu.PlanID, list, obj);

        //获取培训计划下的所有训练任务ID集合
        List<long?> TaskList = trainingPlan.GetPlanTask(db,planStu.PlanID);
        if (TaskList != null && TaskList.Count > 0)
        {
            List<UserInfo> userInfoList = new List<UserInfo>();
            //遍历新添加的学员
            if (list != null && list.Count > 0)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    userInfoList.Add(new UserInfo()
                    {
                        UserId = list[i].ID,
                        UserName = list[i].userName,
                        Department = list[i].departmentKey
                    });
                }
            }
            UserModel userModel = new UserModel();
            userModel.PlanId = planStu.PlanID;
            userModel.TaskId = TaskList;
            userModel.NewUsers = userInfoList;
            //调用远程服务
            trainingPlan.Add_TaskStuToRemoteService(userModel, client);
        }

        //获取培训计划下的所有考试ID集合
        List<long?> ExamList = trainingPlan.GetPlanExam(db,planStu.PlanID);
        if (ExamList != null && ExamList.Count > 0)
        {
            try
            {
                List<ExamUserInfo> examUsers = new List<ExamUserInfo>();
                //遍历新添加的学员
                if (list != null && list.Count > 0)
                {
                    for (int i = 0; i < list.Count; i++)
                    {
                        examUsers.Add(new ExamUserInfo()
                        {
                            UserNumber = list[i].userNumber,
                            UserName = list[i].userName,
                            Department = list[i].departmentKey
                        });
                    }
                }
                ExamUserModel model = new ExamUserModel();
                model.PlanID = planStu.PlanID;
                model.ExaminationListID = ExamList;
                model.userInfos = examUsers;
                //调用远程服务
                trainingPlan.Add_ExamStuToRemoteService(model, client);
            }
            catch (Exception ex)
            {
                string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
                PubMethod.ErrorLog(ex, path);
            }

        }

        return new JsonResult(new { code = 200, message = "Ok" });
    }

    /// <summary>
    ///将学员从培训计划中移除
    /// </summary>
    /// <param name="studentIDList"></param>
    /// <returns></returns>
    [HttpDelete("Delete_StuFromTrainingPlan")]
    public JsonResult Delete_StuFromTrainingPlan(StudentIDList studentIDList)
    {
        var requestJWT = Request.Headers["Authorization"];
        TokenModel obj = Newtonsoft.Json.JsonConvert.DeserializeObject<TokenModel>(client.GetTokenJson(requestJWT).Result);
        return new JsonResult(trainingPlan.Delete_StuFromTrainingPlan(db,rabbit,studentIDList, obj, client));
    }

    /// <summary>
    /// 移除所有学员
    /// </summary>
    /// <param name="strEducation">学历</param>
    /// <param name="strPlane">机型</param>
    /// <param name="strSkill">技术等级</param>
    /// <param name="strFlySta">飞行状态</param>
    /// <param name="PlanID">计划ID</param>
    /// <returns></returns>
    [HttpDelete("Delete_AllStuFromTrainingPlan")]
    public JsonResult Delete_AllStuFromTrainingPlan(string strEducation, string strPlane, string strSkill, string strFlySta, long PlanID)
    {
        var requestJWT = Request.Headers["Authorization"];
        TokenModel obj = Newtonsoft.Json.JsonConvert.DeserializeObject<TokenModel>(client.GetTokenJson(requestJWT).Result);
        return new JsonResult(trainingPlan.Delete_AllStuFromTrainingPlan(db,rabbit,strEducation, strPlane, strSkill, strFlySta, PlanID, obj, client));
    }

    /// <summary>
    /// 查询受训学员
    /// </summary>
    /// <param name="strEducation">学历</param>
    /// <param name="strPlane">机型</param>
    /// <param name="strSkill">技术等级</param>
    /// <param name="strFlySta">飞行状态</param>
    /// <param name="PlanID">计划ID</param>
    /// <param name="PageSize">记录数</param>
    /// <param name="PageIndex">页码</param>
    /// <returns></returns>
    [HttpGet("GetStuFromTrainingPlan")]
    public JsonResult GetStuFromTrainingPlan(string strEducation, string strPlane, string strSkill, string strFlySta, long PlanID, int PageSize = 30, int PageIndex = 1)
    {
        string str = configuration["DicUrl"];
        //部门
        string strDepartment = client.GetRequest(str + "department_type").Result;
        DicModel dicDepartment = Newtonsoft.Json.JsonConvert.DeserializeObject<DicModel>(strDepartment);

        //机型
        string strPlaneType = client.GetRequest(str + "plane_type").Result;
        DicModel dicPlaneType = Newtonsoft.Json.JsonConvert.DeserializeObject<DicModel>(strPlaneType);

        return new JsonResult(trainingPlan.GetStuFromTrainingPlan(db,configuration,strEducation, strPlane, strSkill, strFlySta, PlanID, dicDepartment, dicPlaneType, PageSize, PageIndex));
    }

    /// <summary>
    /// 获取培训计划中的学员ID
    /// </summary>
    /// <param name="ID"></param>
    /// <returns></returns>
    [HttpGet("GetStuByTrainingPlan")]
    public JsonResult GetStuIDByTrainingPlan(long ID)
    {

        return new JsonResult(trainingPlan.GetStuByTrainingPlan(db,ID));
    }

    #endregion

    #region 从大纲生成

    /// <summary>
    /// 从大纲生成培训计划
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPost("Add_TrainingPlanFromProgram")]
    public JsonResult Add_TrainingPlanFromProgram(TrainingPlanFromProgram model)
    {
        var requestJWT = Request.Headers["Authorization"];
        TokenModel obj = Newtonsoft.Json.JsonConvert.DeserializeObject<TokenModel>(client.GetTokenJson(requestJWT).Result);
        return new JsonResult(trainingPlan.Add_TrainingPlanFromProgram(db,rabbit,model, obj, client));
    }
    #endregion

    #region 学习统计

    /// <summary>
    /// 学习统计
    /// </summary>
    /// <param name="PlanID">计划ID</param>
    /// <param name="CourseID">课程ID</param>
    /// <returns></returns>
    [HttpGet("GetLearningStatisticByCourseID")]
    public JsonResult GetLearningStatisticByCourseID(long PlanID, long CourseID)
    {
        return new JsonResult(trainingPlan.GetLearningStatisticByCourseID(db,PlanID, CourseID));
    }

    #endregion

}
