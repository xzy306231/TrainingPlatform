using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.IO;
using Course.Model;

namespace Course.BLL
{
    public class TrainingProgram
    {
        public object GetTrainingProgram(pf_course_manage_v1Context db, string planeType, string TrainType, string TrainName, int PageIndex, int PageSize)
        {
            try
            {
                var query = from t in db.t_training_program
                            where t.delete_flag == 0
                            && (string.IsNullOrEmpty(planeType) ? true : t.plane_type == planeType)
                            && (string.IsNullOrEmpty(TrainType) ? true : t.train_type == TrainType)
                            && (string.IsNullOrEmpty(TrainName) ? true : t.train_program_name.Contains(TrainName))
                            orderby t.update_time descending
                            select new { t.id, t.train_program_name, t.train_type, t.plane_type, t.updateby, t.update_time, t.update_name };

                List<Training_Program_Model> list = new List<Training_Program_Model>();
                var queryList = query.Skip(PageSize * (PageIndex - 1)).Take(PageSize).ToList();
                foreach (var item in queryList)
                {
                    Training_Program_Model obj = new Training_Program_Model();
                    obj.ID = item.id;
                    obj.TrainProgramName = item.train_program_name;
                    obj.PlaneType = item.plane_type;
                    obj.TrainType = item.train_type;
                    obj.UpdateBy = item.updateby;
                    obj.UpdateTime = item.update_time;
                    obj.UpdateName = item.update_name;
                    list.Add(obj);
                }
                return new { code = 200, Result = list, count = query.Count(), message = "ok" };

            }
            catch (Exception ex)
            {
                string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
                PubMethod.ErrorLog(ex, path);
                return new { code = 400, msg = "Error" };
            }

        }
        public object GetTrainingProgramByID(pf_course_manage_v1Context db, long id)
        {
            try
            {
                var query = from t in db.t_training_program
                            where t.id == id && t.delete_flag == 0
                            select t;
                var objModel = query.FirstOrDefault();
                if (objModel != null)
                {
                    TrainingProgramModel obj = new TrainingProgramModel();
                    obj.TrainProgramName = objModel.train_program_name;
                    obj.PlaneType = objModel.plane_type;
                    obj.TrainType = objModel.train_type;
                    obj.RangePurpose = objModel.range_purpose;
                    obj.PurposeVisibleFlag = objModel.purpose_visible_flag;
                    obj.Equipment = objModel.equipment;
                    obj.EquipmentVisibleFlag = objModel.equipment_visible_flag;
                    obj.QualificationType = objModel.qualification_type;
                    obj.QualificationVisibleFlag = objModel.qualification_visible_flag;
                    obj.EndorsementType = objModel.endorsement_type;
                    obj.PlaneType1 = objModel.plane_type1;
                    obj.SumDuration = objModel.sum_duration;
                    obj.TrainTime = objModel.train_time;
                    obj.UpDownTimes = objModel.up_down_times;
                    obj.TechnicalGrade = objModel.technical_grade;
                    obj.OtherRemark = objModel.other_remark;
                    obj.EnterVisibleFlag = objModel.enter_visible_flag;
                    obj.ContentQuestion = objModel.content_question;
                    obj.ContentVisibleFlag = objModel.content_visible_flag;
                    obj.StandardRequest = objModel.standard_request;
                    obj.CourseVisibleFlag = objModel.course_visible_flag;
                    obj.TrainsubjectVisibleFlag = objModel.trainsubject_visible_flag;
                    obj.RequestVisibleFlag = objModel.request_visible_flag;

                    //查找课程类信息
                    var q = from p in db.t_program_course_ref
                            join c in db.t_course on p.courseid equals c.id
                            where p.delete_flag == 0 && p.programid == id
                            select new { c.id, c.course_name, c.course_count };
                    var q_List = q.ToList();
                    if (q_List.Count > 0)
                    {
                        List<CourseInfo> list = new List<CourseInfo>();
                        for (int i = 0; i < q_List.Count; i++)
                        {
                            //知识点
                            var qq = from k in db.t_course_know_tag
                                     join t in db.t_knowledge_tag on k.tag_id equals t.id
                                     where k.course_id == q_List[i].id
                                     select new { t.tag };
                            var qqList = qq.ToList();
                            List<string> listTag = new List<string>();
                            if (qqList.Count > 0)
                            {
                                for (int j = 0; j < qqList.Count; j++)
                                {
                                    listTag.Add(qqList[j].tag);
                                }
                            }

                            CourseInfo c = new CourseInfo();
                            c.CourseID = q_List[i].id;
                            c.CourseName = q_List[i].course_name;
                            c.tags = listTag;
                            c.CourseCount = q_List[i].course_count;
                            list.Add(c);
                        }
                        obj.CourseInfoList = list;
                    }

                    var queryTask = (from t in db.t_program_task_ref
                                     join a in db.t_training_task on t.taskid equals a.id
                                     where t.delete_flag == 0 && t.programid == id
                                     select new { a.id, a.task_name, a.task_desc, a.knowledge_tag, a.course_count, a.src_id, a.task_type, a.type_level, a.level, a.airplane_type }).ToList();
                    List<TrainingTaskInfo> listTask = new List<TrainingTaskInfo>();
                    foreach (var item in queryTask)
                    {
                        var querySubject = (from s in db.t_subject
                                            where s.delete_flag == 0 && s.task_id == item.id
                                            select s).ToList();
                        List<TrainingSubjectInfo> subjects = new List<TrainingSubjectInfo>();
                        foreach (var subject in querySubject)
                        {
                            subjects.Add(new TrainingSubjectInfo
                            {
                                ID = subject.id,
                                TrainNumber = subject.subject_number,
                                TrainName = subject.subject_name,
                                TrainDesc = item.task_desc,
                                TrainKind = subject.subject_kind,
                                PlaneType = subject.plane_type,
                                ExpectResult = subject.expect_result
                            });
                        }

                        listTask.Add(new TrainingTaskInfo
                        {
                            ID = item.id,
                            SrcID = (long)item.src_id,
                            TaskName = item.task_name,
                            TaskDesc = item.task_desc,
                            CourseCount = item.course_count,
                            Tag = item.knowledge_tag,
                            TrainingSubjectList = subjects,
                            TypeLevel = item.type_level,
                            TaskType = item.task_type,
                            AirplaneType = item.airplane_type,
                            Level = item.level
                        });


                    }
                    obj.trainingTaskInfos = listTask;
                    return new { code = 200, Result = obj, message = "ok" };
                }
                else
                    return new { code = 200, Result = "", message = "ok" };

            }
            catch (Exception ex)
            {
                string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
                PubMethod.ErrorLog(ex, path);
                return new { code = 400, msg = "Error" };
            }

        }
        public object Add_TrainingProgram(pf_course_manage_v1Context db,RabbitMQClient rabbit, TrainingProgramModel objModel, TokenModel token)
        {
            try
            {
                t_training_program obj = new t_training_program();
                obj.train_program_name = objModel.TrainProgramName;
                obj.plane_type = objModel.PlaneType;
                obj.train_type = objModel.TrainType;
                obj.range_purpose = objModel.RangePurpose;
                obj.purpose_visible_flag = objModel.PurposeVisibleFlag;
                obj.equipment = objModel.Equipment;
                obj.equipment_visible_flag = objModel.EquipmentVisibleFlag;
                obj.qualification_type = objModel.QualificationType;
                obj.qualification_visible_flag = objModel.QualificationVisibleFlag;
                obj.endorsement_type = objModel.EndorsementType;
                obj.plane_type1 = objModel.PlaneType1;
                obj.sum_duration = objModel.SumDuration;
                obj.train_time = objModel.TrainTime;
                obj.up_down_times = objModel.UpDownTimes;
                obj.technical_grade = objModel.TechnicalGrade;
                obj.other_remark = objModel.OtherRemark;
                obj.enter_visible_flag = objModel.EnterVisibleFlag;
                obj.content_question = objModel.ContentQuestion;
                obj.content_visible_flag = objModel.ContentVisibleFlag;
                obj.standard_request = objModel.StandardRequest;
                obj.request_visible_flag = objModel.RequestVisibleFlag;
                obj.course_visible_flag = objModel.CourseVisibleFlag;
                obj.trainsubject_visible_flag = objModel.TrainsubjectVisibleFlag;
                obj.createby = token.userId;
                obj.updateby = token.userId;
                obj.update_name = token.userName;
                db.t_training_program.Add(obj);
                int i = db.SaveChanges();
                long MaxProgramID = obj.id;
                if (objModel.CourseInfoList != null && objModel.CourseInfoList.Count > 0)//判断是否存在课程
                {
                    for (int j = 0; j < objModel.CourseInfoList.Count; j++)
                    {
                        t_program_course_ref pc = new t_program_course_ref();
                        pc.programid = MaxProgramID;
                        pc.courseid = objModel.CourseInfoList[j].CourseID;
                        db.t_program_course_ref.Add(pc);
                    }
                }
                if (objModel.trainingTaskInfos != null && objModel.trainingTaskInfos.Count > 0)//判断是否存在训练任务
                {
                    for (int j = 0; j < objModel.trainingTaskInfos.Count; j++)
                    {
                        var queryTask = (from t in db.t_training_task
                                         where t.delete_flag == 0 && t.src_id == objModel.trainingTaskInfos[j].ID
                                         select t).FirstOrDefault();
                        if (queryTask != null)//存在
                        {
                            //训练任务与大纲关系
                            t_program_task_ref taskRef = new t_program_task_ref();
                            taskRef.programid = MaxProgramID;
                            taskRef.taskid = queryTask.id;
                            db.t_program_task_ref.Add(taskRef);

                            if (objModel.trainingTaskInfos[j].TrainingSubjectList != null && objModel.trainingTaskInfos[j].TrainingSubjectList.Count > 0)
                            {
                                for (int k = 0; k < objModel.trainingTaskInfos[j].TrainingSubjectList.Count; k++)
                                {
                                    t_subject subject = new t_subject();
                                    subject.task_id = queryTask.id;
                                    subject.plane_type = objModel.trainingTaskInfos[j].TrainingSubjectList[k].PlaneType;
                                    subject.subject_desc = objModel.trainingTaskInfos[j].TrainingSubjectList[k].TrainDesc;
                                    subject.subject_kind = objModel.trainingTaskInfos[j].TrainingSubjectList[k].TrainKind;
                                    subject.subject_name = objModel.trainingTaskInfos[j].TrainingSubjectList[k].TrainName;
                                    subject.subject_number = objModel.trainingTaskInfos[j].TrainingSubjectList[k].TrainNumber;
                                    subject.expect_result = objModel.trainingTaskInfos[j].TrainingSubjectList[k].ExpectResult;
                                    db.t_subject.Add(subject);
                                }
                            }
                        }
                        else
                        {
                            t_training_task task = new t_training_task();
                            task.src_id = objModel.trainingTaskInfos[j].ID;
                            task.task_name = objModel.trainingTaskInfos[j].TaskName;
                            task.task_desc = objModel.trainingTaskInfos[j].TaskDesc;
                            task.knowledge_tag = objModel.trainingTaskInfos[j].Tag;
                            task.course_count = objModel.trainingTaskInfos[j].CourseCount;
                            task.task_type = objModel.trainingTaskInfos[j].TaskType;
                            task.type_level = objModel.trainingTaskInfos[j].TypeLevel;
                            task.level = objModel.trainingTaskInfos[j].Level;
                            task.airplane_type = objModel.trainingTaskInfos[j].AirplaneType;
                            db.t_training_task.Add(task);
                            db.SaveChanges();
                            long MaxTaskID = task.id;

                            //建立关系
                            t_program_task_ref taskRef = new t_program_task_ref();
                            taskRef.programid = MaxProgramID;
                            taskRef.taskid = MaxTaskID;
                            db.t_program_task_ref.Add(taskRef);

                            if (objModel.trainingTaskInfos[j].TrainingSubjectList != null && objModel.trainingTaskInfos[j].TrainingSubjectList.Count > 0)
                            {
                                for (int k = 0; k < objModel.trainingTaskInfos[j].TrainingSubjectList.Count; k++)
                                {
                                    t_subject subject = new t_subject();
                                    subject.task_id = MaxTaskID;
                                    subject.plane_type = objModel.trainingTaskInfos[j].TrainingSubjectList[k].PlaneType;
                                    subject.subject_desc = objModel.trainingTaskInfos[j].TrainingSubjectList[k].TrainDesc;
                                    subject.subject_kind = objModel.trainingTaskInfos[j].TrainingSubjectList[k].TrainKind;
                                    subject.subject_name = objModel.trainingTaskInfos[j].TrainingSubjectList[k].TrainName;
                                    subject.subject_number = objModel.trainingTaskInfos[j].TrainingSubjectList[k].TrainNumber;
                                    subject.expect_result = objModel.trainingTaskInfos[j].TrainingSubjectList[k].ExpectResult;
                                    db.t_subject.Add(subject);
                                }
                            }
                        }

                    }
                }

                db.SaveChanges();

                SysLogModel syslog = new SysLogModel();
                syslog.opNo = token.userNumber;
                syslog.opName = token.userName;
                syslog.opType = 2;
                syslog.moduleName = "培训大纲";
                syslog.logDesc = "创建了培训大纲：" + objModel.TrainProgramName;
                syslog.logSuccessd = 1;
                rabbit.LogMsg(syslog);
               // PubMethod.Log(syslog);
                return new { code = 200, result = i, meaaage = "创建成功" };

            }
            catch (Exception ex)
            {
                string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
                PubMethod.ErrorLog(ex, path);
                return new { code = 400, msg = "Error" };
            }

        }
        public object Update_TrainingProgram(pf_course_manage_v1Context db,RabbitMQClient rabbit, TrainingProgramModel objModel, TokenModel token)
        {
            try
            {
                var query = from t in db.t_training_program
                            where t.id == objModel.ID && t.delete_flag == 0
                            select t;
                var q = query.FirstOrDefault();
                q.train_program_name = objModel.TrainProgramName;
                q.plane_type = objModel.PlaneType;
                q.train_type = objModel.TrainType;
                q.range_purpose = objModel.RangePurpose;
                q.purpose_visible_flag = objModel.PurposeVisibleFlag;
                q.equipment = objModel.Equipment;
                q.equipment_visible_flag = objModel.EquipmentVisibleFlag;
                q.qualification_type = objModel.QualificationType;
                q.qualification_visible_flag = objModel.QualificationVisibleFlag;
                q.endorsement_type = objModel.EndorsementType;
                q.plane_type1 = objModel.PlaneType1;
                q.sum_duration = objModel.SumDuration;
                q.train_time = objModel.TrainTime;
                q.up_down_times = objModel.UpDownTimes;
                q.technical_grade = objModel.TechnicalGrade;
                q.other_remark = objModel.OtherRemark;
                q.enter_visible_flag = objModel.EnterVisibleFlag;
                q.content_question = objModel.ContentQuestion;
                q.content_visible_flag = objModel.ContentVisibleFlag;
                q.course_visible_flag = objModel.CourseVisibleFlag;
                q.trainsubject_visible_flag = objModel.TrainsubjectVisibleFlag;
                q.standard_request = objModel.StandardRequest;
                q.request_visible_flag = objModel.RequestVisibleFlag;
                q.delete_flag = 0;
                q.update_time = DateTime.Now;
                q.updateby = objModel.UpdateBy;
                q.update_name = token.userName;

                long programid = objModel.ID;
                var query_course = from pc in db.t_program_course_ref
                                   where pc.programid == programid && pc.delete_flag == 0
                                   select pc;
                var query_course_list = query_course.ToList();//删除课程关系表数据
                if (query_course_list.Count > 0)
                {
                    for (int j = 0; j < query_course_list.Count; j++)
                    {
                        query_course_list[j].delete_flag = 1;
                    }
                }

                if (objModel.CourseInfoList != null && objModel.CourseInfoList.Count > 0)
                {
                    for (int j = 0; j < objModel.CourseInfoList.Count; j++)
                    {
                        t_program_course_ref pc = new t_program_course_ref();
                        pc.courseid = objModel.CourseInfoList[j].CourseID;
                        pc.programid = programid;
                        db.t_program_course_ref.Add(pc);//创建课程关系表数据
                    }
                }

                var query_task = from ps in db.t_program_task_ref
                                 where ps.programid == programid && ps.delete_flag == 0
                                 select ps;
                var query_task_list = query_task.ToList();//删除训练任务关系表中数据
                if (query_task_list.Count > 0)
                {
                    for (int j = 0; j < query_task_list.Count; j++)
                    {
                        query_task_list[j].delete_flag = 1;
                    }
                }

                if (objModel.trainingTaskInfos != null && objModel.trainingTaskInfos.Count > 0)
                {
                    for (int j = 0; j < objModel.trainingTaskInfos.Count; j++)
                    {
                        var queryTask = (from t in db.t_training_task
                                         where t.delete_flag == 0 && t.src_id == objModel.trainingTaskInfos[j].ID
                                         select t).FirstOrDefault();
                        if (queryTask != null)//存在
                        {
                            t_program_task_ref taskRef = new t_program_task_ref();
                            taskRef.programid = programid;
                            taskRef.taskid = queryTask.id;
                            db.t_program_task_ref.Add(taskRef);

                            if (objModel.trainingTaskInfos[j].TrainingSubjectList != null && objModel.trainingTaskInfos[j].TrainingSubjectList.Count > 0)
                            {
                                for (int k = 0; k < objModel.trainingTaskInfos[j].TrainingSubjectList.Count; k++)
                                {
                                    t_subject subject = new t_subject();
                                    subject.task_id = queryTask.id;
                                    subject.plane_type = objModel.trainingTaskInfos[j].TrainingSubjectList[k].PlaneType;
                                    subject.subject_desc = objModel.trainingTaskInfos[j].TrainingSubjectList[k].TrainDesc;
                                    subject.subject_kind = objModel.trainingTaskInfos[j].TrainingSubjectList[k].TrainKind;
                                    subject.subject_name = objModel.trainingTaskInfos[j].TrainingSubjectList[k].TrainName;
                                    subject.subject_number = objModel.trainingTaskInfos[j].TrainingSubjectList[k].TrainNumber;
                                    subject.expect_result = objModel.trainingTaskInfos[j].TrainingSubjectList[k].ExpectResult;
                                    db.t_subject.Add(subject);
                                }
                            }
                        }
                        else
                        {
                            t_training_task task = new t_training_task();
                            task.src_id = objModel.trainingTaskInfos[j].ID;
                            task.task_name = objModel.trainingTaskInfos[j].TaskName;
                            task.task_desc = objModel.trainingTaskInfos[j].TaskDesc;
                            task.knowledge_tag = objModel.trainingTaskInfos[j].Tag;
                            task.course_count = objModel.trainingTaskInfos[j].CourseCount;
                            task.task_type = objModel.trainingTaskInfos[j].TaskType;
                            task.type_level = objModel.trainingTaskInfos[j].TypeLevel;
                            task.level = objModel.trainingTaskInfos[j].Level;
                            task.airplane_type = objModel.trainingTaskInfos[j].AirplaneType;
                            db.t_training_task.Add(task);
                            db.SaveChanges();
                            long MaxTaskID = (from t in db.t_training_task select t.id).Max();

                            //建立关系
                            t_program_task_ref taskRef = new t_program_task_ref();
                            taskRef.programid = programid;
                            taskRef.taskid = MaxTaskID;
                            db.t_program_task_ref.Add(taskRef);

                            if (objModel.trainingTaskInfos[j].TrainingSubjectList != null && objModel.trainingTaskInfos[j].TrainingSubjectList.Count > 0)
                            {
                                for (int k = 0; k < objModel.trainingTaskInfos[j].TrainingSubjectList.Count; k++)
                                {
                                    t_subject subject = new t_subject();
                                    subject.task_id = MaxTaskID;
                                    subject.plane_type = objModel.trainingTaskInfos[j].TrainingSubjectList[k].PlaneType;
                                    subject.subject_desc = objModel.trainingTaskInfos[j].TrainingSubjectList[k].TrainDesc;
                                    subject.subject_kind = objModel.trainingTaskInfos[j].TrainingSubjectList[k].TrainKind;
                                    subject.subject_name = objModel.trainingTaskInfos[j].TrainingSubjectList[k].TrainName;
                                    subject.subject_number = objModel.trainingTaskInfos[j].TrainingSubjectList[k].TrainNumber;
                                    subject.expect_result = objModel.trainingTaskInfos[j].TrainingSubjectList[k].ExpectResult;
                                    db.t_subject.Add(subject);
                                }
                            }
                        }
                    }
                }

                db.SaveChanges();

                SysLogModel syslog = new SysLogModel();
                syslog.opNo = token.userNumber;
                syslog.opName = token.userName;
                syslog.opType = 3;
                syslog.moduleName = "培训大纲";
                syslog.logDesc = "修改了培训大纲：" + objModel.TrainProgramName;
                syslog.logSuccessd = 1;
                rabbit.LogMsg(syslog);
                return new { code = 200, meaaage = "修改成功" };

            }
            catch (Exception ex)
            {
                string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
                PubMethod.ErrorLog(ex, path);
                return new { code = 400, msg = "Error" };
            }

        }
        public object Delete_TrainingProgram(pf_course_manage_v1Context db,RabbitMQClient rabbit, long id, TokenModel token)
        {
            try
            {
                var query = from t in db.t_training_program
                            where t.id == id
                            select t;
                var q = query.FirstOrDefault();
                q.delete_flag = 1;
                int i = db.SaveChanges();
                if (i > 0)
                {
                    SysLogModel syslog = new SysLogModel();
                    syslog.opNo = token.userNumber;
                    syslog.opName = token.userName;
                    syslog.opType = 3;
                    syslog.moduleName = "培训大纲";
                    syslog.logDesc = "删除了培训大纲：" + q.train_program_name;
                    syslog.logSuccessd = 1;
                    rabbit.LogMsg(syslog);
                    return new { code = 200, result = i, message = "删除成功" };
                }
                else
                    return new { code = 200, result = i, message = "删除失败" };
            }
            catch (Exception ex)
            {
                string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
                PubMethod.ErrorLog(ex, path);
                return new { code = 400, msg = "Error" };
            }

        }
    }
    public class Training_Program_Model
    {
        public long ID { get; set; }
        public string TrainProgramName { get; set; }
        public string PlaneType { get; set; }
        public string TrainType { get; set; }
        public long? UpdateBy { get; set; }
        public string UpdateName { get; set; }
        public DateTime? UpdateTime { get; set; }
    }
    /// <summary>
    /// 培训大纲模型
    /// </summary>
    public class TrainingProgramModel
    {
        public long ID { get; set; }
        public string TrainProgramName { get; set; }
        public string PlaneType { get; set; }
        public string TrainType { get; set; }
        public string RangePurpose { get; set; }
        public sbyte? PurposeVisibleFlag { get; set; }
        public string Equipment { get; set; }
        public sbyte? EquipmentVisibleFlag { get; set; }
        public string QualificationType { get; set; }
        public sbyte? QualificationVisibleFlag { get; set; }
        public string EndorsementType { get; set; }
        public string PlaneType1 { get; set; }
        public int? SumDuration { get; set; }
        public int? TrainTime { get; set; }
        public int? UpDownTimes { get; set; }
        public string TechnicalGrade { get; set; }
        public string OtherRemark { get; set; }
        public sbyte? EnterVisibleFlag { get; set; }
        public string ContentQuestion { get; set; }
        public sbyte? ContentVisibleFlag { get; set; }
        public sbyte? CourseVisibleFlag { get; set; }
        public sbyte? TrainsubjectVisibleFlag { get; set; }
        public string StandardRequest { get; set; }
        public sbyte? RequestVisibleFlag { get; set; }
        public long UpdateBy { get; set; }
        public long CreateBy { get; set; }
        public List<CourseInfo> CourseInfoList { get; set; }
        public List<TrainingTaskInfo> trainingTaskInfos { get; set; }
    }

    /// <summary>
    ///课程信息 
    /// </summary>
    public class CourseInfo
    {
        /// <summary>
        /// 课程ID
        /// </summary>
        public long CourseID { get; set; }
        /// <summary>
        /// 课程名称
        /// </summary>
        public string CourseName { get; set; }

        /// <summary>
        /// 课时数
        /// </summary>
        public decimal CourseCount { get; set; }

        public List<string> tags { get; set; }
    }

    /// <summary>
    /// 训练科目信息
    /// </summary>
    public class TrainingSubjectInfo
    {
        /// <summary>
        /// 源数据ID
        /// </summary>
        public long ID { get; set; }
        public string TrainName { get; set; }
        public string TrainDesc { get; set; }
        public string TrainNumber { get; set; }
        public string TrainKind { get; set; }
        public string PlaneType { get; set; }
        public string ExpectResult { get; set; }
        public long CreateBy { get; set; }
        public List<string> tags { get; set; }
    }
    public class TrainingTaskInfo
    {
        public long ID { get; set; }
        public long SrcID { get; set; }
        public string TaskName { get; set; }
        public string TaskDesc { get; set; }
        public string Tag { get; set; }
        public int? CourseCount { get; set; }
        public string TaskType { get; set; }
        public string TypeLevel { get; set; }
        public string Level { get; set; }
        public string AirplaneType { get; set; }
        public List<TrainingSubjectInfo> TrainingSubjectList { get; set; }
    }
}

