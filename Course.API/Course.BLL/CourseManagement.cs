using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.IO;
using Course.DAL;
using System.Threading.Tasks;
using Course.Model;

namespace Course.BLL
{
    public class CourseManagement
    {
        public object GetCourse(QueryCriteria queryCriteria)
        {
            try
            {
                using (var db = new pf_course_manageContext())
                {
                    if (queryCriteria.IsAsc && !string.IsNullOrEmpty(queryCriteria.FieldName))
                    {
                        IQueryable<t_course> query = null;
                        if (queryCriteria.UserID == 0)//全部课程
                        {
                            query = from c in db.t_course
                                    join k in db.t_course_know_tag on c.id equals k.course_id into ck
                                    from _ck in ck.DefaultIfEmpty()

                                    join t in db.t_knowledge_tag on _ck.tag_id equals t.id into ct
                                    from _ct in ct.DefaultIfEmpty()

                                    where c.delete_flag == 0 //&& pp.delete_flag==0
                                                             //&& (queryCriteria.UserID == 0 ? true : c.create_by == queryCriteria.UserID)
                                          && c.approval_status == "3"//只查找审核通过的
                                          && ((string.IsNullOrEmpty(queryCriteria.CourseNameTag) ? true : c.course_name.Contains(queryCriteria.CourseNameTag))
                                          || (string.IsNullOrEmpty(queryCriteria.CourseNameTag) ? true : _ct.tag.Contains(queryCriteria.CourseNameTag)))
                                          && (queryCriteria.TagID == 0 ? true : _ct.src_id == queryCriteria.TagID)
                                    orderby PubMethod.GetPropertyValue(c, queryCriteria.FieldName)
                                    select c;
                        }
                        else//我的课程
                        {
                            query = from c in db.t_course
                                    join k in db.t_course_know_tag on c.id equals k.course_id into ck
                                    from _ck in ck.DefaultIfEmpty()

                                    join t in db.t_knowledge_tag on _ck.tag_id equals t.id into ct
                                    from _ct in ct.DefaultIfEmpty()

                                    where c.delete_flag == 0 //&& pp.delete_flag==0
                                          && (c.create_by == queryCriteria.UserID)
                                          && ((string.IsNullOrEmpty(queryCriteria.CourseNameTag) ? true : c.course_name.Contains(queryCriteria.CourseNameTag))
                                          || (string.IsNullOrEmpty(queryCriteria.CourseNameTag) ? true : _ct.tag.Contains(queryCriteria.CourseNameTag)))
                                          && (queryCriteria.TagID == 0 ? true : _ct.src_id == queryCriteria.TagID)
                                    orderby PubMethod.GetPropertyValue(c, queryCriteria.FieldName)
                                    select c;
                        }

                        var count = query.Distinct().Count();
                        List<t_course> q = query.Distinct().Skip(queryCriteria.PageSize * (queryCriteria.PageIndex - 1)).Take(queryCriteria.PageSize).ToList();
                        if (q.Count > 0)
                        {
                            List<Course_TagList> list = new List<Course_TagList>();
                            for (int i = 0; i < q.Count; i++)
                            {
                                var qq = from k in db.t_course_know_tag
                                         join t in db.t_knowledge_tag on k.tag_id equals t.id
                                         where k.course_id == q[i].id
                                         select new { t.src_id, t.tag };
                                var qqList = qq.ToList();
                                List<Tag> tags = new List<Tag>();
                                if (qqList.Count > 0)
                                {
                                    for (int j = 0; j < qqList.Count; j++)
                                    {
                                        tags.Add(new Tag() { ID = qqList[j].src_id, TagName = qqList[j].tag });
                                    }
                                }
                                list.Add(new Course_TagList
                                {
                                    ID = q[i].id,
                                    CourseName = q[i].course_name,
                                    CourseDesc = q[i].course_desc,
                                    CourseCount = q[i].course_count,
                                    ImagePath = q[i].thumbnail_path,
                                    CreateTime = q[i].create_time,
                                    CreateName = q[i].user_name,
                                    ApprovalStatus = q[i].approval_status,
                                    ApprovalUserNumber = q[i].approval_user_number,
                                    ApprovalRemark = q[i].approval_remarks,
                                    Tag = tags
                                });
                            }
                            return new { code = 200, result = new { count = count, list = list }, message = "OK" };
                        }
                        else
                            return new { code = 200, message = "OK" };
                    }
                    else if (queryCriteria.IsAsc == false && !string.IsNullOrEmpty(queryCriteria.FieldName))
                    {
                        IQueryable<t_course> query = null;
                        if (queryCriteria.UserID == 0)//全部课程
                        {
                            query = from c in db.t_course
                                    join k in db.t_course_know_tag on c.id equals k.course_id into ck
                                    from _ck in ck.DefaultIfEmpty()

                                    join t in db.t_knowledge_tag on _ck.tag_id equals t.id into ct
                                    from _ct in ct.DefaultIfEmpty()

                                    where c.delete_flag == 0
                                            // && (queryCriteria.UserID == 0 ? true : c.create_by == queryCriteria.UserID)
                                            && c.approval_status == "3"//只查找审核通过的
                                          && ((string.IsNullOrEmpty(queryCriteria.CourseNameTag) ? true : c.course_name.Contains(queryCriteria.CourseNameTag))
                                          || (string.IsNullOrEmpty(queryCriteria.CourseNameTag) ? true : _ct.tag.Contains(queryCriteria.CourseNameTag)))
                                          && (queryCriteria.TagID == 0 ? true : _ct.src_id == queryCriteria.TagID)
                                    orderby PubMethod.GetPropertyValue(c, queryCriteria.FieldName) descending
                                    select c;
                        }
                        else//我的课程
                        {
                            query = from c in db.t_course
                                    join k in db.t_course_know_tag on c.id equals k.course_id into ck
                                    from _ck in ck.DefaultIfEmpty()

                                    join t in db.t_knowledge_tag on _ck.tag_id equals t.id into ct
                                    from _ct in ct.DefaultIfEmpty()

                                    where c.delete_flag == 0
                                            && (c.create_by == queryCriteria.UserID)
                                          // && c.approval_status == "3"//只查找审核通过的
                                          && ((string.IsNullOrEmpty(queryCriteria.CourseNameTag) ? true : c.course_name.Contains(queryCriteria.CourseNameTag))
                                          || (string.IsNullOrEmpty(queryCriteria.CourseNameTag) ? true : _ct.tag.Contains(queryCriteria.CourseNameTag)))
                                          && (queryCriteria.TagID == 0 ? true : _ct.src_id == queryCriteria.TagID)
                                    orderby PubMethod.GetPropertyValue(c, queryCriteria.FieldName) descending
                                    select c;
                        }

                        var count = query.Distinct().Count();
                        List<t_course> q = query.Distinct().Skip(queryCriteria.PageSize * (queryCriteria.PageIndex - 1)).Take(queryCriteria.PageSize).ToList();
                        if (q.Count > 0)
                        {
                            List<Course_TagList> list = new List<Course_TagList>();
                            for (int i = 0; i < q.Count; i++)
                            {
                                var qq = from k in db.t_course_know_tag
                                         join t in db.t_knowledge_tag on k.tag_id equals t.id
                                         where k.course_id == q[i].id
                                         select new { t.src_id, t.tag };
                                var qqList = qq.ToList();
                                List<Tag> tags = new List<Tag>();
                                if (qqList.Count > 0)
                                {
                                    for (int j = 0; j < qqList.Count; j++)
                                    {
                                        tags.Add(new Tag() { ID = qqList[j].src_id, TagName = qqList[j].tag });
                                    }
                                }
                                list.Add(new Course_TagList
                                {
                                    ID = q[i].id,
                                    CourseName = q[i].course_name,
                                    CourseDesc = q[i].course_desc,
                                    ImagePath = q[i].thumbnail_path,
                                    CourseCount = q[i].course_count,
                                    CreateTime = q[i].create_time,
                                    CreateName = q[i].user_name,
                                    ApprovalStatus = q[i].approval_status,
                                    ApprovalRemark = q[i].approval_remarks,
                                    ApprovalUserNumber = q[i].approval_user_number,
                                    Tag = tags
                                });
                            }
                            return new { code = 200, result = new { count = count, list = list }, message = "OK" };
                        }
                        else
                            return new { code = 200, message = "OK" };
                    }
                    else
                    {
                        IQueryable<t_course> query = null;
                        if (queryCriteria.UserID == 0)//全部课程
                        {
                            query = from c in db.t_course
                                    join k in db.t_course_know_tag on c.id equals k.course_id into ck
                                    from _ck in ck.DefaultIfEmpty()

                                    join t in db.t_knowledge_tag on _ck.tag_id equals t.id into ct
                                    from _ct in ct.DefaultIfEmpty()

                                    where c.delete_flag == 0 //&& pp.delete_flag==0
                                                             // && (queryCriteria.UserID == 0 ? true : c.create_by == queryCriteria.UserID)
                                         && c.approval_status == "3"
                                          && ((string.IsNullOrEmpty(queryCriteria.CourseNameTag) ? true : c.course_name.Contains(queryCriteria.CourseNameTag))
                                          || (string.IsNullOrEmpty(queryCriteria.CourseNameTag) ? true : _ct.tag.Contains(queryCriteria.CourseNameTag)))
                                          && (queryCriteria.TagID == 0 ? true : _ct.src_id == queryCriteria.TagID)
                                    orderby c.create_time descending
                                    select c;
                        }
                        else//我的课程
                        {
                            query = from c in db.t_course
                                    join k in db.t_course_know_tag on c.id equals k.course_id into ck
                                    from _ck in ck.DefaultIfEmpty()

                                    join t in db.t_knowledge_tag on _ck.tag_id equals t.id into ct
                                    from _ct in ct.DefaultIfEmpty()

                                    where c.delete_flag == 0 //&& pp.delete_flag==0
                                          && (c.create_by == queryCriteria.UserID)
                                          //&& c.approval_status == "3"
                                          && ((string.IsNullOrEmpty(queryCriteria.CourseNameTag) ? true : c.course_name.Contains(queryCriteria.CourseNameTag))
                                          || (string.IsNullOrEmpty(queryCriteria.CourseNameTag) ? true : _ct.tag.Contains(queryCriteria.CourseNameTag)))
                                          && (queryCriteria.TagID == 0 ? true : _ct.src_id == queryCriteria.TagID)
                                    orderby c.create_time descending
                                    select c;
                        }

                        var count = query.Distinct().Count();
                        List<t_course> q = query.Distinct().Skip(queryCriteria.PageSize * (queryCriteria.PageIndex - 1)).Take(queryCriteria.PageSize).ToList();
                        if (q.Count > 0)
                        {
                            List<Course_TagList> list = new List<Course_TagList>();
                            for (int i = 0; i < q.Count; i++)
                            {
                                var qq = from k in db.t_course_know_tag
                                         join t in db.t_knowledge_tag on k.tag_id equals t.id
                                         where k.course_id == q[i].id
                                         select new { t.src_id, t.tag };
                                var qqList = qq.ToList();
                                List<Tag> tags = new List<Tag>();
                                if (qqList.Count > 0)
                                {
                                    for (int j = 0; j < qqList.Count; j++)
                                    {
                                        tags.Add(new Tag() { ID = qqList[j].src_id, TagName = qqList[j].tag });
                                    }
                                }
                                list.Add(new Course_TagList
                                {
                                    ID = q[i].id,
                                    CourseName = q[i].course_name,
                                    CourseDesc = q[i].course_desc,
                                    ImagePath = q[i].thumbnail_path,
                                    CourseCount = q[i].course_count,
                                    CreateTime = q[i].create_time,
                                    CreateName = q[i].user_name,
                                    ApprovalStatus = q[i].approval_status,
                                    ApprovalRemark = q[i].approval_remarks,
                                    ApprovalUserNumber = q[i].approval_user_number,
                                    Tag = tags
                                });
                            }
                            return new { code = 200, result = new { count = count, list = list }, message = "OK" };
                        }
                        else
                            return new { code = 200, message = "OK" };
                    }

                }
            }
            catch (Exception ex)
            {
                string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
                PubMethod.ErrorLog(ex, path);
                return new { code = 400, msg = "Error" };
            }

        }

        public object GetCourseInfoByID(long ID)
        {
            try
            {
                string strFastDFSUrl = PubMethod.ReadConfigJsonData("FastDFSUrl");
                using (var db = new pf_course_manageContext())
                {
                    var query = from c in db.t_course
                                where c.delete_flag == 0 && c.id == ID
                                select c;
                    var q = query.FirstOrDefault();
                    var qq = from k in db.t_course_know_tag
                             join t in db.t_knowledge_tag on k.tag_id equals t.id
                             where k.course_id == ID
                             select new { t.src_id, t.tag };
                    var qqList = qq.ToList();
                    List<Tag> tags = new List<Tag>();
                    if (qqList.Count > 0)
                    {
                        for (int j = 0; j < qqList.Count; j++)
                        {
                            tags.Add(new Tag() { ID = qqList[j].src_id, TagName = qqList[j].tag });
                        }
                    }
                    Course_TagList course_TagList = new Course_TagList();
                    course_TagList.ID = q.id;
                    course_TagList.CourseName = q.course_name;
                    course_TagList.CourseDesc = q.course_desc;
                    course_TagList.CreateName = q.user_name;
                    course_TagList.CourseCount = q.course_count;
                    course_TagList.LearningTime = q.learning_time.ToString();
                    course_TagList.PartPath = q.thumbnail_path;
                    if (string.IsNullOrEmpty(q.thumbnail_path))
                    {
                        course_TagList.ImagePath = "";
                    }
                    else
                    {
                        course_TagList.ImagePath = strFastDFSUrl + q.thumbnail_path;
                    }
                    course_TagList.ApprovalStatus = q.approval_status;
                    course_TagList.ApprovalRemark = q.approval_remarks;
                    course_TagList.CreateTime = q.create_time;
                    course_TagList.Tag = tags;
                    return new { code = 200, result = course_TagList, msg = "OK" };
                }
            }
            catch (Exception ex)
            {
                string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
                PubMethod.ErrorLog(ex, path);
                return new { code = 400, msg = "Error" };
            }

        }

        /// <summary>
        /// 创建课程
        /// </summary>
        /// <param name="course"></param>
        /// <returns></returns>
        public object Add_Course(CourseTag coursetag, TokenModel token)
        {
            try
            {
                using (var db = new pf_course_manageContext())
                {
                    t_course c = new t_course();
                    c.course_name = coursetag.CourseName;
                    c.course_desc = coursetag.CourseDesc;
                    c.course_count = coursetag.CourseCount;
                    c.learning_time = decimal.Parse(coursetag.LearningTime);
                    c.thumbnail_path = coursetag.ImagePath;
                    c.create_by = coursetag.CreateBy;
                    c.user_name = coursetag.CreateName;
                    c.user_number = coursetag.CreateNumber;
                    c.create_time = DateTime.Now;
                    c.update_by = coursetag.CreateBy;
                    c.update_time = DateTime.Now;
                    c.delete_flag = 0;
                    c.publish_flag = 0;
                    c.approval_status = "1";
                    db.t_course.Add(c);
                    int j = db.SaveChanges();

                    long MaxCourseID = (from t in db.t_course select t.id).Max();
                    if (j > 0)
                    {
                        if (coursetag.TagList.Count > 0)
                        {
                            for (int i = 0; i < coursetag.TagList.Count; i++)
                            {
                                var q_kt = from k in db.t_knowledge_tag
                                           where k.src_id == coursetag.TagList[i].ID && k.delete_flag == 0
                                           select k;
                                var qkt = q_kt.FirstOrDefault();
                                if (qkt == null)//副本不存在
                                {
                                    //写入知识库
                                    t_knowledge_tag tag = new t_knowledge_tag();
                                    tag.src_id = coursetag.TagList[i].ID;
                                    tag.tag = coursetag.TagList[i].TagName;
                                    db.t_knowledge_tag.Add(tag);
                                    db.SaveChanges();
                                    long MaxTagID = (from t in db.t_knowledge_tag select t.id).Max();

                                    //写入关系表
                                    t_course_know_tag ct = new t_course_know_tag();
                                    ct.course_id = MaxCourseID;
                                    ct.tag_id = MaxTagID;
                                    db.t_course_know_tag.Add(ct);
                                }
                                else//存在
                                {
                                    //写入关系表
                                    t_course_know_tag ct = new t_course_know_tag();
                                    ct.course_id = MaxCourseID;
                                    ct.tag_id = qkt.id;
                                    db.t_course_know_tag.Add(ct);
                                }

                                // db.SaveChanges();
                            }
                        }
                        if (db.SaveChanges() > 0)
                        {
                            SysLogModel syslog = new SysLogModel();
                            syslog.opNo = token.userNumber;
                            syslog.opName = token.userName;
                            syslog.opType = 2;
                            syslog.moduleName = "课程管理";
                            syslog.logDesc = "添加课程：" + coursetag.CourseName;
                            syslog.logSuccessd = 1;
                            PubMethod.Log(syslog);
                            return new { code = 200, result = MaxCourseID, message = "OK" };
                        }
                        else
                        {
                            SysLogModel syslog = new SysLogModel();
                            syslog.opNo = token.userNumber;
                            syslog.opName = token.userName;
                            syslog.opType = 2;
                            syslog.moduleName = "课程管理";
                            syslog.logDesc = "添加课程：" + coursetag.CourseName;
                            syslog.logSuccessd = 1;
                            PubMethod.Log(syslog);
                            return new { code = 200, result = MaxCourseID, message = "OK" };
                        }
                    }
                    else
                    {
                        SysLogModel syslog = new SysLogModel();
                        syslog.opNo = token.userNumber;
                        syslog.opName = token.userName;
                        syslog.opType = 2;
                        syslog.moduleName = "课程管理";
                        syslog.logDesc = "添加课程：" + coursetag.CourseName;
                        syslog.logSuccessd = 2;
                        PubMethod.Log(syslog);
                        return new { code = 400, message = "Error" };
                    }
                }
            }
            catch (Exception ex)
            {
                string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
                PubMethod.ErrorLog(ex, path);
                return new { code = 400, msg = "Error" };
            }

        }

        /// <summary>
        /// 修改课程信息
        /// </summary>
        /// <param name="id"></param>
        /// <param name="course"></param>
        /// <returns></returns>
        public object Update_Course(CourseTag coursetag, TokenModel token)
        {
            try
            {
                using (var db = new pf_course_manageContext())
                {
                    var query = from c in db.t_course
                                where c.delete_flag == 0 && c.id == coursetag.CourseID
                                select c;
                    var q = query.FirstOrDefault();
                    q.course_name = coursetag.CourseName;
                    q.course_count = coursetag.CourseCount;
                    q.learning_time = decimal.Parse(coursetag.LearningTime);
                    q.course_desc = coursetag.CourseDesc;
                    q.update_by = coursetag.CreateBy;
                    q.thumbnail_path = coursetag.ImagePath;
                    //q.update_name = coursetag.CreateName;
                    q.update_time = DateTime.Now;
                    int j = db.SaveChanges();
                    if (j > 0)
                    {
                        //删除关系表数据
                        var ckt = from ct in db.t_course_know_tag
                                  where ct.course_id == coursetag.CourseID
                                  select ct;
                        if (ckt.ToList().Count > 0)
                        {
                            db.t_course_know_tag.RemoveRange(ckt.ToList());
                        }

                        if (coursetag.TagList.Count > 0)
                        {
                            for (int i = 0; i < coursetag.TagList.Count; i++)
                            {
                                var q_kt = from k in db.t_knowledge_tag
                                           where k.src_id == coursetag.TagList[i].ID && k.delete_flag == 0
                                           select k;
                                var qkt = q_kt.FirstOrDefault();
                                if (qkt == null)//副本不存在
                                {
                                    //写入知识库
                                    t_knowledge_tag tag = new t_knowledge_tag();
                                    tag.src_id = coursetag.TagList[i].ID;
                                    tag.tag = coursetag.TagList[i].TagName;
                                    db.t_knowledge_tag.Add(tag);
                                    db.SaveChanges();
                                    long MaxTagID = (from t in db.t_knowledge_tag select t.id).Max();

                                    //写入关系表
                                    t_course_know_tag ct = new t_course_know_tag();
                                    ct.course_id = coursetag.CourseID;
                                    ct.tag_id = MaxTagID;
                                    db.t_course_know_tag.Add(ct);
                                }
                                else//副本存在
                                {
                                    //写入关系表
                                    t_course_know_tag ct = new t_course_know_tag();
                                    ct.course_id = coursetag.CourseID;
                                    ct.tag_id = qkt.id;
                                    db.t_course_know_tag.Add(ct);
                                }
                            }
                        }

                        if (db.SaveChanges() + j > 0)
                        {
                            SysLogModel syslog = new SysLogModel();
                            syslog.opNo = token.userNumber;
                            syslog.opName = token.userName;
                            syslog.opType = 3;
                            syslog.moduleName = "课程管理";
                            syslog.logDesc = "修改课程：" + coursetag.CourseName;
                            syslog.logSuccessd = 1;
                            PubMethod.Log(syslog);
                            return new { code = 200, message = "OK" };
                        }
                        else
                        {
                            SysLogModel syslog = new SysLogModel();
                            syslog.opNo = token.userNumber;
                            syslog.opName = token.userName;
                            syslog.opType = 3;
                            syslog.moduleName = "课程管理";
                            syslog.logDesc = "修改课程：" + coursetag.CourseName;
                            syslog.logSuccessd = 2;
                            PubMethod.Log(syslog);
                            return new { code = 400, message = "Error" };
                        }
                    }
                    else
                    {
                        SysLogModel syslog = new SysLogModel();
                        syslog.opNo = token.userNumber;
                        syslog.opName = token.userName;
                        syslog.opType = 3;
                        syslog.moduleName = "课程管理";
                        syslog.logDesc = "修改课程：" + coursetag.CourseName;
                        syslog.logSuccessd = 2;
                        PubMethod.Log(syslog);
                        return new { code = 400, message = "Error" };
                    }
                }
            }
            catch (Exception ex)
            {
                string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
                PubMethod.ErrorLog(ex, path);
                return new { code = 400, msg = "Error" };
            }

        }

        /// <summary>
        /// 课程发布(提交审核)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public object Update_CoursePublish(int id, TokenModel token)
        {
            try
            {
                using (var db = new pf_course_manageContext())
                {
                    var query = from c in db.t_course
                                where c.delete_flag == 0 && c.id == id
                                select c;
                    var obj = query.FirstOrDefault();
                    obj.approval_status = "2";
                    int i = db.SaveChanges();
                    if (i > 0)
                    {
                        //审核消息推送
                        MsgToDo model = new MsgToDo();
                        model.todoType = 1;
                        model.commonId = obj.id;
                        model.pubTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        model.msgName = "您有一个新的课程待审核";
                        model.msgBody = "课程名：" + obj.course_name + ",需要您的审核哦！";
                        model.finishFlag = 1;
                        PubMethod.CourseApproval(model);

                        //消息推送
                        Msg msg = new Msg();
                        msg.msgTitle = "审核课程";
                        msg.msgBody = "课程名：" + obj.course_name;
                        PubMethod.Msg(msg);

                        //日志消息产生
                        SysLogModel log = new SysLogModel();
                        log.opNo = token.userNumber;
                        log.opName = token.userName;
                        log.opType = 3;
                        log.moduleName = "课程管理";
                        log.logDesc = "将课程:" + obj.course_name + " 提交了审核课程";
                        log.logSuccessd = 1;
                        PubMethod.Log(log);
                        return new { code = 200, message = "OK" };
                    }
                    else
                    {
                        //日志消息产生
                        SysLogModel log = new SysLogModel();
                        log.opNo = token.userNumber;
                        log.opName = token.userName;
                        log.opType = 3;
                        log.moduleName = "课程管理";
                        log.logDesc = "将课程:" + obj.course_name + " 提交了审核课程";
                        log.logSuccessd = 2;
                        PubMethod.Log(log);
                        return new { code = 400, message = "该课程已发布" };
                    }
                }
            }
            catch (Exception ex)
            {
                string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
                PubMethod.ErrorLog(ex, path);
                return new { code = 400, msg = "Error" };
            }

        }

        /// <summary>
        /// 删除课程
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public object Delete_Course(long id, string strUserNumber, TokenModel token)
        {
            try
            {
                using (var db = new pf_course_manageContext())
                {
                    var query = from c in db.t_course
                                where c.delete_flag == 0 && c.id == id
                                select c;
                    var q = query.FirstOrDefault();
                    q.delete_flag = 1;
                    // q.delete_desc = "已删除";
                    int i = db.SaveChanges();
                    if (i > 0)
                    {
                        SysLogModel syslog = new SysLogModel();
                        syslog.opNo = token.userNumber;
                        syslog.opName = token.userName;
                        syslog.opType = 4;
                        syslog.moduleName = "课程管理";
                        syslog.logDesc = "删除课程：" + q.course_name;
                        syslog.logSuccessd = 1;
                        PubMethod.Log(syslog);
                        return new { code = 200, message = "OK" };
                    }
                    else
                    {
                        SysLogModel syslog = new SysLogModel();
                        syslog.opNo = token.userNumber;
                        syslog.opName = token.userName;
                        syslog.opType = 4;
                        syslog.moduleName = "课程管理";
                        syslog.logDesc = "删除课程：" + q.course_name;
                        syslog.logSuccessd = 2;
                        PubMethod.Log(syslog);
                        return new { code = 400, message = "Error" };
                    }
                }
            }
            catch (Exception ex)
            {
                string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
                PubMethod.ErrorLog(ex, path);
                return new { code = 400, msg = "Error" };
            }

        }

        /// <summary>
        /// 批量删除课程
        /// </summary>
        /// <param name="courseListID"></param>
        /// <returns></returns>
        public object Delete_BatchCourse(CourseList courseList, TokenModel token)
        {
            try
            {
                using (var db = new pf_course_manageContext())
                {
                    string courseName = "";
                    if (courseList.list.Count > 0)
                    {
                        for (int i = 0; i < courseList.list.Count; i++)
                        {
                            var query = from c in db.t_course
                                        where c.delete_flag == 0 && c.id == courseList.list[i]
                                        select c;
                            var q = query.FirstOrDefault();
                            q.delete_flag = 1;
                            courseName = courseName + "," + q.course_name;
                        }
                    }
                    if (db.SaveChanges() > 0)
                    {
                        courseName = courseName.TrimStart(',');
                        SysLogModel syslog = new SysLogModel();
                        syslog.opNo = token.userNumber;
                        syslog.opName = token.userName;
                        syslog.opType = 4;
                        syslog.moduleName = "课程管理";
                        syslog.logDesc = "批量删除了课程:" + courseName;
                        syslog.logSuccessd = 1;
                        PubMethod.Log(syslog);
                        return new { code = 200, message = "OK" };
                    }
                    else
                    {
                        courseName = courseName.TrimStart(',');
                        SysLogModel syslog = new SysLogModel();
                        syslog.opNo = token.userNumber;
                        syslog.opName = token.userName;
                        syslog.opType = 4;
                        syslog.moduleName = "课程管理";
                        syslog.logDesc = "批量删除了课程:" + courseName;
                        syslog.logSuccessd = 2;
                        PubMethod.Log(syslog);
                        return new { code = 400, msg = "Error" };
                    }
                }
            }
            catch (Exception ex)
            {
                string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
                PubMethod.ErrorLog(ex, path);
                return new { code = 400, msg = "Error" };
            }
        }

        /// <summary>
        /// 批量新增课程知识点
        /// </summary>
        /// <param name="batchCourseTag"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public object SetBatchCourseBatchTag(BatchCourseTag batchCourseTag, TokenModel token)
        {
            try
            {
                using (var db = new pf_course_manageContext())
                {
                    string courseName = "";

                    if (batchCourseTag.CourseListID.Count > 0)
                    {
                        for (int i = 0; i < batchCourseTag.CourseListID.Count; i++)
                        {
                            //查找课程
                            var queryCourse = from c in db.t_course
                                              where c.id == batchCourseTag.CourseListID[i]
                                              select c;
                            var queryCourseF = queryCourse.FirstOrDefault();
                            courseName = courseName + "," + queryCourseF.course_name;
                            //删除已存在的关系数据
                            var query = from k in db.t_course_know_tag
                                        where k.course_id == batchCourseTag.CourseListID[i]
                                        select k;
                            foreach (var item in query)
                            {
                                db.t_course_know_tag.Remove(item);
                            }

                            if (batchCourseTag.tags.Count > 0)
                            {
                                for (int j = 0; j < batchCourseTag.tags.Count; j++)
                                {
                                    var tag = from t in db.t_knowledge_tag
                                              where t.delete_flag == 0 && t.src_id == batchCourseTag.tags[j].ID
                                              select new { t.id };
                                    var q_tag = tag.FirstOrDefault();
                                    if (q_tag == null)//写入副本数据库
                                    {
                                        //写入知识库
                                        t_knowledge_tag k = new t_knowledge_tag();
                                        k.src_id = batchCourseTag.tags[j].ID;
                                        k.tag = batchCourseTag.tags[j].TagName;
                                        k.create_by = token.userId;
                                        db.t_knowledge_tag.Add(k);
                                        db.SaveChanges();
                                        long MaxTagID = (from t in db.t_knowledge_tag select t.id).Max();

                                        //写进关系表
                                        t_course_know_tag ct = new t_course_know_tag();
                                        ct.course_id = batchCourseTag.CourseListID[i];
                                        ct.tag_id = MaxTagID;
                                        db.t_course_know_tag.Add(ct);
                                        //db.SaveChanges();
                                    }
                                    else//存在
                                    {
                                        //写进关系表
                                        t_course_know_tag ct = new t_course_know_tag();
                                        ct.course_id = batchCourseTag.CourseListID[i];
                                        ct.tag_id = q_tag.id;
                                        db.t_course_know_tag.Add(ct);
                                        // db.SaveChanges();
                                    }
                                }
                            }
                        }
                    }
                    courseName = courseName.TrimStart(',');
                    if (db.SaveChanges() > 0)
                    {
                        SysLogModel syslog = new SysLogModel();
                        syslog.opNo = token.userNumber;
                        syslog.opName = token.userName;
                        syslog.opType = 2;
                        syslog.moduleName = "课程管理";
                        syslog.logDesc = "课程：" + courseName + "。批量新增了知识点";
                        syslog.logSuccessd = 1;
                        PubMethod.Log(syslog);
                        return new { code = 200, msg = "OK" };
                    }
                    else
                    {
                        SysLogModel syslog = new SysLogModel();
                        syslog.opNo = token.userNumber;
                        syslog.opName = token.userName;
                        syslog.opType = 2;
                        syslog.moduleName = "课程管理";
                        syslog.logDesc = "课程：" + courseName + "。批量新增了知识点";
                        syslog.logSuccessd = 2;
                        PubMethod.Log(syslog);
                        return new { code = 400, msg = "Error" };
                    }
                }
            }
            catch (Exception ex)
            {
                string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
                PubMethod.ErrorLog(ex, path);
                return new { code = 400, msg = "Error" };
            }
        }

        private List<CourseStruct> list;
        /// <summary>
        /// 根据课程ID获取课程结构
        /// </summary>
        /// <param name="courseid"></param>
        /// <returns></returns>
        public object GetCourseStruct(long courseid)
        {
            try
            {
                using (var db = new pf_course_manageContext())
                {
                    list = new List<CourseStruct>();
                    var query = from s in db.t_course_struct
                                join t in db.t_struct_resource on s.id equals t.course_struct_id into st
                                from _st in st.DefaultIfEmpty()
                                join r in db.t_course_resource on _st.course_resouce_id equals r.id into tr
                                from _tr in tr.DefaultIfEmpty()
                                where s.delete_flag == 0 && s.course_id == courseid && s.parent_id == 0
                                orderby s.create_time ascending
                                select new { s, _tr.resource_type, _tr.resource_extension };
                    var temp = query.ToList();
                    if (temp.Count > 0)
                    {
                        for (int i = 0; i < temp.Count; i++)
                        {
                            CourseStruct node = new CourseStruct();
                            node.ID = temp[i].s.id;
                            node.NodeNama = temp[i].s.course_node_name;
                            node.NodeType = temp[i].s.node_type;
                            node.ResourceExtension = temp[i].resource_extension;
                            node.ParentID = temp[i].s.parent_id;
                            node.Sort = temp[i].s.node_sort;
                            node.ResourceType = temp[i].resource_type;
                            node.ResourceCount = temp[i].s.resource_count;
                            GetChildCourseStruct(node);
                            list.Add(node);
                        }
                    }
                    return new { code = 200, result = list, message = "OK" };
                }
            }
            catch (Exception ex)
            {
                string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
                PubMethod.ErrorLog(ex, path);
                return new { code = 400, msg = "Error" };
            }

        }

        private void GetChildCourseStruct(CourseStruct courseStruct)
        {
            try
            {
                using (var db = new pf_course_manageContext())
                {
                    var query = from s in db.t_course_struct
                                join t in db.t_struct_resource on s.id equals t.course_struct_id into st
                                from _st in st.DefaultIfEmpty()
                                join r in db.t_course_resource on _st.course_resouce_id equals r.id into tr
                                from _tr in tr.DefaultIfEmpty()
                                where s.delete_flag == 0 && s.parent_id == courseStruct.ID
                                orderby s.create_time ascending
                                select new { s, _tr.resource_type, _tr.resource_extension };
                    var temp = query.ToList();
                    if (temp.Count > 0)
                    {
                        for (int i = 0; i < temp.Count; i++)
                        {
                            CourseStruct node = new CourseStruct();
                            node.ID = temp[i].s.id;
                            node.NodeNama = temp[i].s.course_node_name;
                            node.NodeType = temp[i].s.node_type;
                            node.ResourceExtension = temp[i].resource_extension;
                            node.ParentID = temp[i].s.parent_id;
                            node.Sort = temp[i].s.node_sort;
                            node.ResourceType = temp[i].resource_type;
                            node.ResourceCount = temp[i].s.resource_count;
                            GetChildCourseStruct(node);
                            courseStruct.Children.Add(node);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
                PubMethod.ErrorLog(ex, path);
                // return new { code = 400, msg = "Error" };
            }

        }

        /// <summary>
        /// 根据课程结构ID查看相应的资源信息
        /// </summary>
        /// <param name="NodeID"></param>
        /// <returns></returns>
        public object ViewCoursewareByStructNodeID(long NodeID)
        {
            try
            {
                using (var db = new pf_course_manageContext())
                {
                    var query = from s in db.t_struct_resource
                                join c in db.t_course_resource on s.course_resouce_id equals c.id into sc
                                from _sc in sc.DefaultIfEmpty()
                                where s.course_struct_id == NodeID
                                select new { _sc.id, _sc.group_name, _sc.resource_url, _sc.resource_type, _sc.resource_name };
                    var q = query.FirstOrDefault();
                    if (q != null)
                    {
                        string str = PubMethod.ReadConfigJsonData("FastDFSUrl");
                        string resource_url = str + "/" + q.group_name + "/" + q.resource_url;
                        string resource_type = q.resource_type;
                        string resource_name = q.resource_name;
                        return new { code = 200, result = new { q.id, resource_url, q.resource_type, q.resource_name }, message = "OK" };
                    }
                    else
                    {
                        return new { code = 200, msg = "OK" };
                    }
                }
            }
            catch (Exception ex)
            {
                string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
                PubMethod.ErrorLog(ex, path);
                return new { code = 400, msg = "Error" };
            }

        }

        /// <summary>
        /// 创建课程结构节点
        /// </summary>
        /// <param name="CourseStruct"></param>
        /// <returns></returns>
        public object Add_CourseStructNode(CourseStructNode CourseStruct, TokenModel token)
        {
            try
            {
                using (var db = new pf_course_manageContext())
                {
                    //long MaxTag = (from t in db.t_course_struct select t.id).Max();
                    t_course_struct node = new t_course_struct();
                    node.parent_id = CourseStruct.ParentID;
                    node.course_id = CourseStruct.CourseID;
                    node.course_node_name = CourseStruct.NodeName;
                    node.node_type = CourseStruct.NodeType;
                    // node.node_sort = (int)MaxTag;
                    node.delete_flag = 0;
                    // node.delete_desc = "未删除";
                    node.create_time = DateTime.Now;
                    node.create_by = CourseStruct.CreateBy;
                    node.update_by = CourseStruct.CreateBy;
                    node.update_time = DateTime.Now;
                    db.t_course_struct.Add(node);
                    int i = db.SaveChanges();
                    if (i > 0)
                    {
                        SysLogModel syslog = new SysLogModel();
                        syslog.opNo = token.userNumber;
                        syslog.opName = token.userName;
                        syslog.opType = 2;
                        syslog.moduleName = "课程管理";
                        syslog.logDesc = "添加课程结构节点:" + CourseStruct.NodeName;
                        syslog.logSuccessd = 1;
                        PubMethod.Log(syslog);
                        return new { code = 200, message = "OK" };
                    }
                    else
                    {
                        SysLogModel syslog = new SysLogModel();
                        syslog.opNo = token.userNumber;
                        syslog.opName = token.userName;
                        syslog.opType = 2;
                        syslog.moduleName = "课程管理";
                        syslog.logDesc = "添加课程结构节点:" + CourseStruct.NodeName;
                        syslog.logSuccessd = 2;
                        PubMethod.Log(syslog);
                        return new { code = 400, message = "Error" };
                    }
                }
            }
            catch (Exception ex)
            {
                string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
                PubMethod.ErrorLog(ex, path);
                return new { code = 400, msg = "Error" };
            }

        }

        /// <summary>
        /// 根据结构ID，修改结构节点
        /// </summary>
        /// <param name="id"></param>
        /// <param name="CourseStruct"></param>
        /// <returns></returns>
        public object Update_CourseStructNode(int id, string NodeName, long UpdateBy, TokenModel token)
        {
            try
            {
                using (var db = new pf_course_manageContext())
                {
                    var query = from o in db.t_course_struct
                                where o.delete_flag == 0 && o.id == id
                                select o;
                    var q = query.FirstOrDefault();
                    q.course_node_name = NodeName;
                    q.update_by = UpdateBy;
                    //q.update_name = CourseStruct.CreateName;
                    q.update_time = DateTime.Now;
                    int i = db.SaveChanges();
                    if (i > 0)
                    {
                        SysLogModel syslog = new SysLogModel();
                        syslog.opNo = token.userNumber;
                        syslog.opName = token.userName;
                        syslog.opType = 3;
                        syslog.moduleName = "课程管理";
                        syslog.logDesc = "修改课程结构节点:" + NodeName;
                        syslog.logSuccessd = 1;
                        PubMethod.Log(syslog);
                        return new { code = 200, message = "OK" };
                    }
                    else
                    {
                        SysLogModel syslog = new SysLogModel();
                        syslog.opNo = token.userNumber;
                        syslog.opName = token.userName;
                        syslog.opType = 3;
                        syslog.moduleName = "课程管理";
                        syslog.logDesc = "修改课程结构节点:" + NodeName;
                        syslog.logSuccessd = 2;
                        PubMethod.Log(syslog);
                        return new { code = 400, message = "Error" };
                    }
                }
            }
            catch (Exception ex)
            {
                string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
                PubMethod.ErrorLog(ex, path);
                return new { code = 400, msg = "Error" };
            }

        }

        /// <summary>
        /// 删除课程结构节点
        /// </summary>
        /// <param name="CourseStructID"></param>
        /// <returns></returns>
        public object Delete_CourseStructNode(int CourseStructID, TokenModel token)
        {
            try
            {
                using (var db = new pf_course_manageContext())
                {
                    var query = from s in db.t_course_struct
                                where s.delete_flag == 0 && s.id == CourseStructID
                                select s;
                    var q = query.FirstOrDefault();
                    q.delete_flag = 1;
                    SubCoureNodeResourceCountByDeleteNode(CourseStructID);
                    //q.delete_desc = "已删除";
                    int i = db.SaveChanges();
                    if (i > 0)
                    {
                        SysLogModel syslog = new SysLogModel();
                        syslog.opNo = token.userNumber;
                        syslog.opName = token.userName;
                        syslog.opType = 4;
                        syslog.moduleName = "课程管理";
                        syslog.logDesc = "删除了课程结构节点，节点名：" + q.course_node_name;
                        syslog.logSuccessd = 1;
                        PubMethod.Log(syslog);
                        return new { code = 200, message = "OK" };
                    }
                    else
                    {
                        SysLogModel syslog = new SysLogModel();
                        syslog.opNo = token.userNumber;
                        syslog.opName = token.userName;
                        syslog.opType = 4;
                        syslog.moduleName = "课程管理";
                        syslog.logDesc = "删除了课程结构节点，节点名：" + q.course_node_name;
                        syslog.logSuccessd = 2;
                        PubMethod.Log(syslog);
                        return new { code = 400, message = "Error" };
                    }
                }
            }
            catch (Exception ex)
            {
                string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
                PubMethod.ErrorLog(ex, path);
                return new { code = 400, msg = "Error" };
            }

        }

        /// <summary>
        /// 创建节点资源
        /// </summary>
        /// <param name="StructReaource"></param>
        /// <returns></returns>
        public object Add_CourseStructResource(StructResource StructReaource, TokenModel token)
        {
            try
            {
                using (var db = new pf_course_manageContext())
                {
                    if (StructReaource.ResourceList.Count > 0)
                    {
                        string resourceName = "";
                        long NewCourseResourceID = 0;
                        for (int i = 0; i < StructReaource.ResourceList.Count; i++)
                        {
                            var query = from cr in db.t_course_resource
                                        where cr.delete_flag == 0 && cr.src_id == StructReaource.ResourceList[i].SrcID
                                        select cr;
                            var q = query.FirstOrDefault();
                            //检测副本是否存在
                            if (q == null)//不存在
                            {
                                //创建课程资源
                                t_course_resource r = new t_course_resource();
                                r.src_id = StructReaource.ResourceList[i].SrcID;
                                r.resource_name = StructReaource.ResourceList[i].ResourceName;
                                r.resource_desc = StructReaource.ResourceList[i].ResourceDesc;
                                r.resource_type = StructReaource.ResourceList[i].ResourceType;
                                r.resource_extension = StructReaource.ResourceList[i].ResourceExtension;
                                r.resource_time = StructReaource.ResourceList[i].ResourceTime;
                                r.resource_url = StructReaource.ResourceList[i].ResourceUrl;
                                r.group_name = StructReaource.ResourceList[i].GroupName;
                                r.resource_confidential = StructReaource.ResourceList[i].ResourceConfidential;
                                // r.thumbnail_path = StructReaource.ResourceList[i].ThumbnailPath;
                                r.delete_flag = 0;
                                r.create_by = StructReaource.ResourceList[i].CreateBy;
                                r.create_time = DateTime.Now;
                                r.update_by = StructReaource.ResourceList[i].CreateBy;
                                r.update_time = DateTime.Now;
                                resourceName = resourceName + "," + StructReaource.ResourceList[i].ResourceName;
                                db.t_course_resource.Add(r);
                                db.SaveChanges();
                                long maxid = (from t in db.t_course_resource select t.id).Max();

                                //创建关系
                                t_struct_resource sr = new t_struct_resource();
                                sr.course_struct_id = StructReaource.NodeID;
                                sr.course_resouce_id = maxid;
                                db.t_struct_resource.Add(sr);
                                NewCourseResourceID = maxid;
                            }
                            else//存在
                            {
                                //创建关系
                                t_struct_resource sr = new t_struct_resource();
                                sr.course_struct_id = StructReaource.NodeID;
                                sr.course_resouce_id = q.id;
                                resourceName = resourceName + "," + q.resource_name;
                                db.t_struct_resource.Add(sr);
                                NewCourseResourceID = q.id;
                            }
                            //更新父节点数量
                            AddCoureNodeResourceCount(StructReaource.NodeID);
                        }
                        resourceName = resourceName.TrimStart(',');
                        if (db.SaveChanges() > 0)
                        {
                            SysLogModel syslog = new SysLogModel();
                            syslog.opNo = token.userNumber;
                            syslog.opName = token.userName;
                            syslog.opType = 2;
                            syslog.moduleName = "课程管理";
                            syslog.logDesc = "课程添加了课程资源："+ resourceName;
                            syslog.logSuccessd = 1;
                            PubMethod.Log(syslog);
                            return new { code = 200, result = NewCourseResourceID, message = "OK" };
                        }
                        else
                        {
                            SysLogModel syslog = new SysLogModel();
                            syslog.opNo = token.userNumber;
                            syslog.opName = token.userName;
                            syslog.opType = 2;
                            syslog.moduleName = "课程管理";
                            syslog.logDesc = "课程添加了课程资源：" + resourceName;
                            syslog.logSuccessd = 2;
                            PubMethod.Log(syslog);
                            return new { code = 400, message = "Error" };
                        }
                    }
                    else
                        return new { code = 400, message = "Error" };
                }
            }
            catch (Exception ex)
            {
                string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
                PubMethod.ErrorLog(ex, path);
                return new { code = 400, msg = "Error" };
            }

        }

        private void AddCoureNodeResourceCount(long nodeId)
        {
            try
            {
                using (var db = new pf_course_manageContext())
                {
                    var queryParent2 = from s in db.t_course_struct
                                       where s.delete_flag == 0 && s.id == nodeId
                                       select s;
                    var queryParentF2 = queryParent2.FirstOrDefault();
                    if (queryParentF2 == null)
                        return;

                    //更新父节点数量（第三级）
                    var queryParent3 = from s in db.t_course_struct
                                       where s.delete_flag == 0 && s.id == queryParentF2.parent_id
                                       select s;
                    var queryParentF3 = queryParent3.FirstOrDefault();
                    if (queryParentF3 == null)
                        return;

                    //父节点数量加一
                    if (queryParentF3.resource_count == null)
                        queryParentF3.resource_count = 1;
                    else
                        queryParentF3.resource_count = queryParentF3.resource_count + 1;

                    //更新父节点数量（第二级）
                    var queryParent4 = from s in db.t_course_struct
                                       where s.delete_flag == 0 && s.id == queryParentF3.parent_id
                                       select s;
                    var queryParentF4 = queryParent4.FirstOrDefault();
                    if (queryParentF4 == null)
                    {
                        db.SaveChanges();
                        return;
                    }
                    //父节点数量加一
                    if (queryParentF4.resource_count == null)
                        queryParentF4.resource_count = 1;
                    else
                        queryParentF4.resource_count = queryParentF4.resource_count + 1;

                    //更新父节点数量（第一级）
                    var queryParent5 = from s in db.t_course_struct
                                       where s.delete_flag == 0 && s.id == queryParentF4.parent_id
                                       select s;
                    var queryParentF5 = queryParent5.FirstOrDefault();
                    if (queryParentF5 == null)
                    {
                        db.SaveChanges();
                        return;
                    }
                    //父节点数量加一
                    if (queryParentF5.resource_count == null)
                        queryParentF5.resource_count = 1;
                    else
                        queryParentF5.resource_count = queryParentF5.resource_count + 1;

                    //更新父节点数量（课程）
                    var queryParent6 = from s in db.t_course_struct
                                       where s.delete_flag == 0 && s.id == queryParentF5.parent_id
                                       select s;
                    var queryParentF6 = queryParent6.FirstOrDefault();
                    if (queryParentF6 == null)
                    {
                        db.SaveChanges();
                        return;
                    }
                    //父节点数量加一
                    if (queryParentF6.resource_count == null)
                        queryParentF6.resource_count = 1;
                    else
                        queryParentF6.resource_count = queryParentF6.resource_count + 1;

                    db.SaveChanges();

                }
            }
            catch (Exception ex)
            {
                string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
                PubMethod.ErrorLog(ex, path);
            }
        }

        public object Delete_StructResource(long StructID, long ResourceID, TokenModel token)
        {
            try
            {
                using (var db = new pf_course_manageContext())
                {
                    var query = from sr in db.t_struct_resource
                                where sr.course_struct_id == StructID && sr.course_resouce_id == ResourceID
                                select sr;
                    var q = query.FirstOrDefault();
                    db.t_struct_resource.Remove(q);
                    //减去节点数量
                    SubCoureNodeResourceCount(StructID);
                    var queryResource = (from r in db.t_course_resource
                                        where r.id == ResourceID
                                        select r).FirstOrDefault();
                    if (db.SaveChanges() > 0)
                    {
                        SysLogModel syslog = new SysLogModel();
                        syslog.opNo = token.userNumber;
                        syslog.opName = token.userName;
                        syslog.opType = 4;
                        syslog.moduleName = "课程管理";
                        syslog.logDesc = "删除了课程结构中的资源："+queryResource.resource_name;
                        syslog.logSuccessd = 1;
                        PubMethod.Log(syslog);
                        return new { code = 200, message = "OK" };
                    }
                    else
                    {
                        SysLogModel syslog = new SysLogModel();
                        syslog.opNo = token.userNumber;
                        syslog.opName = token.userName;
                        syslog.opType = 4;
                        syslog.moduleName = "课程管理";
                        syslog.logDesc = "删除了课程结构中的资源：" + queryResource.resource_name;
                        syslog.logSuccessd = 2;
                        PubMethod.Log(syslog);
                        return new { code = 400, message = "Error" };
                    }
                }
            }
            catch (Exception ex)
            {
                string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
                PubMethod.ErrorLog(ex, path);
                return new { code = 400, msg = "Error" };
            }

        }

        /// <summary>
        /// 删除资源的方式减去父节点资源数量
        /// </summary>
        /// <param name="nodeId"></param>
        private void SubCoureNodeResourceCount(long nodeId)
        {
            try
            {
                using (var db = new pf_course_manageContext())
                {
                    var queryParent2 = from s in db.t_course_struct
                                       where s.delete_flag == 0 && s.id == nodeId
                                       select s;
                    var queryParentF2 = queryParent2.FirstOrDefault();
                    if (queryParentF2 == null)
                        return;

                    //更新父节点数量（第三级）
                    var queryParent3 = from s in db.t_course_struct
                                       where s.delete_flag == 0 && s.id == queryParentF2.parent_id
                                       select s;
                    var queryParentF3 = queryParent3.FirstOrDefault();
                    if (queryParentF3 == null)
                        return;

                    //父节点数量减一
                    if (queryParentF3.resource_count == null)
                        queryParentF3.resource_count = 0;
                    else
                        queryParentF3.resource_count = queryParentF3.resource_count - 1;

                    //更新父节点数量（第二级）
                    var queryParent4 = from s in db.t_course_struct
                                       where s.delete_flag == 0 && s.id == queryParentF3.parent_id
                                       select s;
                    var queryParentF4 = queryParent4.FirstOrDefault();
                    if (queryParentF4 == null)
                    {
                        db.SaveChanges();
                        return;
                    }
                    //父节点数量减一
                    if (queryParentF4.resource_count == null)
                        queryParentF4.resource_count = 0;
                    else
                        queryParentF4.resource_count = queryParentF4.resource_count - 1;

                    //更新父节点数量（第一级）
                    var queryParent5 = from s in db.t_course_struct
                                       where s.delete_flag == 0 && s.id == queryParentF4.parent_id
                                       select s;
                    var queryParentF5 = queryParent5.FirstOrDefault();
                    if (queryParentF5 == null)
                    {
                        db.SaveChanges();
                        return;
                    }
                    //父节点数量减一
                    if (queryParentF5.resource_count == null)
                        queryParentF5.resource_count = 0;
                    else
                        queryParentF5.resource_count = queryParentF5.resource_count - 1;

                    //更新父节点数量（课程）
                    var queryParent6 = from s in db.t_course_struct
                                       where s.delete_flag == 0 && s.id == queryParentF5.parent_id
                                       select s;
                    var queryParentF6 = queryParent6.FirstOrDefault();
                    if (queryParentF6 == null)
                    {
                        db.SaveChanges();
                        return;
                    }
                    //父节点数量减一
                    if (queryParentF6.resource_count == null)
                        queryParentF6.resource_count = 0;
                    else
                        queryParentF6.resource_count = queryParentF6.resource_count - 1;

                    db.SaveChanges();

                }
            }
            catch (Exception ex)
            {
                string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
                PubMethod.ErrorLog(ex, path);
            }
        }

        /// <summary>
        /// 删除节点的方式减去数量
        /// </summary>
        /// <param name="nodeId"></param>
        private void SubCoureNodeResourceCountByDeleteNode(long nodeId)
        {
            try
            {
                using (var db = new pf_course_manageContext())
                {
                    //查找节点下资源的数量
                    int nCount = GetChildResourceCount(nodeId);

                    var queryParent2 = from s in db.t_course_struct
                                       where s.delete_flag == 0 && s.id == nodeId
                                       select s;
                    var queryParentF2 = queryParent2.FirstOrDefault();
                    if (queryParentF2 == null)
                        return;

                    //更新父节点数量（第三级）
                    var queryParent3 = from s in db.t_course_struct
                                       where s.delete_flag == 0 && s.id == queryParentF2.parent_id
                                       select s;
                    var queryParentF3 = queryParent3.FirstOrDefault();
                    if (queryParentF3 == null)
                        return;

                    //父节点数量减一
                    if (queryParentF3.resource_count == null)
                        queryParentF3.resource_count = 0;
                    else
                        queryParentF3.resource_count = queryParentF3.resource_count - nCount;

                    //更新父节点数量（第二级）
                    var queryParent4 = from s in db.t_course_struct
                                       where s.delete_flag == 0 && s.id == queryParentF3.parent_id
                                       select s;
                    var queryParentF4 = queryParent4.FirstOrDefault();
                    if (queryParentF4 == null)
                    {
                        db.SaveChanges();
                        return;
                    }
                    //父节点数量减一
                    if (queryParentF4.resource_count == null)
                        queryParentF4.resource_count = 0;
                    else
                        queryParentF4.resource_count = queryParentF4.resource_count - nCount;

                    //更新父节点数量（第一级）
                    var queryParent5 = from s in db.t_course_struct
                                       where s.delete_flag == 0 && s.id == queryParentF4.parent_id
                                       select s;
                    var queryParentF5 = queryParent5.FirstOrDefault();
                    if (queryParentF5 == null)
                    {
                        db.SaveChanges();
                        return;
                    }
                    //父节点数量减一
                    if (queryParentF5.resource_count == null)
                        queryParentF5.resource_count = 0;
                    else
                        queryParentF5.resource_count = queryParentF5.resource_count - nCount;

                    //更新父节点数量（课程）
                    var queryParent6 = from s in db.t_course_struct
                                       where s.delete_flag == 0 && s.id == queryParentF5.parent_id
                                       select s;
                    var queryParentF6 = queryParent6.FirstOrDefault();
                    if (queryParentF6 == null)
                    {
                        db.SaveChanges();
                        return;
                    }
                    //父节点数量减一
                    if (queryParentF6.resource_count == null)
                        queryParentF6.resource_count = 0;
                    else
                        queryParentF6.resource_count = queryParentF6.resource_count - nCount;

                    db.SaveChanges();

                }
            }
            catch (Exception ex)
            {
                string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
                PubMethod.ErrorLog(ex, path);
            }
        }

        /// <summary>
        /// 获取子节点下所有资源数量
        /// </summary>
        /// <param name="nodeId"></param>
        /// <returns></returns>
        private int GetChildResourceCount(long nodeId)
        {
            try
            {
                using (var db = new pf_course_manageContext())
                {
                    int nCount = 0;
                    //第一级课程--内连接
                    var queryResource1 = from r in db.t_course_struct
                                         where r.delete_flag == 0 && r.id == nodeId
                                         select r;
                    var queryResource1F = queryResource1.FirstOrDefault();
                    if (queryResource1F == null)
                        return nCount;
                    var querySR1 = from s in db.t_struct_resource
                                   where s.course_struct_id == queryResource1F.id
                                   select s;
                    nCount = nCount + querySR1.Count();

                    //第二级目录--内连接
                    var queryResource2 = from r in db.t_course_struct
                                         where r.delete_flag == 0 && r.parent_id == queryResource1F.id
                                         select r;
                    var queryResource2F = queryResource2.FirstOrDefault();
                    if (queryResource2F == null)
                        return nCount;
                    else
                    {
                        var querySR2 = from r in db.t_course_struct
                                       join s in db.t_struct_resource on r.id equals s.course_struct_id
                                       where r.delete_flag == 0 && r.parent_id == queryResource1F.id
                                       select r;
                        nCount = nCount + querySR2.Count();
                    }

                    //第三级目录--内连接
                    var queryResource3 = from r in db.t_course_struct
                                         where r.delete_flag == 0 && r.parent_id == queryResource2F.id
                                         select r;
                    var queryResource3F = queryResource3.FirstOrDefault();
                    if (queryResource3F == null)
                        return nCount;
                    else
                    {
                        var querySR3 = from r in db.t_course_struct
                                       join s in db.t_struct_resource on r.id equals s.course_struct_id
                                       where r.delete_flag == 0 && r.parent_id == queryResource2F.id
                                       select r;
                        nCount = nCount + querySR3.Count();
                    }

                    //第四级目录--内连接
                    var queryResource4 = from r in db.t_course_struct
                                         where r.delete_flag == 0 && r.parent_id == queryResource3F.id
                                         select r;
                    var queryResource4F = queryResource4.FirstOrDefault();
                    if (queryResource4F == null)
                        return nCount;
                    else
                    {
                        var querySR4 = from r in db.t_course_struct
                                       join s in db.t_struct_resource on r.id equals s.course_struct_id
                                       where r.delete_flag == 0 && r.parent_id == queryResource3F.id
                                       select r;
                        nCount = nCount + querySR4.Count();
                    }

                    //第五级目录--内连接
                    var queryResource5 = from r in db.t_course_struct
                                         where r.delete_flag == 0 && r.parent_id == queryResource4F.id
                                         select r;
                    var queryResource5F = queryResource5.FirstOrDefault();
                    if (queryResource5F == null)
                        return nCount;
                    else
                    {
                        var querySR5 = from r in db.t_course_struct
                                       join s in db.t_struct_resource on r.id equals s.course_struct_id
                                       where r.delete_flag == 0 && r.parent_id == queryResource4F.id
                                       select r;
                        nCount = nCount + querySR5.Count();
                    }
                    return nCount;
                }
            }
            catch (Exception ex)
            {
                string path = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"/ErrorLog/";
                PubMethod.ErrorLog(ex, path);
                return 0;
            }
        }

    }

    public class BatchCourseTag
    {
        public List<long> CourseListID { get; set; }
        public List<Tag> tags { get; set; }
    }

    public class CourseList
    {
        public List<long> list { get; set; }
        public string UserNumber { get; set; }
    }
    public class CourseStruct
    {
        public CourseStruct()
        {
            Children = new List<CourseStruct>();
            ParentID = 0;
        }
        public long ID { get; set; }
        public string NodeNama { get; set; }
        public string NodeType { get; set; }
        public string ResourceType { get; set; }
        public string ResourceExtension { get; set; }
        public string LearningProcess { get; set; }
        public long ParentID { get; set; }
        public int? ResourceCount { get; set; }
        public int Sort { get; set; }
        public List<CourseStruct> Children { get; set; }
    }

    public class CourseStructNode
    {
        public long CourseID { get; set; }
        public long ParentID { get; set; }
        public string NodeName { get; set; }
        public string NodeType { get; set; }
        public long CreateBy { get; set; }

    }
    public class QueryCriteria
    {
        public string CourseNameTag { get; set; }
        public long UserID { get; set; }
        public long TagID { get; set; }
        public int PageSize { get; set; }
        public int PageIndex { get; set; }
        public string FieldName { get; set; }
        public long PlanID { get; set; }
        public bool IsAsc { get; set; }

    }

    public class Course_TagList
    {
        public long ID { get; set; }
        public string CourseName { get; set; }
        public string CourseDesc { get; set; }
        public string ImagePath { get; set; }
        public string PartPath { get; set; }
        public decimal CourseCount { get; set; }
        public string LearningTime { get; set; }
        public DateTime? CreateTime { get; set; }
        public string CreateName { get; set; }
        public string ApprovalUserNumber { get; set; }
        public string ApprovalStatus { get; set; }
        public string ApprovalRemark { get; set; }

        public List<Tag> Tag { get; set; }
    }

    public class CourseTag
    {
        public long CourseID { get; set; }
        public string CourseName { get; set; }
        public string CourseDesc { get; set; }
        public decimal CourseCount { get; set; }
        public string LearningTime { get; set; }
        public string ImagePath { get; set; }
        public string PartPath { get; set; }
        public long CreateBy { get; set; }
        public string CreateName { get; set; }
        public string CreateNumber { get; set; }
        public List<Tag> TagList { get; set; }
    }

    public class Tag
    {
        public long? ID { get; set; }
        public string TagName { get; set; }
    }

    public class StructResource
    {
        public long NodeID { get; set; }
        public List<Resource> ResourceList { get; set; }
    }

    public class Resource
    {
        public long ID { get; set; }
        public long SrcID { get; set; }
        public string ResourceName { get; set; }
        public string ResourceExtension { get; set; }
        public string ResourceDesc { get; set; }
        public string ResourceType { get; set; }
        public int? ResourceTime { get; set; }
        public string GroupName { get; set; }
        public string ResourceUrl { get; set; }
        public string ResourceConfidential { get; set; }
        public long? CreateBy { get; set; }

    }
}
