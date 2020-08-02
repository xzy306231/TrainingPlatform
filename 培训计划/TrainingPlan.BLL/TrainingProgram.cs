using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;


    /// <summary>
    /// 培训大纲
    /// </summary>
    public class TrainingProgram
    {
        //public object GetTrainingProgram(string planeType, string TrainType, string TrainName, int PageIndex, int PageSize)
        //{
        //    try
        //    {
        //        using (var db = new pf_training_plan_v1Context())
        //        {
        //            var query = from t in db.t_training_program
        //                        where t.delete_flag == 0
        //                        && (string.IsNullOrEmpty(planeType) ? true : t.plane_type == planeType)
        //                        && (string.IsNullOrEmpty(TrainType) ? true : t.train_type == TrainType)
        //                        && (string.IsNullOrEmpty(TrainName) ? true : t.train_program_name.Contains(TrainName))
        //                        orderby t.update_time descending
        //                        select new { t.id, t.train_program_name, t.train_type, t.plane_type, t.updateby, t.update_time, t.update_name };

        //            List<Training_Program_Model> list = new List<Training_Program_Model>();
        //            foreach (var item in query.Skip(PageSize * (PageIndex - 1)).Take(PageSize))
        //            {
        //                Training_Program_Model obj = new Training_Program_Model();
        //                obj.ID = item.id;
        //                obj.TrainProgramName = item.train_program_name;
        //                obj.PlaneType = item.plane_type;
        //                obj.TrainType = item.train_type;
        //                obj.UpdateBy = item.updateby;
        //                obj.UpdateTime = item.update_time;
        //                obj.UpdateName = item.update_name;
        //                list.Add(obj);
        //            }

        //            return new { code = 200, Result = list, count = query.Count(), message = "ok" };
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        string path = Path.GetDirectoryName(this.GetType().Assembly.Location) +  @"/ErrorLog/";
        //        PubMethod.ErrorLog(ex, path);
        //        return new { code = 400, msg = "Error" };
        //    }

        //}
        //public object GetTrainingProgramByID(long id)
        //{
        //    try
        //    {
        //        using (var db = new pf_training_plan_v1Context())
        //        {
        //            var query = from t in db.t_training_program
        //                        where t.id == id && t.delete_flag == 0
        //                        select t;
        //            var objModel = query.FirstOrDefault();
        //            if (objModel != null)
        //            {
        //                TrainingProgramModel obj = new TrainingProgramModel();
        //                obj.TrainProgramName = objModel.train_program_name;
        //                obj.PlaneType = objModel.plane_type;
        //                obj.TrainType = objModel.train_type;
        //                obj.RangePurpose = objModel.range_purpose;
        //                obj.PurposeVisibleFlag = objModel.purpose_visible_flag;
        //                obj.Equipment = objModel.equipment;
        //                obj.EquipmentVisibleFlag = objModel.equipment_visible_flag;
        //                obj.QualificationType = objModel.qualification_type;
        //                obj.QualificationVisibleFlag = objModel.qualification_visible_flag;
        //                obj.EndorsementType = objModel.endorsement_type;
        //                obj.PlaneType1 = objModel.plane_type1;
        //                obj.SumDuration = objModel.sum_duration;
        //                obj.TrainTime = objModel.train_time;
        //                obj.UpDownTimes = objModel.up_down_times;
        //                obj.TechnicalGrade = objModel.technical_grade;
        //                obj.OtherRemark = objModel.other_remark;
        //                obj.EnterVisibleFlag = objModel.enter_visible_flag;
        //                obj.ContentQuestion = objModel.content_question;
        //                obj.ContentVisibleFlag = objModel.content_visible_flag;
        //                obj.StandardRequest = objModel.standard_request;
        //                obj.CourseVisibleFlag = objModel.course_visible_flag;
        //                obj.TrainsubjectVisibleFlag = objModel.trainsubject_visible_flag;
        //                obj.RequestVisibleFlag = objModel.request_visible_flag;

        //                //查找课程类信息
        //                var q = from p in db.t_program_course_ref
        //                        join c in db.t_course on p.courseid equals c.id
        //                        where p.delete_flag == 0 && p.programid == id
        //                        select new { c.id, c.course_name, c.course_count };
        //                var q_List = q.ToList();
        //                if (q_List.Count > 0)
        //                {
        //                    List<CourseInfo> list = new List<CourseInfo>();
        //                    for (int i = 0; i < q_List.Count; i++)
        //                    {
        //                        //知识点
        //                        var qq = from k in db.t_course_know_tag
        //                                 join t in db.t_knowledge_tag on k.tag_id equals t.id
        //                                 where k.course_id == q_List[i].id
        //                                 select new { t.tag };
        //                        var qqList = qq.ToList();
        //                        List<string> listTag = new List<string>();
        //                        if (qqList.Count > 0)
        //                        {
        //                            for (int j = 0; j < qqList.Count; j++)
        //                            {
        //                                listTag.Add(qqList[j].tag);
        //                            }
        //                        }

        //                        CourseInfo c = new CourseInfo();
        //                        c.CourseID = q_List[i].id;
        //                        c.CourseName = q_List[i].course_name;
        //                        c.tags = listTag;
        //                        c.CourseCount = q_List[i].course_count;
        //                        list.Add(c);
        //                    }
        //                    obj.CourseInfoList = list;
        //                }

        //                //查找训练科目类信息
        //                var qt = from p in db.t_program_subject_ref
        //                         join ts in db.t_train_subject on p.subjectid equals ts.id
        //                         where p.delete_flag == 0 && p.programid == id
        //                         select new { ts.id,ts.train_number, ts.train_name, ts.train_kind, ts.plane_type, ts.expect_result };
        //                var qt_List = qt.ToList();
        //                if (qt_List.Count > 0)
        //                {
        //                    List<TrainingSubjectInfo> list = new List<TrainingSubjectInfo>();
        //                    for (int i = 0; i < qt_List.Count; i++)
        //                    {
        //                        //知识点
        //                        var qq = from k in db.t_subject_know_tag
        //                                 join t in db.t_knowledge_tag on k.tag_id equals t.id
        //                                 where k.subject_id == qt_List[i].id
        //                                 select new { t.tag };
        //                        var qqList = qq.ToList();
        //                        List<string> listTag = new List<string>();
        //                        if (qqList.Count > 0)
        //                        {
        //                            for (int j = 0; j < qqList.Count; j++)
        //                            {
        //                                listTag.Add(qqList[j].tag);
        //                            }
        //                        }

        //                        TrainingSubjectInfo ts = new TrainingSubjectInfo();
        //                        ts.ID= qt_List[i].id;
        //                        ts.TrainName = qt_List[i].train_name;
        //                        ts.TrainNumber = qt_List[i].train_number;
        //                        ts.TrainKind = qt_List[i].train_kind;
        //                        ts.ExpectResult = qt_List[i].expect_result;
        //                        ts.tags = listTag;
        //                        list.Add(ts);
        //                    }
        //                    obj.TrainingSubjectInfoList = list;
        //                }

        //                return new { code = 200, Result = obj, message = "ok" };
        //            }
        //            else
        //                return new { code = 200, Result = "", message = "ok" };
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        string path = Path.GetDirectoryName(this.GetType().Assembly.Location) +  @"/ErrorLog/";
        //        PubMethod.ErrorLog(ex, path);
        //        return new { code = 400, msg = "Error" };
        //    }

        //}
        //public object Add_TrainingProgram(TrainingProgramModel objModel, TokenModel token)
        //{
        //    try
        //    {
        //        using (var db = new pf_training_plan_v1Context())
        //        {
        //            t_training_program obj = new t_training_program();
        //            obj.train_program_name = objModel.TrainProgramName;
        //            obj.plane_type = objModel.PlaneType;
        //            obj.train_type = objModel.TrainType;
        //            obj.range_purpose = objModel.RangePurpose;
        //            obj.purpose_visible_flag = objModel.PurposeVisibleFlag;
        //            obj.equipment = objModel.Equipment;
        //            obj.equipment_visible_flag = objModel.EquipmentVisibleFlag;
        //            obj.qualification_type = objModel.QualificationType;
        //            obj.qualification_visible_flag = objModel.QualificationVisibleFlag;
        //            obj.endorsement_type = objModel.EndorsementType;
        //            obj.plane_type1 = objModel.PlaneType1;
        //            obj.sum_duration = objModel.SumDuration;
        //            obj.train_time = objModel.TrainTime;
        //            obj.up_down_times = objModel.UpDownTimes;
        //            obj.technical_grade = objModel.TechnicalGrade;
        //            obj.other_remark = objModel.OtherRemark;
        //            obj.enter_visible_flag = objModel.EnterVisibleFlag;
        //            obj.content_question = objModel.ContentQuestion;
        //            obj.content_visible_flag = objModel.ContentVisibleFlag;
        //            obj.standard_request = objModel.StandardRequest;
        //            obj.request_visible_flag = objModel.RequestVisibleFlag;
        //            obj.course_visible_flag = objModel.CourseVisibleFlag;
        //            obj.trainsubject_visible_flag = objModel.TrainsubjectVisibleFlag;
        //            obj.delete_flag = 0;
        //            obj.createby = token.userId;
        //            obj.create_time = DateTime.Now;
        //            obj.update_time = DateTime.Now;
        //            obj.updateby = token.userId;
        //            obj.update_name = token.userName;
        //            db.t_training_program.Add(obj);
        //            int i = db.SaveChanges();
        //            //long MaxProgramID = (from t in db.t_training_program select t.id).Max();
        //            long MaxProgramID = obj.id;
        //            if (objModel.CourseInfoList != null && objModel.CourseInfoList.Count > 0)//判断是否存在课程
        //            {
        //                for (int j = 0; j < objModel.CourseInfoList.Count; j++)
        //                {
        //                    t_program_course_ref pc = new t_program_course_ref();
        //                    pc.programid = MaxProgramID;
        //                    pc.courseid = objModel.CourseInfoList[j].CourseID;
        //                    pc.delete_flag = 0;
        //                    pc.t_create = DateTime.Now;
        //                    pc.t_modified = DateTime.Now;
        //                    db.t_program_course_ref.Add(pc);
        //                }
        //            }
        //            if (objModel.TrainingSubjectInfoList != null && objModel.TrainingSubjectInfoList.Count > 0)//判断是否存在训练科目
        //            {
        //                for (int j = 0; j < objModel.TrainingSubjectInfoList.Count; j++)
        //                {

        //                    t_program_subject_ref ps = new t_program_subject_ref();
        //                    ps.programid = MaxProgramID;
        //                    ps.subjectid = objModel.TrainingSubjectInfoList[j].ID;
        //                    ps.delete_flag = 0;
        //                    ps.t_create = DateTime.Now;
        //                    ps.t_modified = DateTime.Now;
        //                    db.t_program_subject_ref.Add(ps);//写入关系表
        //                                                     // }
        //                }
        //            }
        //            int n = db.SaveChanges() + i;
        //            if (n > 0)
        //            {
        //                SysLogModel syslog = new SysLogModel();
        //                syslog.opNo = token.userNumber;
        //                syslog.opName = token.userName;
        //                syslog.opType = 2;
        //                syslog.moduleName = "培训大纲";
        //                syslog.logDesc = "创建了培训大纲：" + objModel.TrainProgramName;
        //                syslog.logSuccessd = 1;
        //                PubMethod.Log(syslog);
        //                return new { code = 200, result = i, meaaage = "创建成功" };
        //            }
        //            else
        //                return new { code = 200, result = i, meaaage = "创建失败" };
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        string path = Path.GetDirectoryName(this.GetType().Assembly.Location) +  @"/ErrorLog/";
        //        PubMethod.ErrorLog(ex, path);
        //        return new { code = 400, msg = "Error" };
        //    }

        //}
        //public object Update_TrainingProgram(TrainingProgramModel objModel, TokenModel token)
        //{
        //    try
        //    {
        //        using (var db = new pf_training_plan_v1Context())
        //        {
        //            var query = from t in db.t_training_program
        //                        where t.id == objModel.ID && t.delete_flag == 0
        //                        select t;
        //            var q = query.FirstOrDefault();
        //            q.train_program_name = objModel.TrainProgramName;
        //            q.plane_type = objModel.PlaneType;
        //            q.train_type = objModel.TrainType;
        //            q.range_purpose = objModel.RangePurpose;
        //            q.purpose_visible_flag = objModel.PurposeVisibleFlag;
        //            q.equipment = objModel.Equipment;
        //            q.equipment_visible_flag = objModel.EquipmentVisibleFlag;
        //            q.qualification_type = objModel.QualificationType;
        //            q.qualification_visible_flag = objModel.QualificationVisibleFlag;
        //            q.endorsement_type = objModel.EndorsementType;
        //            q.plane_type1 = objModel.PlaneType1;
        //            q.sum_duration = objModel.SumDuration;
        //            q.train_time = objModel.TrainTime;
        //            q.up_down_times = objModel.UpDownTimes;
        //            q.technical_grade = objModel.TechnicalGrade;
        //            q.other_remark = objModel.OtherRemark;
        //            q.enter_visible_flag = objModel.EnterVisibleFlag;
        //            q.content_question = objModel.ContentQuestion;
        //            q.content_visible_flag = objModel.ContentVisibleFlag;
        //            q.course_visible_flag = objModel.CourseVisibleFlag;
        //            q.trainsubject_visible_flag = objModel.TrainsubjectVisibleFlag;
        //            q.standard_request = objModel.StandardRequest;
        //            q.request_visible_flag = objModel.RequestVisibleFlag;
        //            q.delete_flag = 0;
        //            q.update_time = DateTime.Now;
        //            q.updateby = objModel.UpdateBy;
        //            q.update_name = token.userName;

        //            long programid = objModel.ID;
        //            var query_course = from pc in db.t_program_course_ref
        //                               where pc.programid == programid && pc.delete_flag == 0
        //                               select pc;
        //            var query_course_list = query_course.ToList();//删除课程关系表数据
        //            if (query_course_list.Count > 0)
        //            {
        //                for (int j = 0; j < query_course_list.Count; j++)
        //                {
        //                    query_course_list[j].delete_flag = 1;
        //                    query_course_list[j].t_modified = DateTime.Now;
        //                }
        //                //db.SaveChanges();
        //            }

        //            if (objModel.CourseInfoList != null && objModel.CourseInfoList.Count > 0)
        //            {
        //                for (int j = 0; j < objModel.CourseInfoList.Count; j++)
        //                {
        //                    t_program_course_ref pc = new t_program_course_ref();
        //                    pc.courseid = objModel.CourseInfoList[j].CourseID;
        //                    pc.programid = programid;
        //                    pc.t_create = DateTime.Now;
        //                    pc.t_modified = DateTime.Now;
        //                    db.t_program_course_ref.Add(pc);//创建课程关系表数据
        //                }
        //                //db.SaveChanges();
        //            }

        //            var query_subject = from ps in db.t_program_subject_ref
        //                                where ps.programid == programid && ps.delete_flag == 0
        //                                select ps;
        //            var query_subject_list = query_subject.ToList();//删除训练科目关系表中数据
        //            if (query_subject_list.Count > 0)
        //            {
        //                for (int j = 0; j < query_subject_list.Count; j++)
        //                {
        //                    query_subject_list[j].delete_flag = 1;
        //                    query_subject_list[j].t_modified = DateTime.Now;
        //                }
        //                //db.SaveChanges();
        //            }

        //            if (objModel.TrainingSubjectInfoList != null && objModel.TrainingSubjectInfoList.Count > 0)
        //            {
        //                for (int j = 0; j < objModel.TrainingSubjectInfoList.Count; j++)
        //                {
        //                    t_program_subject_ref ps = new t_program_subject_ref();
        //                    ps.programid = programid;
        //                    ps.subjectid = objModel.TrainingSubjectInfoList[j].ID;
        //                    ps.delete_flag = 0;
        //                    ps.t_create = DateTime.Now;
        //                    ps.t_modified = DateTime.Now;
        //                    db.t_program_subject_ref.Add(ps);//写入关系表
        //                                                     // }
        //                }
        //                //db.SaveChanges();
        //            }

        //            int i = db.SaveChanges();
        //            if (i > 0)
        //            {
        //                SysLogModel syslog = new SysLogModel();
        //                syslog.opNo = token.userNumber;
        //                syslog.opName = token.userName;
        //                syslog.opType = 3;
        //                syslog.moduleName = "培训大纲";
        //                syslog.logDesc = "修改了培训大纲：" + objModel.TrainProgramName;
        //                syslog.logSuccessd = 1;
        //                PubMethod.Log(syslog);
        //                return new { code = 200, result = i, meaaage = "修改成功" };
        //            }
        //            else
        //                return new { code = 200, result = i, meaaage = "修改失败" };
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        string path = Path.GetDirectoryName(this.GetType().Assembly.Location) +  @"/ErrorLog/";
        //        PubMethod.ErrorLog(ex, path);
        //        return new { code = 400, msg = "Error" };
        //    }

        //}
        //public object Delete_TrainingProgram(long id, TokenModel token)
        //{
        //    try
        //    {
        //        using (var db = new pf_training_plan_v1Context())
        //        {
        //            var query = from t in db.t_training_program
        //                        where t.id == id
        //                        select t;
        //            var q = query.FirstOrDefault();
        //            q.delete_flag = 1;
        //            int i = db.SaveChanges();
        //            if (i > 0)
        //            {
        //                SysLogModel syslog = new SysLogModel();
        //                syslog.opNo = token.userNumber;
        //                syslog.opName = token.userName;
        //                syslog.opType = 3;
        //                syslog.moduleName = "培训大纲";
        //                syslog.logDesc = "删除了培训大纲：" + q.train_program_name;
        //                syslog.logSuccessd = 1;
        //                PubMethod.Log(syslog);
        //                return new { code = 200, result = i, message = "删除成功" };
        //            }
        //            else
        //                return new { code = 200, result = i, message = "删除失败" };
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        string path = Path.GetDirectoryName(this.GetType().Assembly.Location) +  @"/ErrorLog/";
        //        PubMethod.ErrorLog(ex, path);
        //        return new { code = 400, msg = "Error" };
        //    }

        //}
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
        public List<TrainingSubjectInfo> TrainingSubjectInfoList { get; set; }

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

