using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class LessonSchedule
{
    #region 教室管理

    public object GetClassroom(pf_training_plan_v1Context db, string classroomType, string classroomName, string classroomStatus, int pageIndex = 1, int pageSize = 10)
    {
        try
        {
            IList<Classroom> classrooms = new List<Classroom>();
            var queryCount = db.t_classroom.Where(x => x.delete_flag == 0
                                                           && (string.IsNullOrEmpty(classroomType) ? true : x.room_type == classroomType)
                                                           && (string.IsNullOrEmpty(classroomName) ? true : x.room_name.Contains(classroomName))
                                                           && (string.IsNullOrEmpty(classroomStatus) ? true : x.room_status == classroomStatus)).Count();

            var queryClassroomList = db.t_classroom.Where(x => x.delete_flag == 0
                                                         && (string.IsNullOrEmpty(classroomType) ? true : x.room_type == classroomType)
                                                         && (string.IsNullOrEmpty(classroomName) ? true : x.room_name.Contains(classroomName))
                                                         && (string.IsNullOrEmpty(classroomStatus) ? true : x.room_status == classroomStatus)).OrderBy(x => x.room_number).Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();
            foreach (var item in queryClassroomList)
            {
                classrooms.Add(new Classroom
                {
                    ID = item.id,
                    RoomNumber = item.room_number,
                    RoomName = item.room_name,
                    RoomType = item.room_type,
                    RoomCapacity = (int)item.room_capacity,
                    RoomFunction = item.room_function,
                    RoomStatus = item.room_status
                });
            }
            return new { code = 200, result = new { classrooms, count = queryCount }, message = "OK" };
        }
        catch (Exception ex)
        {
            string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
            PubMethod.ErrorLog(ex, path);
            return new { code = 400, msg = "Error" };
        }
    }

    public async Task<object> AddClassroom(pf_training_plan_v1Context db, Classroom classroom, TokenModel token, RabbitMQClient rabbit)
    {
        try
        {
            var queryRoomCount = db.t_classroom.Where(x => x.delete_flag == 0 && x.room_number == classroom.RoomNumber).Count();
            if (queryRoomCount > 0)
                return new { code = 400, message = "教室编号已存在，请重新输入编号！" };

            t_classroom room = new t_classroom();
            room.create_number = token.userNumber;
            room.room_name = classroom.RoomName;
            room.room_type = classroom.RoomType;
            room.room_capacity = classroom.RoomCapacity;
            room.room_function = classroom.RoomFunction;
            room.room_status = classroom.RoomStatus;
            room.room_number = classroom.RoomNumber;
            db.Add(room);
            await db.SaveChangesAsync();

            SysLogModel sysLog = new SysLogModel();
            sysLog.moduleName = "教室管理";
            sysLog.opName = token.userName;
            sysLog.opNo = token.userNumber;
            sysLog.opType = 2;
            sysLog.logSuccessd = 1;
            sysLog.logDesc = "创建了编号:" + classroom.RoomNumber + ",名称:“" + classroom.RoomName + "”的教室";
            return new { code = 200, message = "OK" };
        }
        catch (Exception ex)
        {
            string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
            PubMethod.ErrorLog(ex, path);
            return new { code = 400, msg = "Error" };
        }
    }

    public async Task<object> EditClassroom(pf_training_plan_v1Context db, Classroom classroom, TokenModel token, RabbitMQClient rabbit)
    {
        try
        {
            var queryClassroom = db.t_classroom.Where(x => x.delete_flag == 0 && x.id == classroom.ID).FirstOrDefault();
            queryClassroom.room_number = classroom.RoomNumber;
            queryClassroom.room_name = classroom.RoomName;
            queryClassroom.room_capacity = classroom.RoomCapacity;
            queryClassroom.room_function = classroom.RoomFunction;
            queryClassroom.room_type = classroom.RoomType;
            await db.SaveChangesAsync();

            SysLogModel sysLog = new SysLogModel();
            sysLog.moduleName = "教室管理";
            sysLog.opName = token.userName;
            sysLog.opNo = token.userNumber;
            sysLog.opType = 3;
            sysLog.logSuccessd = 1;
            sysLog.logDesc = "修改了教室，编号:" + queryClassroom.room_number;
            return new { code = 200, message = "OK" };
        }
        catch (Exception ex)
        {
            string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
            PubMethod.ErrorLog(ex, path);
            return new { code = 400, msg = "Error" };
        }
    }

    public async Task<object> StartUse(pf_training_plan_v1Context db, List<long> list, TokenModel token, RabbitMQClient rabbit)
    {
        try
        {
            string roomNum = "";
            var queryClassroomList = db.t_classroom.Where(x => x.delete_flag == 0 && list.Contains(x.id) && x.room_status == "0").ToList();
            foreach (var item in queryClassroomList)
            {
                item.room_status = "1";//启用
                roomNum += item.room_number + ",";
            }
            await db.SaveChangesAsync();

            SysLogModel sysLog = new SysLogModel();
            sysLog.moduleName = "教室管理";
            sysLog.opName = token.userName;
            sysLog.opNo = token.userNumber;
            sysLog.opType = 3;
            sysLog.logSuccessd = 1;
            sysLog.logDesc = "启用了教室，编号:" + roomNum.TrimEnd(',');
            return new { code = 200, message = "OK" };
        }
        catch (Exception ex)
        {
            string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
            PubMethod.ErrorLog(ex, path);
            return new { code = 400, msg = "Error" };
        }
    }

    public async Task<object> StopUse(pf_training_plan_v1Context db, List<long> list, TokenModel token, RabbitMQClient rabbit)
    {
        try
        {
            string roomNum = "";
            var querySchedule = db.t_lesson_schedule.Where(x => x.delete_flag == 0 && list.Contains((long)x.classroom_id) && x.schedule_datetime > DateTime.Now).ToList();
            foreach (var item in querySchedule)
            {
                item.delete_flag = 1;
            }
            var queryClassroomList = db.t_classroom.Where(x => x.delete_flag == 0 && list.Contains(x.id) && x.room_status == "1").ToList();
            foreach (var item in queryClassroomList)
            {
                item.room_status = "0";//停用
                roomNum += item.room_number + ",";
            }
            await db.SaveChangesAsync();

            SysLogModel sysLog = new SysLogModel();
            sysLog.moduleName = "教室管理";
            sysLog.opName = token.userName;
            sysLog.opNo = token.userNumber;
            sysLog.opType = 3;
            sysLog.logSuccessd = 1;
            sysLog.logDesc = "停用了教室，编号:" + roomNum.TrimEnd(',');
            return new { code = 200, message = "OK" };
        }
        catch (Exception ex)
        {
            string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
            PubMethod.ErrorLog(ex, path);
            return new { code = 400, msg = "Error" };
        }
    }

    public async Task<object> RemoveClassrooms(pf_training_plan_v1Context db, List<long> list, TokenModel token, RabbitMQClient rabbit)
    {
        try
        {
            string roomNum = "";
            var querySchedule = db.t_lesson_schedule.Where(x => x.delete_flag == 0 && list.Contains((long)x.classroom_id) && x.schedule_datetime > DateTime.Now).ToList();
            foreach (var item in querySchedule)
            {
                item.delete_flag = 1;
            }
            var queryClassroomList = db.t_classroom.Where(x => x.delete_flag == 0 && list.Contains(x.id)).ToList();
            foreach (var item in queryClassroomList)
            {
                item.delete_flag = 1;
                roomNum += item.room_number + ",";
            }
            await db.SaveChangesAsync();

            SysLogModel sysLog = new SysLogModel();
            sysLog.moduleName = "教室管理";
            sysLog.opName = token.userName;
            sysLog.opNo = token.userNumber;
            sysLog.opType = 4;
            sysLog.logSuccessd = 1;
            sysLog.logDesc = "删除了教室，编号:" + roomNum.TrimEnd(',');
            return new { code = 200, message = "OK" };
        }
        catch (Exception ex)
        {
            string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
            PubMethod.ErrorLog(ex, path);
            return new { code = 400, msg = "Error" };
        }
    }

    #endregion


    #region 排课课表
    public object GetAllTrainingPlan(pf_training_plan_v1Context db)
    {
        try
        {
            var queryTrainingPlanList = db.t_training_plan.Where(x => x.delete_flag == 0).Select(x => new { x.id, x.plan_name, x.plan_status, x.stu_count, x.start_time, x.end_time }).ToList();
            List<TrainingPlanModel> trainings = new List<TrainingPlanModel>();
            trainings.Add(new TrainingPlanModel { ID = 0, PlanName = "全部" });
            foreach (var item in queryTrainingPlanList)
            {
                trainings.Add(new TrainingPlanModel
                {
                    ID = item.id,
                    PlanName = item.plan_name,
                    PlanStatus = item.plan_status
                });
            }
            return new { code = 200, result = trainings, message = "OK" };
        }
        catch (Exception ex)
        {
            string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
            PubMethod.ErrorLog(ex, path);
            return new { code = 400, msg = "Error" };
        }
    }
    public object GetNotEndTrainingPlan(pf_training_plan_v1Context db)
    {
        try
        {
            var queryTrainingPlanList = db.t_training_plan.Where(x => x.delete_flag == 0 && (x.plan_status == "1" || x.plan_status == "2")).Select(x => new { x.id, x.plan_name, x.stu_count, x.start_time, x.end_time }).ToList();
            List<TrainingPlanModel> trainings = new List<TrainingPlanModel>();
            foreach (var item in queryTrainingPlanList)
            {
                trainings.Add(new TrainingPlanModel
                {
                    ID = item.id,
                    PlanName = item.plan_name,
                    StuCount = item.stu_count,
                    StartTime = item.start_time.ToString(),
                    EndTime = item.end_time.ToString()
                });
            }
            return new { code = 200, result = trainings, message = "OK" };
        }
        catch (Exception ex)
        {
            string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
            PubMethod.ErrorLog(ex, path);
            return new { code = 400, msg = "Error" };
        }
    }
    public object GetAllClassroom(pf_training_plan_v1Context db, string roomType)
    {
        try
        {
            var queryClassroomList = db.t_classroom.Where(x => x.delete_flag == 0 && x.room_status == "1" && (string.IsNullOrEmpty(roomType) ? true : x.room_type == roomType)).OrderBy(x => x.room_number).ToList();
            List<Classroom> classrooms = new List<Classroom>();
            foreach (var item in queryClassroomList)
            {
                classrooms.Add(new Classroom
                {
                    ID = item.id,
                    RoomNumber = item.room_number,
                    RoomName = item.room_name,
                    RoomCapacity = (int)item.room_capacity,
                    RoomType = item.room_type
                });
            }
            return new { code = 200, result = classrooms, message = "OK" };
        }
        catch (Exception ex)
        {
            string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
            PubMethod.ErrorLog(ex, path);
            return new { code = 400, msg = "Error" };
        }
    }
    public object GetScheduleInfo(pf_training_plan_v1Context db, long planID, string trainingType, string startDate, long classroomId)
    {
        try
        {
            var queryList = db.t_classroom.Join(db.t_lesson_schedule, c => c.id, s => s.classroom_id, (c, s) => new { c, s })
                                          .Join(db.t_training_plan, s => s.s.plan_id, p => p.id, (s, p) => new { s, p })
                                          .Join(db.t_plan_course_task_exam_ref, s => s.s.s.content_id, r => r.id, (s, r) => new { s, r })
                                          .Where(x => x.s.s.c.delete_flag == 0
                                              && x.s.s.s.delete_flag == 0
                                              && x.s.p.delete_flag == 0
                                              && x.r.delete_flag == 0
                                              && (x.s.s.s.schedule_datetime.Date >= DateTime.Parse(startDate).AddDays(-1))
                                              && (x.s.s.s.schedule_datetime.Date <= DateTime.Parse(startDate).AddDays(5))
                                              && x.s.s.c.id == classroomId
                                              && x.s.s.c.room_status == "1"
                                              && (string.IsNullOrEmpty(trainingType) ? true : x.s.s.c.room_type == trainingType))
                                      .Select(x => new
                                      {
                                          x.s.s.s.content_name,
                                          x.s.p.plan_name,
                                          x.r.teacher_name,
                                          contentid = x.r.id,
                                          x.s.p.stu_count,
                                          planid = x.s.p.id,
                                          x.s.s.s.schedule_datetime,
                                          x.s.s.s.id
                                      }).ToList();
            List<ScheduleInfo> list = new List<ScheduleInfo>();
            List<ScheduleInfo> list1 = new List<ScheduleInfo>();
            List<ScheduleInfo> list2 = new List<ScheduleInfo>();
            List<ScheduleInfo> list3 = new List<ScheduleInfo>();
            List<ScheduleInfo> list4 = new List<ScheduleInfo>();
            List<ScheduleInfo> list5 = new List<ScheduleInfo>();
            List<ScheduleInfo> list6 = new List<ScheduleInfo>();
            List<ScheduleInfo> list7 = new List<ScheduleInfo>();
            foreach (var item in queryList)
            {
                bool isSelected = false;
                if (planID == item.planid)
                    isSelected = true;
                if (planID == 0)
                    isSelected = true;
                list.Add(new ScheduleInfo
                {
                    ID = item.id,
                    PlanName = item.plan_name,
                    ScheduleDatetime = item.schedule_datetime.ToString(),
                    TrainingScheduleDatetime = item.schedule_datetime,
                    Scheduletime = item.schedule_datetime.Hour.ToString(),
                    ContentName = item.content_name,
                    StuCount = item.stu_count,
                    TeacherName = item.teacher_name,
                    IsSelected = isSelected,
                    ContentID = item.contentid
                });
            }
            list1.AddRange(list.FindAll(x => x.TrainingScheduleDatetime.Date == DateTime.Parse(startDate).AddDays(-1)).OrderBy(x => x.TrainingScheduleDatetime).ToList());
            list2.AddRange(list.FindAll(x => x.TrainingScheduleDatetime.Date == DateTime.Parse(startDate).AddDays(0)).OrderBy(x => x.TrainingScheduleDatetime).ToList());
            list3.AddRange(list.FindAll(x => x.TrainingScheduleDatetime.Date == DateTime.Parse(startDate).AddDays(1)).OrderBy(x => x.TrainingScheduleDatetime).ToList());
            list4.AddRange(list.FindAll(x => x.TrainingScheduleDatetime.Date == DateTime.Parse(startDate).AddDays(2)).OrderBy(x => x.TrainingScheduleDatetime).ToList());
            list5.AddRange(list.FindAll(x => x.TrainingScheduleDatetime.Date == DateTime.Parse(startDate).AddDays(3)).OrderBy(x => x.TrainingScheduleDatetime).ToList());
            list6.AddRange(list.FindAll(x => x.TrainingScheduleDatetime.Date == DateTime.Parse(startDate).AddDays(4)).OrderBy(x => x.TrainingScheduleDatetime).ToList());
            list7.AddRange(list.FindAll(x => x.TrainingScheduleDatetime.Date == DateTime.Parse(startDate).AddDays(5)).OrderBy(x => x.TrainingScheduleDatetime).ToList());
            List<List<ScheduleInfo>> resultList = new List<List<ScheduleInfo>>();
            resultList.Add(list1);
            resultList.Add(list2);
            resultList.Add(list3);
            resultList.Add(list4);
            resultList.Add(list5);
            resultList.Add(list6);
            resultList.Add(list7);
            return new
            {
                code = 200,
                result = resultList,
                message = "OK"
            };
        }
        catch (Exception ex)
        {
            string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
            PubMethod.ErrorLog(ex, path);
            return new { code = 400, msg = "Error" };
        }
    }

    public async Task<object> AddTrainingSchedule(pf_training_plan_v1Context db, TrainingSchedule trainingSchedule, TokenModel token)
    {
        try
        {
            //查询培训计划下学生的数量
            var queryPlan = db.t_training_plan.Where(x => x.delete_flag == 0 && x.id == trainingSchedule.PlanID).FirstOrDefault();
            //查看教室的容量
            var queryclassroom = db.t_classroom.Where(x => x.delete_flag == 0 && x.id == trainingSchedule.ClassroomID).FirstOrDefault();
            if (queryPlan.stu_count > queryclassroom.room_capacity)
                return new { code = 400, message = "教室容量不能满足此培训计划中的学员数量！" };

            //判断当前的教室类型（理论、实践）
            if (queryclassroom.room_type == "1" && trainingSchedule.ContentDiv == "2")
                return new { code = 400, message = "教室类型与计划内容不匹配！" };
            else if (queryclassroom.room_type == "2" && trainingSchedule.ContentDiv == "1")
                return new { code = 400, message = "教室类型与计划内容不匹配！" };

            //同一时间段是否有其他教学任务
            if (CheckSameTimeOtherClassroom(db, trainingSchedule.ContentID, trainingSchedule.ScheduleTime) == false)
                return new { code = 400, message = "此时间段，该教员有其他教学任务！" };

            //验证同一时间段同一教室安排多个教学任务
            if (CheckSameClassroomTime(db, trainingSchedule.ClassroomID, trainingSchedule.ScheduleTime) == false)
                return new { code = 400, message = "同一时间段，同一教室不能安排两个教学任务！" };

            //判断同一个学生同一时间段不同教室
            if (CheckSameTimeStuOtherClassroom(db, trainingSchedule.PlanID, trainingSchedule.ScheduleTime) == false)
                return new { code = 400, message = "此时间段，部分学员有其他学习任务！" };

            if (CheckRepeatData(db, trainingSchedule.ClassroomID, trainingSchedule.ScheduleTime) == false)
                return new { code = 400, message = "此时间段有教学安排，不得安排课程" };

            //判断之后的几个小时是否有课程安排
            DateTime startTime = DateTime.Parse(trainingSchedule.ScheduleTime);
            DateTime endTime = DateTime.Parse(trainingSchedule.EndTime);
            var count = (endTime - startTime).Hours;
            List<DateTime> date = new List<DateTime>();
            for (int i = 0; i < count; i++)
            {
                date.Add(DateTime.Parse(trainingSchedule.ScheduleTime).AddHours(i));
            }
            var query = db.t_lesson_schedule.Where(x => x.delete_flag == 0
                                                && x.classroom_id == trainingSchedule.ClassroomID
                                                && date.Contains(x.schedule_datetime)).Count();
            if (query > 0)
                return new { code = 400, message = "此教室在这个时间点之后有其他教学活动！" };

            //查询计划下已经排了多少学时课程
            if (CheckTimeIsBeyond(db, trainingSchedule.ContentID, count) == false)
                return new { code = 400, message = "此计划内容，已达到排课学时上限！" };

            for (int i = 0; i < count; i++)
            {
                t_lesson_schedule schedule = new t_lesson_schedule();
                schedule.classroom_id = trainingSchedule.ClassroomID;
                schedule.schedule_datetime = DateTime.Parse(trainingSchedule.ScheduleTime).AddHours(i);
                schedule.plan_id = trainingSchedule.PlanID;
                schedule.content_id = trainingSchedule.ContentID;
                schedule.content_name = trainingSchedule.ContentName;
                schedule.content_div = trainingSchedule.ContentDiv;
                schedule.create_number = token.userNumber;
                schedule.create_name = token.userName;
                schedule.update_number = token.userNumber;
                db.t_lesson_schedule.Add(schedule);
            }
            await db.SaveChangesAsync();
            return new { code = 200, message = "OK" };
        }
        catch (Exception ex)
        {
            string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
            PubMethod.ErrorLog(ex, path);
            return new { code = 400, msg = "Error" };
        }
    }
    public async Task<object> EditTrainingSchedule(pf_training_plan_v1Context db, TrainingSchedule trainingSchedule, TokenModel token)
    {
        try
        {
            //查询培训计划下学生的数量
            var queryPlan = db.t_training_plan.Where(x => x.delete_flag == 0 && x.id == trainingSchedule.PlanID).FirstOrDefault();
            //查看教室的容量
            var queryclassroom = db.t_classroom.Where(x => x.delete_flag == 0 && x.id == trainingSchedule.ClassroomID).FirstOrDefault();
            if (queryPlan.stu_count > queryclassroom.room_capacity)
                return new { code = 400, message = "教室容量不能满足此培训计划中的学员数量！" };

            //判断当前的教室类型（理论、实践）
            if (queryclassroom.room_type == "1" && trainingSchedule.ContentDiv == "2")
                return new { code = 400, message = "教室类型与计划内容不匹配！" };
            else if (queryclassroom.room_type == "2" && (trainingSchedule.ContentDiv == "1" || trainingSchedule.ContentDiv == "3"))
                return new { code = 400, message = "教室类型与计划内容不匹配！" };

            //同一时间段是否有其他教学任务
            if (CheckSameTimeOtherClassroom(db, trainingSchedule.ContentID, trainingSchedule.ScheduleTime) == false)
                return new { code = 400, message = "此时间段，该教员有其他教学任务！" };

            //验证同一时间段同一教室安排多个教学任务
            if (CheckSameClassroomTime(db, trainingSchedule.ClassroomID, trainingSchedule.ScheduleTime) == false)
                return new { code = 400, message = "同一时间段，同一教室不能安排两个教学任务！" };

            //判断同一个学生同一时间段不同教室
            if (CheckSameTimeStuOtherClassroom(db, trainingSchedule.PlanID, trainingSchedule.ScheduleTime) == false)
                return new { code = 400, message = "此时间段，部分学员有其他学习任务！" };

            //判断之后的几个小时是否有课程安排
            DateTime startTime = DateTime.Parse(trainingSchedule.ScheduleTime);
            DateTime endTime = DateTime.Parse(trainingSchedule.EndTime);
            var count = (endTime - startTime).Hours;
            List<DateTime> date = new List<DateTime>();
            for (int i = 0; i < count; i++)
            {
                date.Add(DateTime.Parse(trainingSchedule.ScheduleTime).AddHours(i));
            }
            var query = db.t_lesson_schedule.Where(x => x.delete_flag == 0
                                                && x.classroom_id == trainingSchedule.ClassroomID
                                                && date.Contains(x.schedule_datetime)).Count();
            if (query > 0)
                return new { code = 400, message = "此教室在这个时间点之后有其他教学活动！" };

            //查询计划下已经排了多少学时课程
            if (CheckTimeIsBeyond(db, trainingSchedule.ContentID, count) == false)
                return new { code = 400, message = "此计划内容，已达到排课学时上限！" };

            var queryTrainingSchedule = db.t_lesson_schedule.Where(x => x.delete_flag == 0 && x.id == trainingSchedule.ID).FirstOrDefault();
            queryTrainingSchedule.schedule_datetime = DateTime.Parse(trainingSchedule.ScheduleTime);
            queryTrainingSchedule.plan_id = trainingSchedule.PlanID;
            queryTrainingSchedule.content_id = trainingSchedule.ContentID;
            queryTrainingSchedule.content_name = trainingSchedule.ContentName;
            queryTrainingSchedule.content_div = trainingSchedule.ContentDiv;
            queryTrainingSchedule.create_number = token.userNumber;
            queryTrainingSchedule.create_name = token.userName;
            queryTrainingSchedule.update_number = token.userNumber;
            await db.SaveChangesAsync();
            return new { code = 200, message = "OK" };
        }
        catch (Exception ex)
        {
            string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
            PubMethod.ErrorLog(ex, path);
            return new { code = 400, msg = "Error" };
        }
    }
    private bool CheckRepeatData(pf_training_plan_v1Context db, long classroomId, string startTime)
    {
        try
        {
            var query = db.t_lesson_schedule.Where(x => x.delete_flag == 0 && x.classroom_id == classroomId && x.schedule_datetime == DateTime.Parse(startTime)).Count();
            if (query > 0)
                return false;
            else
                return true;
        }
        catch (Exception ex)
        {
            string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
            PubMethod.ErrorLog(ex, path);
            return false;
        }
    }
    private bool CheckTimeIsBeyond(pf_training_plan_v1Context db, long contentId, int count)
    {
        try
        {
            //已经排的课时
            var queryLearningTime = db.t_lesson_schedule.Where(x => x.delete_flag == 0 && x.content_id == contentId).Count() + count;
            var queryContent = db.t_plan_course_task_exam_ref.Where(x => x.id == contentId).FirstOrDefault();
            string dif = queryContent.dif;
            if (dif == "1")//课程
            {
                var queryCourse = db.t_course.Where(x => x.id == queryContent.content_id).FirstOrDefault();
                var learningTime = queryCourse.learning_time;
                if (queryLearningTime == 1 && learningTime < 1)
                    return true;
                if (queryLearningTime > learningTime)
                    return false;
                else
                    return true;
            }
            else if (dif == "2")//任务
            {
                var queryTask = db.t_task_bus.Where(x => x.id == contentId).FirstOrDefault();
                var learningTime = queryTask.class_hour;
                if (queryLearningTime == 1 && learningTime < 1)
                    return true;
                if (queryLearningTime > learningTime)
                    return false;
                else
                    return true;
            }
            else//考试
            {
                var queryExam = db.t_examination_manage.Where(x => x.id == contentId).FirstOrDefault();
                var time = queryExam.exam_duration;
                if (queryLearningTime == 1 && time < 1)
                    return true;
                if (queryLearningTime > time)
                    return false;
                else
                    return true;
            }
        }
        catch (Exception ex)
        {
            string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
            PubMethod.ErrorLog(ex, path);
            return false;
        }
    }

    /// <summary>
    /// 验证同时间同一教员不同教室的问题
    /// </summary>
    /// <param name="db"></param>
    /// <param name="contentId"></param>
    /// <param name="startTime"></param>
    /// <param name="techerNum"></param>
    /// <returns></returns>
    private bool CheckSameTimeOtherClassroom(pf_training_plan_v1Context db, long contentId, string startTime)
    {
        try
        {
            var queryTeacherNum = db.t_plan_course_task_exam_ref.Where(x => x.id == contentId).Select(x => x.teacher_num).FirstOrDefault();
            if (string.IsNullOrEmpty(queryTeacherNum))
                return true;
            var queryTeacher = (from s in db.t_lesson_schedule
                                join c in db.t_plan_course_task_exam_ref on s.content_id equals c.id
                                where s.schedule_datetime == DateTime.Parse(startTime) && c.teacher_num == queryTeacherNum && s.delete_flag == 0
                                select s).Count();
            if (queryTeacher == 0)
                return true;
            else
                return false;
        }
        catch (Exception ex)
        {
            string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
            PubMethod.ErrorLog(ex, path);
            return false;
        }
    }
    /// <summary>
    /// 判断同一个学生同一时间段不同教室
    /// </summary>
    /// <param name="db"></param>
    /// <param name="planId"></param>
    /// <param name="startTime"></param>
    /// <returns></returns>
    private bool CheckSameTimeStuOtherClassroom(pf_training_plan_v1Context db, long planId, string startTime)
    {
        try
        {
            //查找这个计划下的学生
            var queryThisPlanStuList = db.t_trainingplan_stu.Where(x => x.delete_flag == 0 && x.trainingplan_id == planId).Select(x => x.user_number).ToList();
            //查找这个时间段的所有培训计划的ID集合
            var querySameTimePlanIdList = db.t_lesson_schedule.Where(x => x.delete_flag == 0 && x.schedule_datetime == DateTime.Parse(startTime)).Select(x => x.plan_id).Distinct().ToList();
            //查找培训计划下所有学生
            var queryOtherPlanStuList = db.t_trainingplan_stu.Where(x => x.delete_flag == 0 && querySameTimePlanIdList.Contains(x.trainingplan_id)).Select(x => x.user_number).Distinct().ToList();
            //判断集合的交集
            if (queryThisPlanStuList.Intersect(queryOtherPlanStuList).Count() > 0)
                return false;
            else
                return true;
        }
        catch (Exception ex)
        {
            string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
            PubMethod.ErrorLog(ex, path);
            return false;
        }
    }

    /// <summary>
    /// 验证同一时间段同一教室安排多个教学任务
    /// </summary>
    /// <param name="db"></param>
    /// <param name="classroomId"></param>
    /// <param name="startTime"></param>
    /// <returns></returns>
    private bool CheckSameClassroomTime(pf_training_plan_v1Context db, long classroomId, string startTime)
    {
        try
        {
            var queryTrainingSchedule = db.t_lesson_schedule.Where(x => x.delete_flag == 0 && x.classroom_id == classroomId && x.schedule_datetime == DateTime.Parse(startTime)).Count();
            if (queryTrainingSchedule > 0)
                return false;
            else
                return true;
        }
        catch (Exception ex)
        {
            string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
            PubMethod.ErrorLog(ex, path);
            return false;
        }
    }

    public async Task<object> RemoveTrainingSchedule(pf_training_plan_v1Context db, TokenModel token, List<long> list)
    {
        try
        {
            var queryTrainScheduleList = db.t_lesson_schedule.Where(x => x.delete_flag == 0 && list.Contains(x.id)).ToList();
            foreach (var item in queryTrainScheduleList)
            {
                item.delete_flag = 1;
            }
            await db.SaveChangesAsync();
            return new { code = 200, message = "OK" };
        }
        catch (Exception ex)
        {
            string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
            PubMethod.ErrorLog(ex, path);
            return new { code = 400, msg = "Error" };
        }
    }

    public object GetPlanContentById(pf_training_plan_v1Context db, long planId)
    {
        try
        {
            List<PlanContent> contents = new List<PlanContent>();
            var queryPlanCourse = db.t_plan_course_task_exam_ref.Join(db.t_course, r => r.content_id, c => c.id, (r, c) => new { r, c }).Where(x => x.r.delete_flag == 0 && x.r.plan_id == planId && x.r.dif == "1").Select(x => new { x.r.id, x.r.teacher_name, x.c.course_name, contentdiv = "1" }).ToList();
            foreach (var item in queryPlanCourse)
            {
                contents.Add(new PlanContent
                {
                    ID = item.id,
                    ConditionName = item.course_name,
                    TeacherName = item.teacher_name,
                    Dif = item.contentdiv
                });
            }

            var queryPlanTask = db.t_plan_course_task_exam_ref.Join(db.t_task_bus, r => r.content_id, c => c.id, (r, c) => new { r, c }).Where(x => x.r.delete_flag == 0 && x.r.plan_id == planId && x.r.dif == "2").Select(x => new { x.r.id, x.r.teacher_name, x.c.task_name, contentdiv = "2" }).ToList();
            foreach (var item in queryPlanTask)
            {
                contents.Add(new PlanContent
                {
                    ID = item.id,
                    ConditionName = item.task_name,
                    TeacherName = item.teacher_name,
                    Dif = item.contentdiv
                });
            }

            var queryPlanExam = db.t_plan_course_task_exam_ref.Join(db.t_examination_manage, r => r.content_id, c => c.id, (r, c) => new { r, c }).Where(x => x.r.delete_flag == 0 && x.r.plan_id == planId && x.r.dif == "3").Select(x => new { x.r.id, x.r.teacher_name, x.c.exam_name, contentdiv = x.c.exam_div }).ToList();
            foreach (var item in queryPlanExam)
            {
                contents.Add(new PlanContent
                {
                    ID = item.id,
                    ConditionName = item.exam_name,
                    TeacherName = item.teacher_name,
                    Dif = item.contentdiv
                });
            }
            return new { code = 200, result = contents, message = "OK" };
        }
        catch (Exception ex)
        {
            string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
            PubMethod.ErrorLog(ex, path);
            return new { code = 400, msg = "Error" };
        }
    }

    public async Task<object> InitizeData(pf_training_plan_v1Context db)
    {
        try
        {
            string strDate = "";
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 24; j++)
                {
                    t_lesson_schedule schedule = new t_lesson_schedule();
                    schedule.classroom_id = 1;
                    schedule.plan_id = 1;
                    schedule.content_id = 1;
                    string date = (i + 1).ToString().PadLeft(2, '0');
                    string time = j.ToString().PadLeft(2, '0');
                    strDate = "2020-06-" + date + " " + time + ":00:00";
                    schedule.schedule_datetime = DateTime.Parse(strDate);
                    db.Add(schedule);
                }
            }
            await db.SaveChangesAsync();
            return new { code = 200 };
        }
        catch (Exception)
        {

            throw;
        }
    }

    public object AutoArrangeSchedule(pf_training_plan_v1Context db, long planId)
    {
        try
        {
            //自动排某一个培训计划
            if (planId != 0)
            {
                List<ScheduleInfo> trainingSchedule = new List<ScheduleInfo>();

                //查询计划信息
                var queryPlan = db.t_training_plan.Where(x => x.delete_flag == 0 && x.id == planId).SingleOrDefault();
                if (queryPlan.plan_status == "3")//已结束
                    return new { code = 400, message = "不得对已结束的培训计划做自动排课操作！" };

                List<ScheduleInfo> theoryList = GetTheorySchedule(db, planId, queryPlan);
                trainingSchedule.AddRange(theoryList);

                List<ScheduleInfo> taskList = GetTaskSchedule(db, planId, queryPlan, trainingSchedule.Count);
                trainingSchedule.AddRange(taskList);

                List<ScheduleInfo> examList = GetExamSchedule(db, planId, queryPlan, trainingSchedule.Count);
                trainingSchedule.AddRange(examList);

                //查找数据库已经存在的课程
                List<TrainingSchedule> schedules = GetTrainingSchedules(db, planId);
                return new { queryPlan, trainingSchedule };
            }
            return null;
        }
        catch (Exception ex)
        {
            string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
            PubMethod.ErrorLog(ex, path);
            return new { code = 400, msg = "Error" };
        }
    }

    private void GenerateSchedule(pf_training_plan_v1Context db, List<ScheduleInfo> trainingSchedule, int stuCount)
    {
        var a = trainingSchedule.FindAll(x => x.ContentDiv == "1");
        var classroomID = GetClassrooms(db, "1", stuCount);
    }

    private List<TrainingSchedule> GetTrainingSchedules(pf_training_plan_v1Context db, long planId)
    {
        try
        {
            //将数据库数据读出来
            List<TrainingSchedule> schedules = new List<TrainingSchedule>();
            var querySchedule = db.t_lesson_schedule.Where(x => x.delete_flag == 0 && x.plan_id == planId).ToList();
            for (int i = 0; i < querySchedule.Count; i++)
            {
                schedules.Add(new TrainingSchedule
                {
                    ID = querySchedule[i].id,
                    PlanID = (long)querySchedule[i].plan_id,
                    ContentID = (long)querySchedule[i].content_id,
                    ClassroomID = (long)querySchedule[i].classroom_id,
                    ScheduleTime = querySchedule[i].schedule_datetime.ToString(),
                    ContentName = querySchedule[i].content_name,
                    ContentDiv = querySchedule[i].content_div
                });
            }
            return schedules;
        }
        catch (Exception)
        {
            return new List<TrainingSchedule>();
        }
    }
    private List<long> GetClassrooms(pf_training_plan_v1Context db, string div, int stuCount)
    {
        var classRoomsID = db.t_classroom.Where(x => x.delete_flag == 0 && x.room_status == "1" && x.room_type == div && x.room_capacity >= stuCount).Select(x => x.id).ToList();
        return classRoomsID;
    }
    private List<ScheduleInfo> GetTheorySchedule(pf_training_plan_v1Context db, long planId, t_training_plan queryPlan)
    {
        try
        {
            List<ScheduleInfo> trainingSchedule = new List<ScheduleInfo>();
            var queryPlanContent = db.t_plan_course_task_exam_ref.Join(db.t_course, r => r.content_id, c => c.id, (r, c) => new { r, c }).Where(x => x.r.plan_id == planId && x.r.dif == "1" && x.r.delete_flag == 0).ToList();
            for (int i = 0; i < queryPlanContent.Count; i++)
            {
                int CellCount = 0;
                if (queryPlanContent[i].c.learning_time % 1 == 0)
                    CellCount = decimal.ToInt16(queryPlanContent[i].c.learning_time / 1);
                else
                    CellCount = decimal.ToInt16(queryPlanContent[i].c.learning_time / 1) + 1;
                //扣减已经安排在课程表里的
                var querySchedule = db.t_lesson_schedule.Where(x => x.delete_flag == 0 && x.plan_id == planId && x.content_id == queryPlanContent[i].r.id).Count();

                for (int j = 0; j < CellCount - querySchedule; j++)
                {
                    trainingSchedule.Add(new ScheduleInfo
                    {
                        ID = j + 1,
                        PlanID = (long)queryPlanContent[i].r.plan_id,
                        PlanName = queryPlan.plan_name,
                        ContentID = queryPlanContent[i].r.id,
                        ContentName = queryPlanContent[i].c.course_name,
                        StuCount = queryPlan.stu_count,
                        TeacherName = queryPlanContent[i].r.teacher_name,
                        ContentDiv = "1"
                    });
                }
            }
            return trainingSchedule;
        }
        catch (Exception)
        {
            return new List<ScheduleInfo>();
        }
    }

    private List<ScheduleInfo> GetTaskSchedule(pf_training_plan_v1Context db, long planId, t_training_plan queryPlan, int listCount)
    {
        try
        {
            List<ScheduleInfo> trainingSchedule = new List<ScheduleInfo>();
            var queryPlanContent = db.t_plan_course_task_exam_ref.Join(db.t_task_bus, r => r.content_id, t => t.id, (r, t) => new { r, t }).Where(x => x.r.plan_id == planId && x.r.dif == "2" && x.r.delete_flag == 0).ToList();
            for (int i = 0; i < queryPlanContent.Count; i++)
            {
                int CellCount = 0;
                if (queryPlanContent[i].t.class_hour % 1 == 0)
                    CellCount = (int)queryPlanContent[i].t.class_hour / 1;
                else
                    CellCount = ((int)queryPlanContent[i].t.class_hour / 1) + 1;
                //扣减已经安排在课程表里的
                var querySchedule = db.t_lesson_schedule.Where(x => x.delete_flag == 0 && x.plan_id == planId && x.content_id == queryPlanContent[i].r.id).Count();
                for (int j = 0; j < CellCount - querySchedule; j++)
                {
                    trainingSchedule.Add(new ScheduleInfo
                    {
                        ID = ++listCount,
                        PlanID = (long)queryPlanContent[i].r.plan_id,
                        PlanName = queryPlan.plan_name,
                        ContentID = queryPlanContent[i].r.id,
                        ContentName = queryPlanContent[i].t.task_name,
                        StuCount = queryPlan.stu_count,
                        TeacherName = queryPlanContent[i].r.teacher_name,
                        ContentDiv = "2"
                    });
                }
            }
            return trainingSchedule;
        }
        catch (Exception)
        {
            return new List<ScheduleInfo>();
        }
    }

    private List<ScheduleInfo> GetExamSchedule(pf_training_plan_v1Context db, long planId, t_training_plan queryPlan, int listCount)
    {
        try
        {
            List<ScheduleInfo> trainingSchedule = new List<ScheduleInfo>();
            var queryPlanContent = db.t_plan_course_task_exam_ref.Join(db.t_examination_manage, r => r.content_id, e => e.id, (r, e) => new { r, e }).Where(x => x.r.plan_id == planId && x.r.dif == "3" && x.r.delete_flag == 0).ToList();
            for (int i = 0; i < queryPlanContent.Count; i++)
            {
                int CellCount = 0;
                if (queryPlanContent[i].e.exam_duration % 1 == 0)
                    CellCount = (int)queryPlanContent[i].e.exam_duration / 1;
                else
                    CellCount = ((int)queryPlanContent[i].e.exam_duration / 1) + 1;
                //扣减已经安排在课程表里的
                var querySchedule = db.t_lesson_schedule.Where(x => x.delete_flag == 0 && x.plan_id == planId && x.content_id == queryPlanContent[i].r.id).Count();
                for (int j = 0; j < CellCount - querySchedule; j++)
                {
                    trainingSchedule.Add(new ScheduleInfo
                    {
                        ID = ++listCount,
                        PlanID = (long)queryPlanContent[i].r.plan_id,
                        PlanName = queryPlan.plan_name,
                        ContentID = queryPlanContent[i].r.id,
                        ContentName = queryPlanContent[i].e.exam_name,
                        StuCount = queryPlan.stu_count,
                        TeacherName = queryPlanContent[i].r.teacher_name,
                        ContentDiv = "3"
                    });
                }
            }
            return trainingSchedule;
        }
        catch (Exception)
        {
            return new List<ScheduleInfo>();
        }
    }

    #endregion

    #region 培训计划下查询

    public object GetScheduleInfoFromPlan(pf_training_plan_v1Context db, long planID, string trainingType, string startDate)
    {
        try
        {
            var queryList = db.t_classroom.Join(db.t_lesson_schedule, c => c.id, s => s.classroom_id, (c, s) => new { c, s })
                                          .Join(db.t_training_plan, s => s.s.plan_id, p => p.id, (s, p) => new { s, p })
                                          .Join(db.t_plan_course_task_exam_ref, s => s.s.s.content_id, r => r.id, (s, r) => new { s, r })
                                          .Where(x => x.s.s.c.delete_flag == 0
                                              && x.s.s.s.delete_flag == 0
                                              && x.s.p.delete_flag == 0
                                              && x.s.p.id == planID
                                              && x.r.delete_flag == 0
                                              && (x.s.s.s.schedule_datetime.Date >= DateTime.Parse(startDate).AddDays(-1))
                                              && (x.s.s.s.schedule_datetime.Date <= DateTime.Parse(startDate).AddDays(5))
                                              && x.s.s.c.room_status == "1"
                                              && (string.IsNullOrEmpty(trainingType) ? true : x.s.s.c.room_type == trainingType))
                                      .Select(x => new
                                      {
                                          x.s.s.s.content_name,
                                          x.s.p.plan_name,
                                          x.r.teacher_name,
                                          contentid = x.r.id,
                                          x.s.p.stu_count,
                                          planid = x.s.p.id,
                                          x.s.s.s.schedule_datetime,
                                          x.s.s.s.id
                                      }).ToList();
            List<ScheduleInfo> list = new List<ScheduleInfo>();
            List<ScheduleInfo> list1 = new List<ScheduleInfo>();
            List<ScheduleInfo> list2 = new List<ScheduleInfo>();
            List<ScheduleInfo> list3 = new List<ScheduleInfo>();
            List<ScheduleInfo> list4 = new List<ScheduleInfo>();
            List<ScheduleInfo> list5 = new List<ScheduleInfo>();
            List<ScheduleInfo> list6 = new List<ScheduleInfo>();
            List<ScheduleInfo> list7 = new List<ScheduleInfo>();
            foreach (var item in queryList)
            {
                list.Add(new ScheduleInfo
                {
                    ID = item.id,
                    PlanName = item.plan_name,
                    ScheduleDatetime = item.schedule_datetime.ToString(),
                    TrainingScheduleDatetime = item.schedule_datetime,
                    Scheduletime = item.schedule_datetime.Hour.ToString(),
                    ContentName = item.content_name,
                    StuCount = item.stu_count,
                    TeacherName = item.teacher_name,
                    IsSelected = true,
                    ContentID = item.contentid
                });
            }
            list1.AddRange(list.FindAll(x => x.TrainingScheduleDatetime.Date == DateTime.Parse(startDate).AddDays(-1)).OrderBy(x => x.TrainingScheduleDatetime).ToList());
            list2.AddRange(list.FindAll(x => x.TrainingScheduleDatetime.Date == DateTime.Parse(startDate).AddDays(0)).OrderBy(x => x.TrainingScheduleDatetime).ToList());
            list3.AddRange(list.FindAll(x => x.TrainingScheduleDatetime.Date == DateTime.Parse(startDate).AddDays(1)).OrderBy(x => x.TrainingScheduleDatetime).ToList());
            list4.AddRange(list.FindAll(x => x.TrainingScheduleDatetime.Date == DateTime.Parse(startDate).AddDays(2)).OrderBy(x => x.TrainingScheduleDatetime).ToList());
            list5.AddRange(list.FindAll(x => x.TrainingScheduleDatetime.Date == DateTime.Parse(startDate).AddDays(3)).OrderBy(x => x.TrainingScheduleDatetime).ToList());
            list6.AddRange(list.FindAll(x => x.TrainingScheduleDatetime.Date == DateTime.Parse(startDate).AddDays(4)).OrderBy(x => x.TrainingScheduleDatetime).ToList());
            list7.AddRange(list.FindAll(x => x.TrainingScheduleDatetime.Date == DateTime.Parse(startDate).AddDays(5)).OrderBy(x => x.TrainingScheduleDatetime).ToList());
            List<List<ScheduleInfo>> resultList = new List<List<ScheduleInfo>>();
            resultList.Add(list1);
            resultList.Add(list2);
            resultList.Add(list3);
            resultList.Add(list4);
            resultList.Add(list5);
            resultList.Add(list6);
            resultList.Add(list7);
            return new
            {
                code = 200,
                result = resultList,
                message = "OK"
            };
        }
        catch (Exception ex)
        {
            string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
            PubMethod.ErrorLog(ex, path);
            return new { code = 400, msg = "Error" };
        }
    }

    #endregion
}
public class Classroom
{
    public long ID { get; set; }
    public string RoomNumber { get; set; }
    public string RoomName { get; set; }
    public string RoomType { get; set; }
    public int RoomCapacity { get; set; }
    public string RoomFunction { get; set; }
    public string RoomStatus { get; set; }

}
public class ScheduleInfo
{
    public long ID { get; set; }
    public string ScheduleDatetime { get; set; }
    public DateTime TrainingScheduleDatetime { get; set; }
    public string Scheduletime { get; set; }
    public long PlanID { get; set; }
    public string PlanName { get; set; }
    public long ContentID { get; set; }
    public string ContentName { get; set; }
    public string TeacherName { get; set; }
    public string ContentDiv { get; set; }
    public int StuCount { get; set; }
    public bool IsSelected { get; set; }
}
public class TrainingSchedule
{
    public long ID { get; set; }
    public long ClassroomID { get; set; }
    public string ScheduleTime { get; set; }
    public string EndTime { get; set; }
    public long PlanID { get; set; }
    public long ContentID { get; set; }
    public string ContentName { get; set; }
    public string ContentDiv { get; set; }
}

